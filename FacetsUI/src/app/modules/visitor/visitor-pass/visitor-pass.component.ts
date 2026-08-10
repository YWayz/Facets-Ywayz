import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { VisitorRegistrationService } from '../services/visitor-registration.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { VisitorPassModel } from '../models/visitor-pass.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { EventService } from '../../event/services/event.service';
import { appConstant, OnSitePayingMode } from 'src/app/core/extensions/app-constants';
import { EventDetailModel } from '../../event/models/event-detail.model';
import { EventDateSelectionModel } from '../../report/models/event-date-selection.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { convertOnlyDate, isEmpty } from 'src/app/core/extensions/helpers';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalService } from 'src/app/core/services/modal.service';
import { VisitorPassQrScanComponent } from './visitor-pass-qr-scan/visitor-pass-qr-scan.component';
import { PassGenerationPermissions, SuperAdminPermissions, VisitorRegistrationPermissions } from 'src/app/core/extensions/permission-constants';
import { AuthService } from '../../auth/services/auth.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { EventSettingsService } from '../../event/services/event-settings.service';
import { PaymentSettingsModel } from '../../event/models/payment-settings.model';
import { VisitorRegistrationCounterComponent } from '../visitor-registration/visitor-registration-counter/visitor-registration-counter.component';
import { CounterSelectionComponent } from './counter-selection/counter-selection.component';
import { RegistrationCounterService } from '../../event/services/registration-counter.service';
import { CounterAssignmentStatusModel } from '../../event/models/counter-assignment-status.model';

@Component({
  selector: 'facets-visitor-pass',
  templateUrl: './visitor-pass.component.html',
  styleUrls: ['./visitor-pass.component.scss']
})
export class VisitorPassComponent implements OnInit {

  isBlocked = false;
  canGeneratePass = false;
  isPrinted = false;
  hasCounterAssigned = false;

  currentDate = convertOnlyDate(new Date());

  counterTypes: string[] = ['PaymentOnly', 'RegistrationAndPayment'];

  paymentSettingsModel = new PaymentSettingsModel();
  visitorPassModels = new Array<VisitorPassModel>();
  eventDates = new Array<EventDateSelectionModel>();
  eventDateId: string;
  attendanceSheduleId: string;
  eventId: string;
  passGenerationPermissions = PassGenerationPermissions;
  superAdminPermissions = SuperAdminPermissions;

  onSitePayingMode = OnSitePayingMode;

  searchModel = new SearchRequestModel(10, 1);
  pageSizeOptions: number[] = [10, 25, 50, 100];

  visitorRegistrationService = inject(VisitorRegistrationService);
  toasterService = inject(ToasterService);
  eventService = inject(EventService);
  eventSettingsService = inject(EventSettingsService);
  router = inject(Router);
  activatedRoute = inject(ActivatedRoute);
  modalService = inject(ModalService);
  authService = inject(AuthService)
  registrationCounterService = inject(RegistrationCounterService);

  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor() {
    this.eventId = localStorage.getItem(appConstant.selectedEventId)!.toString();
    this.getPaymentSettings();

  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe({
      next: (params: Params) => {
        const nicPassport = params['nicPassport'];
        if (nicPassport != undefined || nicPassport != null) {
          this.searchModel.searchTerm = nicPassport;
        }
      }
    });

    if (this.authService.hasPermissionAuthorization([this.superAdminPermissions.all, this.passGenerationPermissions.view])) {
      this.getEventDatesByEventId(this.eventId!);
    }
  }

  checkCounterAssignment() {
    this.isBlocked = true;
    this.registrationCounterService.registrationCounterAssignment(this.counterTypes).subscribe({
      next: (result: ResponseResult<CounterAssignmentStatusModel>) => {
        this.hasCounterAssigned = result.data.hasCounterAssigned;

        if (this.paymentSettingsModel.onSitePayingMode == OnSitePayingMode.payAtPassGeneration ||
          this.paymentSettingsModel.payLaterForOnlineRegistration == true) {

          if (this.hasCounterAssigned == false) {
            this.modalService.displayDialog(CounterSelectionComponent)
            this.isBlocked = false;
          }
        }
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    })
  }

  getPaymentSettings() {
    this.isBlocked = true;
    this.eventSettingsService.getPaymentSettings(this.eventId).subscribe({
      next: (res: ResponseResult<PaymentSettingsModel>) => {
        this.paymentSettingsModel = res.data;
        this.isBlocked = false;
        this.checkCounterAssignment();
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      },
    })
  }

  getEventDatesByEventId(eventId: string) {
    this.isBlocked = true;
    this.eventService.getById(eventId).subscribe({
      next: (res: ResponseResult<EventDetailModel>) => {
        res.data.eventDates.map(p => this.eventDates.push({ key: p.key, value: p.value, isSelected: false }));

        const today = convertOnlyDate(new Date());
        const eventDateSelectionModel = this.eventDates.find(f => convertOnlyDate(f.value) == today)
        if (eventDateSelectionModel != undefined) {
          const eventDateSelection = this.eventDates.filter(f => convertOnlyDate(f.value) == today)[0];
          eventDateSelection.isSelected = true;

          let date = this.eventDates.find(s => s.key == eventDateSelection.key)?.value;
          let convertedDate = convertOnlyDate(date!);
          this.canGeneratePass = this.currentDate == convertedDate;
          this.getVisitorPassByDates(eventDateSelection.key);
        }

        this.isBlocked = false;
      },
    })
  }

  selectEventDate(eventDateId: string) {
    this.eventDates = this.eventDates.map(eventDate => ({
      ...eventDate,
      isSelected: eventDate.key === eventDateId
    }));

    let date = this.eventDates.find(s => s.key == eventDateId)?.value;
    let convertedDate = convertOnlyDate(date!);
    this.canGeneratePass = this.currentDate == convertedDate;
    this.getVisitorPassByDates(eventDateId);
  }

  getVisitorPassByDates(eventDateId: string) {
    this.isBlocked = true;
    this.eventDateId = eventDateId

    this.visitorRegistrationService.getVisitorPassesByDates(this.searchModel, this.eventDateId, this.attendanceSheduleId, true, this.isPrinted).subscribe({
      next: (res: ResponseResult<VisitorPassModel[]>) => {
        this.visitorPassModels = res.data;
        this.searchModel.totalRecords = res.totalRecordCount;
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
        this.isBlocked = false;
      },
    })
  }

  searchVisitor() {
    this.getVisitorPassByDates(this.eventDateId)
  }

  search() {
    this.searchModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    if (this.eventDateId != undefined) {
      if (!isEmpty(this.searchModel.searchTerm) && this.searchModel.searchTerm.length > 0)
        this.searchVisitor();
      else if (isEmpty(this.searchModel.searchTerm)) {
        this.searchVisitor();
      }
    }
    else {
      this.toasterService.warning("Event date is not selected");
    }
  }

  clearSearchTerm() {
    if (this.eventDateId != undefined) {
      this.searchModel.searchTerm = '';
      this.search();
    }
    else {
      this.toasterService.warning("Event date is not selected")
    }
  }
 
  navigateToVisitorPassProfile(visitorId: string, attendanceScheduleId: string, registrationId: string) {
    this.router.navigate([`admin/visitor/pass-generation/${visitorId}/profile/${attendanceScheduleId}/registrations/${registrationId}`], { queryParams: { eventDateId: this.eventDateId } }
    );
  }

  scanQr() {
    this.modalService.displayDialog(VisitorPassQrScanComponent);
    this.modalService.confirmed().subscribe((attendanceSheduleId: string) => {
      if (attendanceSheduleId != undefined) {
        this.eventDates = [];
        this.attendanceSheduleId = attendanceSheduleId;
        const eventId = localStorage.getItem(appConstant.selectedEventId)
        this.getEventDatesByEventId(eventId!);
      }
    });
  }

  generatePass(visitorId: string, attendanceScheduleId: string) {
    this.router.navigate(['admin/visitor/pass-generation/print', visitorId, this.eventDateId, attendanceScheduleId]);
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    // this.getRegistrations();
    this.searchVisitor();
  }

  reset() {
    this.attendanceSheduleId = '';
    this.isPrinted = false;
    this.clearSearchTerm();
  }

  showAll() {
    this.isPrinted = true;
    if (this.eventDateId != undefined) {
      this.getVisitorPassByDates(this.eventDateId);
    }
    else {
      this.toasterService.warning("Event date is not selected")
    }
  }

  goToPayLater(visitorRegistrationId: string, visitorPassModel: VisitorPassModel) {
    this.router.navigate([`admin/visitor/pay-later/${visitorRegistrationId}`], { queryParams: { nicPassport: visitorPassModel.nicNumber == null ? visitorPassModel.passportNumber : visitorPassModel.nicNumber } })

  }
}
