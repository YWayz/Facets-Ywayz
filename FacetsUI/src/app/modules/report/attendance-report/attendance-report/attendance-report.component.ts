import { Component, OnInit, ViewChild, inject } from '@angular/core';
// import { VisitorReportModel } from '../models/visitor-report.model';
// import { VisitorReportFilterModel } from '../models/visitor-report-filter.model';
import { SharedService } from 'src/app/core/services/shared.service';
// import { ReportService } from '../services/report.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
// import { EventDetailModel } from '../../event/models/event-detail.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { KeyValue } from 'src/app/core/models/key-value.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
// import { EventDateSelectionModel } from '../models/event-date-selection.model';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { Observable, forkJoin, map, startWith } from 'rxjs';
import { ExcelService } from 'src/app/core/services/excel.service';
import { EventDateSelectionModel } from '../../models/event-date-selection.model';
import { ReportService } from '../../services/report.service';
import { VisitorReportFilterModel } from '../../models/visitor-report-filter.model';
import { VisitorReportModel } from '../../models/visitor-report.model';
import { EventDetailModel } from 'src/app/modules/event/models/event-detail.model';
import { AttendanceReportModel } from '../../models/attendence-report-model';

@Component({
  selector: 'facets-attendance-report',
  templateUrl: './attendance-report.component.html',
  styleUrls: ['./attendance-report.component.scss']
})
export class AttendanceReportComponent {

  isBlocked = false;
  isAllSelected = true;
  showFilter = false;

  visitors: AttendanceReportModel[];
  attendanceReportFilterModel = new VisitorReportFilterModel();
  eventDates = new Array<EventDateSelectionModel>();
  searchModel = new SearchRequestModel(10, 1);
  pageSizeOptions: number[] = [10, 25, 50, 100];
  countries: KeyValue<string, string>[] = [];
  passCategories: KeyValue<string, string>[] = [];
  visitorStatuses: string[] = [
    "Active", "BlackListed"
  ];
  isSpecific = false;
  selectedReportFilter: 'specific' | 'range' = 'range';


  sharedService = inject(SharedService);
  reportService = inject(ReportService);
  toasterService = inject(ToasterService);
  lookupService = inject(LookupsService);
  excelService = inject(ExcelService);

  @ViewChild(MatPaginator) paginator: MatPaginator;

  public get eventId() {
    return this.sharedService.getEventId();;
  }

  constructor() {
  }

  ngOnInit(): void {
    this.getKeyValues();
    this.getEventDates();
  }

  getKeyValues() {
    let getCountries = this.lookupService.getCountries();
    let getPassCategories = this.lookupService.getPassCategories(new SearchRequestModel(100, 1), this.eventId, '&passCategoryType=AssocifyMember&passCategoryType=LocalAndForeignVisitor&passCategoryType=LocalBuyer&passCategoryType=ForeignBuyer&passCategoryType=Custom_Visitor');
    forkJoin([getCountries, getPassCategories]).subscribe(results => {
      this.countries = results[0].data;
      this.passCategories = results[1].data;

      // var allKeyvalue: KeyValue<string, string> = { key: "", value: 'All' };
      // this.passCategories.unshift(allKeyvalue);
    }, error => {
      this.isBlocked = false;
      this.toasterService.error(error);
    });
  }

  getEventDates() {
    this.isBlocked = true;
    this.reportService.getEventById(this.eventId)
      .subscribe({
        next: (res: ResponseResult<EventDetailModel>) => {
          res.data.eventDates.map(p => this.eventDates.push({ key: p.key, value: p.value, isSelected: true }));
          this.attendanceReportFilterModel.eventId = this.eventId;
          this.attendanceReportFilterModel.eventDateIds = this.eventDates.map(p => p.key);
          this.getVisitorDetails();
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  getVisitorDetails() {
    this.isBlocked = true;
    this.reportService.getAttendenceReport(this.searchModel, this.attendanceReportFilterModel)
      .subscribe({
        next: (res: ResponseResult<AttendanceReportModel[]>) => {
          this.visitors = res.data;
          this.searchModel.totalRecords = res.totalRecordCount;
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    this.getVisitorDetails();
  }

  changeSelection(eventDate: EventDateSelectionModel) {
    var res = this.eventDates.find(p => p.key == eventDate.key)!;
    res.isSelected = !res.isSelected;
    this.attendanceReportFilterModel.eventDateIds = this.eventDates.filter(p => p.isSelected).map(p => p.key);
    this.isAllSelected = this.eventDates.every(p => p.isSelected);

    if (this.attendanceReportFilterModel.eventDateIds.length != 0)
      this.getVisitorDetails();
    else
      this.visitors = [];
  }

  selectAll() {
    if (this.isAllSelected)
      this.eventDates.forEach(p => p.isSelected = false);
    else
      this.eventDates.forEach(p => p.isSelected = true);

    this.attendanceReportFilterModel.eventDateIds = this.eventDates.filter(p => p.isSelected).map(p => p.key);
    this.isAllSelected = this.eventDates.every(p => p.isSelected);

    if (this.isAllSelected)
      this.getVisitorDetails();
    else
      this.visitors = [];
  }

  cancel() {
    this.attendanceReportFilterModel.visitorEventRegistrationFromDate = '';
    this.attendanceReportFilterModel.visitorEventRegistrationToDate = '';
  }

  cancelFilter() {
    this.showFilter = !this.showFilter;
    this.eventDates = new Array<EventDateSelectionModel>();
    this.attendanceReportFilterModel = new VisitorReportFilterModel();
    this.getEventDates();
  }

  export() {
    this.searchModel.pageNumber = 1;
    this.searchModel.pageSize = this.searchModel.totalRecords;

    let headers = ["First Name", "Last Name", "Identification", "Country", "Pass Category", "Mobile No", "Email", "Company", "Status"];

    this.reportService.getAttendenceReport(this.searchModel, this.attendanceReportFilterModel)
      .subscribe({
        next: (res: ResponseResult<VisitorReportModel[]>) => {
          this.searchModel.totalRecords = res.totalRecordCount;

          if (res.data.length == 0)
            return this.toasterService.warning("No Records Found");

          this.excelService.generateExcel('Attendance', headers, res.data);

          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  search() {
    this.searchModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getVisitorDetails();
  }

  getVisitorStatusClass(status: string) {
    if (status == 'Active')
      return 's-active';
    if (status == 'BlackListed')
      return 's-blacklisted';
    if (status == 'Cancelled')
      return 's-cancel';
    return '';
  }

  changeFilter() {
    if (this.selectedReportFilter == 'specific')
      this.isSpecific = true;
    else this.isSpecific = false;

    if (!this.isSpecific)
      this.attendanceReportFilterModel.visitorEventRegistrationSpecificDate = undefined;
    else {
      this.attendanceReportFilterModel.visitorEventRegistrationFromDate = undefined;
      this.attendanceReportFilterModel.visitorEventRegistrationToDate = undefined;
    }
  }
}
