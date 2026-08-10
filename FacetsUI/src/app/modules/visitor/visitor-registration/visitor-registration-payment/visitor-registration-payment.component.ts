import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChildren, inject } from '@angular/core';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { Observable, fromEvent, merge } from 'rxjs';
import { PaymentModel } from '../../models/payment.model';
import { PaymentService } from '../../services/payment.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { OnSitePayingMode, appConstant } from 'src/app/core/extensions/app-constants';
import { VisitorRegisterModel } from '../../models/visitor-register.model';
import { VisitorRegistrationService } from '../../services/visitor-registration.service';
import { SharedService } from 'src/app/core/services/shared.service';
import { EventSettingsService } from 'src/app/modules/event/services/event-settings.service';
import { PaymentSettingsModel } from 'src/app/modules/event/models/payment-settings.model';

@Component({
  selector: 'facets-visitor-registration-payment',
  templateUrl: './visitor-registration-payment.component.html',
  styleUrls: ['./visitor-registration-payment.component.scss']
})
export class VisitorRegistrationPaymentComponent implements OnInit, AfterViewInit {

  isBlocked = false;
  isFormSubmitted = false;
  isShowForCardPayment = false;
  isDisableButton = false;
  totalFinalAmount = 0;
  totalPavilionAmount = 0;

  onSitePayingModel = OnSitePayingMode;

  eventId = ''

  paymentForm: FormGroup;
  paymentModel: PaymentModel;
  paymentSettingsModel = new PaymentSettingsModel();
  validationModel: ValidationModel = new ValidationModel();

  paymentService = inject(PaymentService);
  toasterService = inject(ToasterService);
  authService = inject(AuthService);
  formBuilder = inject(FormBuilder);
  visitorRegistrationService = inject(VisitorRegistrationService);
  eventSettingsService = inject(EventSettingsService);
  sharedService = inject(SharedService);

  @Input() visitorId: string;
  @Input() visitorEventRegistrationModel: VisitorRegisterModel | undefined;
  @Input() isPavilionSessionExist: boolean
  @Input() finalAmount: number;
  @Output() isRegisterSuccess = new EventEmitter<string>();
  @Output() isPass = new EventEmitter<string>();
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor() {
    this.validationModel.validationMessages = {
      lastFourDigitsofCard: {
        pattern: 'Please enter valid digit'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);

    this.eventId = localStorage.getItem(appConstant.selectedEventId)!.toString();
  }

  ngOnInit(): void {
    this.createPaymentForm();
    if (this.isPavilionSessionExist) {
      const amounts = this.sharedService.visitorRegistrationModel!.pavilionSessions.map(m => m.amount);
      this.totalPavilionAmount = amounts.reduce((total, amount) => total + amount, 0);
    }
    else {
      this.totalPavilionAmount = 0;
    }
    // this.totalFinalAmount = this.finalAmount + this.totalPavilionAmount;

    if(this.finalAmount > 3000)
      {

        this.totalFinalAmount = 3000 + this.totalPavilionAmount;
        this.finalAmount=3000;

      }else{
        this.totalFinalAmount = this.finalAmount+ this.totalPavilionAmount;

      }

    this.getPaymentSettings();
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.paymentForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.paymentForm, this.isFormSubmitted);
  }

  createPaymentForm() {
    this.paymentForm = this.formBuilder.group({
      paymentMethod: ['cash', Validators.required],
      lastFourDigitsofCard: ['', Validators.pattern('^[0-9]*$')],
      referenceNumber: [''],
    });
  }

  onPaymentMethod() {
    const paymentMethod = this.paymentForm.get('paymentMethod')?.value;

    if (paymentMethod == 'card') this.isShowForCardPayment = true;
    else {
      this.isShowForCardPayment = false;
      this.createPaymentForm();
    }
  }

  back() {
    this.isPass.emit(this.visitorId);
  }

  getPaymentSettings() {
    this.isBlocked = true;
    this.eventSettingsService.getPaymentSettings(this.eventId).subscribe({
      next: (res: ResponseResult<PaymentSettingsModel>) => {
        this.paymentSettingsModel = res.data;        
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      },
    })
  }

  save() {
    this.isDisableButton = true;
    if (!this.sharedService.visitorRegistrationModel?.id) {
      if (this.isPavilionSessionExist) {
        this.sharedService.visitorRegistrationModel!.pavilionSessionIds = this.sharedService.visitorRegistrationModel!.pavilionSessions.map(m => m.id)
      }
      this.visitorRegistrationService.create(this.sharedService.visitorRegistrationModel!)
        .subscribe({
          next: (res: ResponseResult<VisitorRegisterModel>) => {
            if (this.paymentSettingsModel.onSitePayingMode == OnSitePayingMode.payAtRegistration) {
              this.savePayment(res.data.id);
            } else {
              this.isBlocked = false;
              this.isFormSubmitted = false;
              this.sharedService.onSitePayingMode = this.paymentSettingsModel.onSitePayingMode;
              this.isRegisterSuccess.emit(this.visitorId);
              const selectedEvent = this.authService.authData?.accessibleEvents.find(f => f.id == localStorage.getItem(appConstant.selectedEventId));
              this.toasterService.success(`The visitor has been successfully registered to the ${selectedEvent.name}`)
            }
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.isDisableButton = false;
            this.isFormSubmitted = false;
            this.toasterService.error(err);
          }
        });
    }
    else {
      if (this.isPavilionSessionExist) {
        this.sharedService.visitorRegistrationModel!.pavilionSessionIds = this.sharedService.visitorRegistrationModel!.pavilionSessions.map(m => m.id)
      }
      this.visitorRegistrationService.update(this.sharedService.visitorRegistrationModel!.id, this.sharedService.visitorRegistrationModel!).subscribe({
        next: () => {
          if (this.paymentSettingsModel.onSitePayingMode == OnSitePayingMode.payAtRegistration) {
            this.savePayment(this.sharedService.visitorRegistrationModel!.id)
          } else {
            this.isBlocked = false;
            this.isFormSubmitted = false;
            this.sharedService.onSitePayingMode = this.paymentSettingsModel.onSitePayingMode;
            this.isRegisterSuccess.emit(this.visitorId);
            const selectedEvent = this.authService.authData?.accessibleEvents.find(f => f.id == localStorage.getItem(appConstant.selectedEventId));
            this.toasterService.successfullyUpdated(`The registration for the ${selectedEvent.name}`)
          }
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.isDisableButton = false;
          this.toasterService.error(err);
        }
      });
    }



  }

  savePayment(visitorRegistrationId: string) {
    this.isFormSubmitted = true;
    this.validate();

    if (this.paymentForm.invalid) { return; }

    this.isBlocked = true;
    this.paymentModel = Object.assign({}, this.paymentModel, this.paymentForm.value);
    if (this.totalFinalAmount === 0) this.paymentModel.paymentMethod = 'NoPaymentNeeded'
    this.paymentModel.amount = this.totalFinalAmount;
    this.paymentModel.registrationId = visitorRegistrationId;
    this.paymentModel.visitorId = this.visitorId;
    this.paymentModel.rateType = this.sharedService.visitorRegistrationModel?.rateType!;

    this.paymentService.create(this.paymentModel)
      .subscribe({
        next: () => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.isRegisterSuccess.emit(this.visitorId);
          this.sharedService.onSitePayingMode = this.paymentSettingsModel.onSitePayingMode;
          const selectedEvent = this.authService.authData?.accessibleEvents.find(f => f.id == localStorage.getItem(appConstant.selectedEventId));
          this.toasterService.success(`The visitor has been successfully registered to the ${selectedEvent.name}`)
          this.isDisableButton = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.isDisableButton = false;
          this.toasterService.error(err);
        }
      });
  }
}
