import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChildren, inject } from '@angular/core';
import { FormGroup, FormBuilder, FormControlName } from '@angular/forms';
import { MatSelectChange } from '@angular/material/select';
import { Observable, fromEvent, merge } from 'rxjs';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SharedService } from 'src/app/core/services/shared.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { EventDetailModel } from 'src/app/modules/event/models/event-detail.model';
import { PassCategoryModel } from 'src/app/modules/event/models/pass-category.model';
import { EventService } from 'src/app/modules/event/services/event.service';
import { PassCategoryService } from 'src/app/modules/event/services/pass-category.service';
import { PavilionService } from 'src/app/modules/event/services/pavilion.service';
import { PavilionSessionSummaryModel } from 'src/app/modules/visitor/models/pavilion-session-summary.model';
import { PavilionSummaryModel } from 'src/app/modules/visitor/models/pavilion-summary.model';
import { VisitorPavilionSessionAttendanceScheduleModel } from 'src/app/modules/visitor/models/visitor-pavilion-session-attendance-schedule.model';
import { PavilionSessionService } from 'src/app/modules/visitor/services/pavilion-session.service';
import { VerifyNicPassportComponent } from 'src/app/shared/components/registration/verify-nic-passport/verify-nic-passport.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { PublicSiteService } from '../../services/public-site.service';

@Component({
  selector: 'facets-visitor-registration-pavilion',
  templateUrl: './visitor-registration-pavilion.component.html',
  styleUrls: ['./visitor-registration-pavilion.component.scss'],
  standalone: true,
  imports: [SharedModule, VerifyNicPassportComponent]
})
export class VisitorRegistrationPavilionComponentimplements implements OnInit, AfterViewInit {

  isBlocked = false;
  isFormSubmitted = false;
  isSessionShow = false;
  checkedList = new Array<any>();
  unInvoicedCheckedList = new Array<any>();
  pavilionId: string

  passCategoryModel: PassCategoryModel;
  pavilionSummariesModel: PavilionSummaryModel[];
  visitorPavilionSessionAttendanceSchedulesModel: VisitorPavilionSessionAttendanceScheduleModel[];
  pavilionSessionSummariesModel: PavilionSessionSummaryModel[];

  pavilionForm: FormGroup;
  eventDetailModel = new EventDetailModel();
  validationModel: ValidationModel = new ValidationModel();

  formBuilder = inject(FormBuilder);
  pavilionService = inject(PavilionService);
  publicSiteService = inject(PublicSiteService);
  pavilionSessionService = inject(PavilionSessionService);
  toasterService = inject(ToasterService);
  eventService = inject(EventService);
  sharedService = inject(SharedService);
  passCategoryService = inject(PassCategoryService);

  @Input() visitorId: string;
  @Input() isPavilion: boolean;
  @Output() isPass = new EventEmitter<string>();
  @Output() isPayment = new EventEmitter<string>();
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor() {
    this.validationModel.validationMessages = {

    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createVisitorPavilionForm();
    this.getPassCategory(this.sharedService.visitorRegistrationModel!.passCategoryId)
    this.getEventById(localStorage.getItem(appConstant.selectedEventId)!);
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.pavilionForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.pavilionForm, this.isFormSubmitted);
  }

  createVisitorPavilionForm() {
    this.pavilionForm = this.formBuilder.group({
      pavilionId: [''],
      pavilionSessionIds: ['']
    });
  }

  getEventById(eventId: string) {
    this.isBlocked = true;
    this.eventService.getById(eventId)
      .subscribe({
        next: (res: ResponseResult<EventDetailModel>) => {
          this.eventDetailModel = res.data;
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  getVisitorPavilionSessions() {
    this.pavilionSessionService.getVisitorPavilionSessions(this.sharedService.visitorRegistrationModel!.id).subscribe({
      next: (result: ResponseResult<VisitorPavilionSessionAttendanceScheduleModel[]>) => {
        this.isBlocked = false;
        this.visitorPavilionSessionAttendanceSchedulesModel = result.data;
        this.sharedService.isUpdatePavilion = true;

        this.pavilionForm.patchValue({
          pavilionId: this.pavilionSummariesModel[0].id
        });

        this.pavilionSessionSummariesModel = this.pavilionSummariesModel[0].pavilionSessionSummaries;
        this.pavilionSessionSummariesModel = this.pavilionSessionSummariesModel.filter(f => this.sharedService.visitorRegistrationModel!.allEventDateIds.includes(f.eventDateId));
        if (this.pavilionSessionSummariesModel) {
          this.pavilionSessionSummariesModel.forEach(pavilionSession => pavilionSession.eventDate = this.eventDetailModel.eventDates.find(f => f.key == pavilionSession.eventDateId)?.value!);
        }

        if (this.visitorPavilionSessionAttendanceSchedulesModel.length != 0 && this.sharedService.visitorRegistrationModel!.pavilionSessions == undefined) {
          
          this.visitorPavilionSessionAttendanceSchedulesModel.filter(f => f.isCancelled == false).forEach(pavSession => {
            const pavilionSessionSummaries = this.pavilionSummariesModel.find(f => f.id == pavSession.pavilionId)!.pavilionSessionSummaries;
            let pavilionSession = pavilionSessionSummaries.find(f => f.id == pavSession.pavilionSessionId)!;
            if (pavilionSession) {
              pavilionSession.isSelected = true;
              pavilionSession.isInvoiced = pavSession.isInvoiced;
              pavilionSession.isCancelled = pavSession.isCancelled;
              pavilionSession.amount = this.passCategoryModel.passCategoryPavilionSettings.find(f => f.pavilionId == pavilionSession.pavilionId)!.pavilionRate;
              pavilionSession.eventDate = this.eventDetailModel.eventDates.find(f => f.key == pavilionSession.eventDateId)?.value!
              this.checkedList.push(pavilionSession);
            }
          });
          this.unInvoicedCheckedList.push(...this.checkedList.filter(f => f.isSelected == true && f.isInvoiced == false));
          this.pavilionForm.controls.pavilionSessionIds.patchValue(this.checkedList.map(m => m.id));
          this.pavilionForm.patchValue(this.visitorPavilionSessionAttendanceSchedulesModel!);
          this.isSessionShow = true;
        }
        else {
          if(this.sharedService.visitorRegistrationModel!.pavilionSessions != undefined) {
            this.sharedService.visitorRegistrationModel!.pavilionSessions.forEach(pavSession => {
              const pavilionSessionSummaries = this.pavilionSummariesModel.find(f => f.id == pavSession.pavilionId)!.pavilionSessionSummaries;
              let pavilionSession = pavilionSessionSummaries.find(f => f.id == pavSession.id)!;
              if (pavilionSession) {
                pavilionSession.isSelected = true;
                pavilionSession.isInvoiced = false;
                pavilionSession.isCancelled = pavSession.isCancelled;
                pavilionSession.amount = this.passCategoryModel.passCategoryPavilionSettings.find(f => f.pavilionId == pavilionSession.pavilionId)!.pavilionRate;
                pavilionSession.eventDate = this.eventDetailModel.eventDates.find(f => f.key == pavilionSession.eventDateId)?.value!
                this.checkedList.push(pavilionSession);
              }
            });
          }
                    
          this.visitorPavilionSessionAttendanceSchedulesModel.filter(f => f.isCancelled == false).forEach(pavSession => {
            const pavilionSessionSummaries = this.pavilionSummariesModel.find(f => f.id == pavSession.pavilionId)!.pavilionSessionSummaries;
            let pavilionSession = pavilionSessionSummaries.find(f => f.id == pavSession.pavilionSessionId)!;
            if (pavilionSession) {
              pavilionSession.isSelected = true;
              pavilionSession.isInvoiced = pavSession.isInvoiced;
              pavilionSession.isCancelled = pavSession.isCancelled;
              pavilionSession.amount = this.passCategoryModel.passCategoryPavilionSettings.find(f => f.pavilionId == pavilionSession.pavilionId)!.pavilionRate;
              pavilionSession.eventDate = this.eventDetailModel.eventDates.find(f => f.key == pavilionSession.eventDateId)?.value!
              this.checkedList.push(pavilionSession);
            }
          });
          this.unInvoicedCheckedList.push(...this.checkedList.filter(f => f.isSelected == true && f.isInvoiced == false));
          this.pavilionForm.controls.pavilionSessionIds.patchValue(this.checkedList.map(m => m.id));
          this.pavilionForm.patchValue(this.visitorPavilionSessionAttendanceSchedulesModel!);
          this.isSessionShow = true;
        }
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  getPavilions() {
    this.publicSiteService.getAllPavilions().subscribe({
      next: (result: ResponseResult<PavilionSummaryModel[]>) => {
        this.isBlocked = false;
        this.pavilionSummariesModel = result.data;

        if (!this.sharedService.isUpdatePavilion && (this.sharedService.visitorRegistrationModel!.pavilionSessionIds != undefined && this.sharedService.visitorRegistrationModel!.pavilionSessionIds.length > 0)) {
          this.pavilionForm.patchValue({
            pavilionId: this.pavilionSummariesModel[0].id
          });

          this.pavilionSessionSummariesModel = this.pavilionSummariesModel[0].pavilionSessionSummaries;
          this.pavilionSessionSummariesModel = this.pavilionSessionSummariesModel.filter(f => this.sharedService.visitorRegistrationModel!.allEventDateIds.includes(f.eventDateId));
          if (this.pavilionSessionSummariesModel) {
            this.pavilionSessionSummariesModel.forEach(pavilionSession => pavilionSession.eventDate = this.eventDetailModel.eventDates.find(f => f.key == pavilionSession.eventDateId)?.value!);
          }

          this.sharedService.visitorRegistrationModel!.pavilionSessions.forEach(pavSession => {
            const pavilionSessionSummaries = this.pavilionSummariesModel.find(f => f.id == pavSession.pavilionId)!.pavilionSessionSummaries;
            let pavilionSession = pavilionSessionSummaries.find(f => f.id == pavSession.id)!;
            if (pavilionSession) {
              pavilionSession.isSelected = true;
              pavilionSession.isInvoiced = false;
              pavilionSession.amount = this.passCategoryModel.passCategoryPavilionSettings.find(f => f.pavilionId == pavilionSession.pavilionId)!.pavilionRate;
              pavilionSession.eventDate = this.eventDetailModel.eventDates.find(f => f.key == pavilionSession.eventDateId)?.value!
              this.checkedList.push(pavilionSession);
            }
          });
          this.unInvoicedCheckedList.push(...this.checkedList.filter(f => f.isSelected == true && f.isInvoiced == false));
          this.pavilionForm.controls.pavilionSessionIds.patchValue(this.checkedList.map(m => m.id));
          this.isSessionShow = true;
        }

        if(this.sharedService.visitorRegistrationModel?.id) {
          this.getVisitorPavilionSessions();
        }
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  onPavilionChange(event: MatSelectChange) {
    this.isSessionShow = true;
    this.pavilionSessionSummariesModel = this.pavilionSummariesModel.find(f => f.id == event.value)!.pavilionSessionSummaries;
    this.pavilionSessionSummariesModel = this.pavilionSessionSummariesModel.filter(f => this.sharedService.visitorRegistrationModel!.allEventDateIds.includes(f.eventDateId));
    if (this.pavilionSessionSummariesModel) {
      this.pavilionSessionSummariesModel.forEach(pavilionSession => pavilionSession.eventDate = this.eventDetailModel.eventDates.find(f => f.key == pavilionSession.eventDateId)?.value!);
    }
  }

  checkUncheck(pavilionSession: any) {
    const isChecked = pavilionSession.isSelected = !pavilionSession.isSelected;

    if (isChecked) {
      pavilionSession.amount = this.passCategoryModel.passCategoryPavilionSettings.find(f => f.pavilionId == pavilionSession.pavilionId)?.pavilionRate
      this.checkedList.push(pavilionSession);
      this.unInvoicedCheckedList.push(pavilionSession);
      this.pavilionForm.controls.pavilionSessionIds.patchValue(this.checkedList.map(m => m.id));
    }
    else {
      const index = this.checkedList.findIndex(f => f.id == pavilionSession.id);
      const unInvoicedIndex = this.unInvoicedCheckedList.findIndex(f => f.id == pavilionSession.id);
      this.unInvoicedCheckedList.splice(unInvoicedIndex, 1);
      this.checkedList.splice(index, 1);
      this.pavilionForm.controls.pavilionSessionIds.patchValue(this.checkedList.map(m => m.id));
    }
  }

  getPassCategory(passCategoryId: string) {
    this.isBlocked = true;
    this.publicSiteService.getPassCategory(localStorage.getItem(appConstant.selectedEventId)!, passCategoryId).subscribe({
      next: (result: ResponseResult<PassCategoryModel>) => {
        this.isBlocked = false;
        this.passCategoryModel = result.data;
        this.getPavilions();
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    })
  }

  createPavilion() {
    this.isFormSubmitted = true;
    this.validate();

    if (this.pavilionForm.invalid) {
      if ((this.pavilionForm.value.pavilionId != undefined && this.pavilionForm.value.pavilionId != "") && this.unInvoicedCheckedList!.length === 0) {
        return this.toasterService.warning('Please select pavilion sessions');
      }
      else {
        return;
      }
    }

    this.pavilionForm.controls.pavilionSessionIds.patchValue(this.checkedList.map(m => m.id));

    this.isBlocked = true;
    this.isPayment.emit(this.visitorId);

    this.sharedService.visitorRegistrationModel!.pavilionSessions = this.unInvoicedCheckedList;
    this.sharedService.visitorRegistrationModel!.pavilionSessionIds = this.pavilionForm.value.pavilionSessionIds;
    this.sharedService.visitorRegistrationModel!.pavilionId = this.pavilionId;
    this.sharedService.visitorRegistrationModel = this.sharedService.visitorRegistrationModel;
    this.sharedService.isUpdatePavilion = false;
  }

  updatePavilion() {
    this.isFormSubmitted = true;
    this.validate();

    if (this.pavilionForm.invalid) {
      if ((this.pavilionForm.value.pavilionId != undefined && this.pavilionForm.value.pavilionId != "") && this.unInvoicedCheckedList!.length === 0) {
        return this.toasterService.warning('Please select pavilion sessions');
      }
      else {
        return;
      }
    }

    this.isBlocked = true;
    this.pavilionForm.controls.pavilionSessionIds.patchValue(this.checkedList.map(m => m.id));

    this.isPayment.emit(this.visitorId);

    this.sharedService.visitorRegistrationModel!.pavilionSessions = this.unInvoicedCheckedList;
    this.sharedService.visitorRegistrationModel!.pavilionSessionIds = this.pavilionForm.value.pavilionSessionIds;
    this.sharedService.visitorRegistrationModel!.pavilionId = this.pavilionId;
    this.sharedService.visitorRegistrationModel = this.sharedService.visitorRegistrationModel;
  }

  back() {
    this.isPass.emit(this.visitorId);
  }

  savePavilion() {
    if (this.sharedService.visitorRegistrationModel!.id == undefined) {
      this.createPavilion();
    }
    else {
      if(this.sharedService.visitorRegistrationModel?.eventDateIds.length! == 0 && this.unInvoicedCheckedList.length == 0){
        this.toasterService.warning("You have not selected event dates or pavilion sessions");
        return ;
      }
      else{

        this.updatePavilion();
      }
    }
  }
}
