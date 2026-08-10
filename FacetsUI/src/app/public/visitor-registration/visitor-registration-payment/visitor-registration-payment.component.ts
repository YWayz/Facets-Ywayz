import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChildren, inject } from '@angular/core';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { Observable, fromEvent, merge, of, switchMap } from 'rxjs';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { PublicSiteService } from '../../services/public-site.service';
import { CreatePaymentModel } from '../../models/create-payment.model';
import { SharedService } from 'src/app/core/services/shared.service';
import { VisitorRegisterModel } from 'src/app/modules/visitor/models/visitor-register.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { PaymentModel } from '../../models/payment.model';
import { ModalService } from 'src/app/core/services/modal.service';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { PaymentRequestModel } from '../../models/payment-request.model';
import { OnepayService } from '../../services/onepay.service';
import { GatewayModel } from '../../models/gateway.model';
import { EventSettingsService } from 'src/app/modules/event/services/event-settings.service';
import { PaymentSettingsModel } from 'src/app/modules/event/models/payment-settings.model';

@Component({
  selector: 'facets-visitor-registration-payment',
  templateUrl: './visitor-registration-payment.component.html',
  styleUrls: ['./visitor-registration-payment.component.scss'],
  standalone: true,
  imports: [SharedModule]
})
export class VisitorRegistrationPaymentComponent implements OnInit, AfterViewInit {

  isBlocked = false;
  isFormSubmitted = false;
  isShowForCardPayment = false;
  isDisableButton = false;
  isDisableButtonForPayLater = false;

  eventId = '';

  gatewayModel = new GatewayModel();
  paymentSettingsModel = new PaymentSettingsModel();

  totalFinalAmount = 0;
  totalPavilionAmount = 0;

  paymentForm: FormGroup;
  paymentModel: CreatePaymentModel;
  paymentResponseModel = new PaymentModel();
  validationModel: ValidationModel = new ValidationModel();

  toasterService = inject(ToasterService);
  authService = inject(AuthService);
  formBuilder = inject(FormBuilder);
  publicSiteService = inject(PublicSiteService);
  sharedService = inject(SharedService);
  onepayService = inject(OnepayService);
  modalService = inject(ModalService);
  eventSettingsService = inject(EventSettingsService);

  // isDisableButton: boolean = false;
  isConsentGiven: boolean = false;

  @Input() visitorId: string;
  @Input() visitorEventRegistrationModel: VisitorRegisterModel | undefined;
  @Input() finalAmount: number;
  @Input() isPavilionSessionExist: boolean
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

    //Replace s request Event Id ='c7c1b543-a444-4b1d-3f0c-08dcd9764f6b'
     this.eventId = '86d0a7c8-be73-47c2-041e-08dc119fbf38'//'86d0a7c8-be73-47c2-041e-08dc119fbf38'
    //  this.eventId = localStorage.getItem(appConstant.selectedEventId)!.toString();
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
      this.publicSiteService.create(this.sharedService.visitorRegistrationModel!)
        .subscribe({
          next: (res: ResponseResult<VisitorRegisterModel>) => {
            this.sharedService.visitorRegistrationModel!.id = res.data.id;
            this.savePayment(res.data.id);
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
      this.publicSiteService.updateVisitorRegistration(this.sharedService.visitorRegistrationModel!.id, this.sharedService.visitorRegistrationModel!).subscribe({
        next: () => {
          this.savePayment(this.sharedService.visitorRegistrationModel!.id)
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isDisableButton = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
    }
  }

  payAtCounter() {
    this.isDisableButtonForPayLater = true;
    if (!this.sharedService.visitorRegistrationModel?.id) {
      if (this.isPavilionSessionExist) {
        this.sharedService.visitorRegistrationModel!.pavilionSessionIds = this.sharedService.visitorRegistrationModel!.pavilionSessions.map(m => m.id)
      }
      this.publicSiteService.create(this.sharedService.visitorRegistrationModel!)
        .subscribe({
          next: (res: ResponseResult<VisitorRegisterModel>) => {
            this.sharedService.visitorRegistrationModel!.id = res.data.id;
            if (this.paymentSettingsModel.payLaterForOnlineRegistration == true) {
              this.isRegisterSuccess.emit(this.visitorId);
              const selectedEvent = this.authService.authData?.accessibleEvents.find((f: any) => f.id == localStorage.getItem(appConstant.selectedEventId));
              this.toasterService.success(`The visitor has been successfully registered to the ${selectedEvent.name}`);
              this.isDisableButtonForPayLater = false;
            }
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.isDisableButtonForPayLater = false;
            this.isFormSubmitted = false;
            this.toasterService.error(err);
          }
        });
    }
    else {
      if (this.isPavilionSessionExist) {
        this.sharedService.visitorRegistrationModel!.pavilionSessionIds = this.sharedService.visitorRegistrationModel!.pavilionSessions.map(m => m.id)
      }
      this.publicSiteService.updateVisitorRegistration(this.sharedService.visitorRegistrationModel!.id, this.sharedService.visitorRegistrationModel!).subscribe({
        next: () => {
          if (this.paymentSettingsModel.payLaterForOnlineRegistration == true) {
            this.isBlocked = false;
            this.isFormSubmitted = false;
            this.isRegisterSuccess.emit(this.visitorId);
            const selectedEvent = this.authService.authData?.accessibleEvents.find((f: any) => f.id == localStorage.getItem(appConstant.selectedEventId));
            this.toasterService.success(`The visitor has been successfully registered to the ${selectedEvent.name}`);
            this.isDisableButton = false;
          }
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isDisableButtonForPayLater = false;
          this.isFormSubmitted = false;
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

    this.publicSiteService.createPayment(this.paymentModel)
      .pipe(switchMap((result: ResponseResult<PaymentModel>) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.paymentResponseModel = result.data;
        if (this.paymentResponseModel.isOnlinePayment && this.paymentResponseModel.totalAmount != 0) {
          this.requestPayment(this.paymentResponseModel.invoiceId);
        }
        else {
          this.isRegisterSuccess.emit(this.visitorId);
          const selectedEvent = this.authService.authData?.accessibleEvents.find((f: any) => f.id == localStorage.getItem(appConstant.selectedEventId));
          this.toasterService.success(`The visitor has been successfully registered to the ${selectedEvent.name}`);
          this.isDisableButton = false;
        }
        return of();
      }))
      .subscribe({
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isDisableButton = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

  payAtPassGeneration(visitorRegistrationId: string) {
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

    this.publicSiteService.createPayment(this.paymentModel).subscribe({
      next: (result: ResponseResult<PaymentModel>) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.paymentResponseModel = result.data;
        this.isRegisterSuccess.emit(this.visitorId);
        const selectedEvent = this.authService.authData?.accessibleEvents.find((f: any) => f.id == localStorage.getItem(appConstant.selectedEventId));
        this.toasterService.success(`The visitor has been successfully registered to the ${selectedEvent.name}`);
        this.isDisableButton = false;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isDisableButton = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      },
    });
  }

  requestPayment(invoiceId: string) {
    const paymentRequestModel = new PaymentRequestModel();
    paymentRequestModel.invoiceId = invoiceId;
    this.onepayService.requestPayment(paymentRequestModel).subscribe({
      next: (result: ResponseResult<GatewayModel>) => {
        this.gatewayModel = result.data;
        window.location.href = this.gatewayModel.redirect_url;
      },
      error: (err: ErrorResponse) => {
        this.isDisableButton = false;
        this.toasterService.error(err);
      }
    })
  }
  onConsentChange(): void {
    // Any additional logic when consent changes can be handled here
  }

  // save(): void {
  //   this.isDisableButton = true;
  //   // Add your save logic here
  //   // After saving, you can set isDisableButton back to false
  // }
}
