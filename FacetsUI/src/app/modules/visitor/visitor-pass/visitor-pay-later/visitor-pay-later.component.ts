import { AfterViewInit, Component, ElementRef, OnInit, ViewChildren, inject } from '@angular/core';
import { FormGroup, FormBuilder, FormControlName, Validators } from '@angular/forms';
import { Observable, fromEvent, merge, of, switchMap } from 'rxjs';
import { OnSitePayingMode, appConstant } from 'src/app/core/extensions/app-constants';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { PaymentSettingsModel } from 'src/app/modules/event/models/payment-settings.model';
import { EventSettingsService } from 'src/app/modules/event/services/event-settings.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { PaymentModel } from '../../models/payment.model';
import { VisitorRegisterModel } from '../../models/visitor-register.model';
import { PaymentService } from '../../services/payment.service';
import { VisitorRegistrationService } from '../../services/visitor-registration.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { PassCategoryService } from 'src/app/modules/event/services/pass-category.service';
import { PassCategoryModel } from 'src/app/modules/event/models/pass-category.model';
import { convertOnlyDate } from 'src/app/core/extensions/helpers';
import { EventDetailModel } from 'src/app/modules/event/models/event-detail.model';
import { EventService } from 'src/app/modules/event/services/event.service';

@Component({
  selector: 'facets-visitor-pay-later',
  templateUrl: './visitor-pay-later.component.html',
  styleUrls: ['./visitor-pay-later.component.scss']
})
export class VisitorPayLaterComponent implements OnInit, AfterViewInit {

  isBlocked = false;
  isFormSubmitted = false;
  isShowForCardPayment = false;
  isDisableButton = false;
  isFlatRate = false;

  isFlatRateEventDate: { startDate: string, endDate: string };

  totalFinalAmount = 0;
  totalPavilionAmount = 0;
  appliedDiscount = ''

  amount = 0;

  onSitePayingModel = OnSitePayingMode;

  eventId = ''
  visitorRegistrationId = ''
  finalAmount: number = 0;
  nicPassport: string = '';

  paymentForm: FormGroup;
  paymentModel: PaymentModel;
  eventDetailModel = new EventDetailModel();
  visitorRegisterModel = new VisitorRegisterModel();
  passCategoryModel = new PassCategoryModel();
  paymentSettingsModel = new PaymentSettingsModel();
  validationModel: ValidationModel = new ValidationModel();

  eventDates = new Array<any>();

  activatedRoute = inject(ActivatedRoute);
  eventService = inject(EventService);
  router = inject(Router);
  paymentService = inject(PaymentService);
  toasterService = inject(ToasterService);
  passCategoryService = inject(PassCategoryService);
  authService = inject(AuthService);
  formBuilder = inject(FormBuilder);
  visitorRegistrationService = inject(VisitorRegistrationService);
  eventSettingsService = inject(EventSettingsService);

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
    this.activatedRoute.queryParams.subscribe({
      next: (params: Params) => {
        this.nicPassport = params['nicPassport'];
      }
    });

    this.activatedRoute.params.subscribe({
      next: (param: Params) => {
        this.visitorRegistrationId = param['visitorRegistrationId'];


        console.log(this.nicPassport);
        if (this.visitorRegistrationId != undefined) {
          this.getEventById(this.eventId);
          this.getRegistrationById();
        }
      },
    })

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
    this.router.navigate(['/admin/visitor/pass-generation'], { queryParams: { nicPassport: this.nicPassport } });
  }

  getRegistrationById() {
    this.isBlocked = true;
    this.visitorRegistrationService.getById(this.visitorRegistrationId)
      .pipe(switchMap((visitorRegistrationResult: ResponseResult<VisitorRegisterModel>) => {
        this.visitorRegisterModel = visitorRegistrationResult.data;
        this.passCategoryService.getPassCategory(this.eventId, visitorRegistrationResult.data.passCategoryId).subscribe({
          next: (passCategoryResult: ResponseResult<PassCategoryModel>) => {
            this.passCategoryModel = passCategoryResult.data;

            this.passDiscount();
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.toasterService.error(err);
          }
        })
        return of();
      }))
      .subscribe({
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isDisableButton = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  getEventById(eventId: string) {
    this.isBlocked = true;
    this.eventService.getById(eventId)
      .subscribe({
        next: (res: ResponseResult<EventDetailModel>) => {
          this.eventDetailModel = res.data;
          this.eventDetailModel.eventDates.map(m => this.eventDates.push({ key: m.key, value: m.value, isSelected: false, isInvoiced: false, isPassedDate: false }));
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
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      },
    })
  }

  save() {
    this.isDisableButton = true;
    this.savePayment(this.visitorRegisterModel!.id)
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
    this.paymentModel.visitorId = this.visitorRegisterModel.visitorId;
    this.paymentModel.rateType = this.visitorRegisterModel?.rateType!;

    this.paymentService.create(this.paymentModel)
      .subscribe({
        next: () => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          const selectedEvent = this.authService.authData?.accessibleEvents.find(f => f.id == localStorage.getItem(appConstant.selectedEventId));
          this.toasterService.success(`The visitor has been successfully registered to the ${selectedEvent.name}`)
          this.isDisableButton = false;
          this.router.navigate(['/admin/visitor/pass-generation'], { queryParams: { nicPassport: this.nicPassport } });
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.isDisableButton = false;
          this.toasterService.error(err);
        }
      });
  }

  passDiscount() {
    this.isBlocked = true;
    const passRate = this.passCategoryModel.passCategorySettings[0];

    if (passRate.rateType == 'FlatRate') {
      this.isFlatRate = true;
    }
    else {
      this.isFlatRate = false;
      this.isFlatRateEventDate = { startDate: '', endDate: '' };
    }

    if (passRate.isChargeable) {
      if (!passRate.applyEntireEventDiscountedRate && !passRate.applyEarlyRegistrationDiscountedRate) {
        this.appliedDiscount = '';
        this.amount = passRate.rate;
      }

      if (passRate.applyEntireEventDiscountedRate) {
        const unInvoiced = this.visitorRegisterModel.visitorAttendanceSchedules.filter(f => f.isInvoiced == false);
        if (unInvoiced.length === this.eventDates.map(m => m.key).length) {
          this.appliedDiscount = 'All-Day pass discount';
          this.amount = passRate.discountedRate;
        }
        else {
          this.appliedDiscount = '';
          if (passRate.rateType == 'FlatRate') {
            this.amount = 0;
          }
          else {
            this.amount = passRate.rate;
          }
        }
      }

      if (passRate.applyEarlyRegistrationDiscountedRate) {
        const validUntilDate = convertOnlyDate(passRate.earlyRegistrationDiscountedRateValidUntil);
        const currentDate = convertOnlyDate(new Date());
        if (currentDate <= validUntilDate) {
          this.appliedDiscount = 'Early registration discount';
          this.amount = passRate.discountedRate;
        }
        else {
          this.appliedDiscount = '';
          this.amount = passRate.rate;
        }
      }
    }
    else this.amount = 0;

    this.finalAmount = passRate.rateType == "PerDayRate" ? this.amount *= this.visitorRegisterModel.visitorAttendanceSchedules.filter(f => f.isInvoiced == false).length : this.amount;

    const passCategoryPavilionSettingsModels = this.passCategoryModel.passCategoryPavilionSettings.filter(s => s.passCategoryId == this.visitorRegisterModel.passCategoryId);

    passCategoryPavilionSettingsModels.forEach(passCategoryPavilionSettingsModel => {

      if (passCategoryPavilionSettingsModel.passCategoryId == this.visitorRegisterModel.passCategoryId) {

        this.totalPavilionAmount += passCategoryPavilionSettingsModel.pavilionRate * this.visitorRegisterModel.visitorPavilionSessionAttendanceSchedules.filter(s => s.isInvoiced == false && s.pavilionId == this.visitorRegisterModel.visitorPavilionSessionAttendanceSchedules.find(s => s.pavilionId == passCategoryPavilionSettingsModel.pavilionId)?.pavilionId).length;
      }
    });

    // this.totalFinalAmount = this.finalAmount + this.totalPavilionAmount;
    if(this.finalAmount > 3000)
      {

        this.totalFinalAmount = 3000 + this.totalPavilionAmount;
        this.finalAmount=3000;

      }else{
        this.totalFinalAmount = this.finalAmount+ this.totalPavilionAmount;

      }

    this.isBlocked = false;
  }
}
