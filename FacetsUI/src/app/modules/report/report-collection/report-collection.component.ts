import { KeyValue } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { ExcelService } from 'src/app/core/services/excel.service';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { SharedService } from 'src/app/core/services/shared.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { EventDateSelectionModel } from '../models/event-date-selection.model';
import { ReportService } from '../services/report.service';
import { CollectionReportModel } from '../models/collection-report.model';
import { CollectionReportFilterModel } from '../models/collection-report-filter.model';
import { RegistrationCounterLookupModel } from 'src/app/shared/models/registration-counter-lookup.model';

@Component({
  selector: 'facets-report-collection',
  templateUrl: './report-collection.component.html',
  styleUrls: ['./report-collection.component.scss']
})
export class ReportCollectionComponent implements OnInit {

  isBlocked = false;
  isAllSelected = true;
  showFilter = false;
  isSpecific = false;

  collections: CollectionReportModel[];
  totalCollection: CollectionReportModel = new CollectionReportModel();
  collectionReportFilterModel = new CollectionReportFilterModel();
  eventDates = new Array<EventDateSelectionModel>();
  passCategories: KeyValue<string, string>[] = [];
  users: KeyValue<string, string>[] = [];
  counters: RegistrationCounterLookupModel[];
  selectedReportFilter: 'specific' | 'range' = 'range';

  reportService = inject(ReportService);
  toasterService = inject(ToasterService);
  lookupService = inject(LookupsService);
  excelService = inject(ExcelService);

  constructor(private sharedService: SharedService) {
  }

  public get eventId() {
    return this.sharedService.getEventId();;
  }

  ngOnInit(): void {
    this.getKeyValues();
    this.getCollectionReportDetails();
  }

  getKeyValues() {
    let getPassCategories = this.lookupService.getPassCategories(new SearchRequestModel(100, 1), this.eventId, '&passCategoryType=AssocifyMember&passCategoryType=LocalAndForeignVisitor&passCategoryType=LocalBuyer&passCategoryType=ForeignBuyer&passCategoryType=Custom_Visitor');
    let getCounters = this.lookupService.getAllRegistrationCounter(this.eventId, new SearchRequestModel(50, 1))
    let getAssignedUsers = this.lookupService.getAssignedEventUsers(this.eventId);
    forkJoin([getPassCategories, getCounters, getAssignedUsers]).subscribe(results => {
      this.passCategories = results[0].data;
      this.counters = results[1].data;
      this.users = results[2].data;
    }, error => {
      this.isBlocked = false;
      this.toasterService.error(error);
    });
  }

  getCollectionReportDetails() {
    this.isBlocked = true;
    this.collectionReportFilterModel.eventId = this.eventId;
    this.reportService.getCollectionReport(this.collectionReportFilterModel)
      .subscribe({
        next: (res: ResponseResult<CollectionReportModel[]>) => {
          this.collections = res.data;

          if (this.collections.length != 0) {
            this.totalCollection.description = 'Total Registrations';
            this.totalCollection.registeredCount = this.collections.reduce((accumulator, object) => {
              return accumulator + object.registeredCount;
            }, 0);
            this.totalCollection.amount = this.collections.reduce((accumulator, object) => {
              return accumulator + object.amount;
            }, 0);
            this.collections.push(this.totalCollection);
          }
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  cancelFilter() {
    this.showFilter = !this.showFilter;
    this.eventDates = new Array<EventDateSelectionModel>();
    this.collectionReportFilterModel = new CollectionReportFilterModel();
    this.getCollectionReportDetails();
  }

  export() {
    if (this.collections.length == 0)
      return this.toasterService.warning("No Records Found");

    let headers = ["Description","Card Count","Card Amount","Cash Count","Cash Amount","Total Count", "Collected Amount"];
    this.excelService.generateExcel("Collection", headers, this.collections[0]);
    console.log(headers)
    console.log(this.collections)
  }

  cancel() {
    this.collectionReportFilterModel.paidFromDate = '';
    this.collectionReportFilterModel.paidToDate = '';
  }

  changeFilter() {
    if (this.selectedReportFilter == 'specific')
      this.isSpecific = true;
    else this.isSpecific = false;

    if (!this.isSpecific)
      this.collectionReportFilterModel.specificDate = undefined;
    else {
      this.collectionReportFilterModel.paidFromDate = undefined;
      this.collectionReportFilterModel.paidToDate = undefined;
    }
  }
}
