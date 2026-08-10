import { AfterViewInit, Component, ElementRef, OnInit, ViewChildren, inject } from '@angular/core';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { PaymentSettingsModel } from '../../../models/payment-settings.model';
import { Observable, fromEvent, merge } from 'rxjs';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { EventSettingsService } from '../../../services/event-settings.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { PaymentSettingsPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';

@Component({
  selector: 'facets-payment-settings',
  templateUrl: './payment-settings.component.html',
  styleUrls: ['./payment-settings.component.scss']
})
export class PaymentSettingsComponent implements OnInit, AfterViewInit {

  isFormSubmitted = false;
  isBlocked = false;

  eventId: '';

  superAdminPermissions = SuperAdminPermissions;
  paymentSettingsPermissions = PaymentSettingsPermissions;

  paymentSettingsModel = new PaymentSettingsModel();
  validationModel: ValidationModel = new ValidationModel();

  paymentSettingsForm: FormGroup;

  formBuilder = inject(FormBuilder);
  activatedRoute = inject(ActivatedRoute);
  eventSettingsService = inject(EventSettingsService);
  toasterService = inject(ToasterService);

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];
  constructor() {
    this.validationModel.validationMessages = { };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createPaymentSettingsForm();
    this.activatedRoute.params.subscribe((param: Params) => {
      this.eventId = param['eventId'];
    })
    this.getPaymentSettings();
  }

  createPaymentSettingsForm() {
    this.paymentSettingsForm = this.formBuilder.group({
      onSitePayingMode: ['PayAtRegistration'],
      payLaterForOnlineRegistration: [false],
    });
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.paymentSettingsForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.paymentSettingsForm, this.isFormSubmitted);
  }

  getPaymentSettings() {
    this.isBlocked = true;
    this.eventSettingsService.getPaymentSettings(this.eventId).subscribe({
      next: (res: ResponseResult<PaymentSettingsModel>) => {
        this.paymentSettingsModel = res.data;
        this.paymentSettingsForm.patchValue(this.paymentSettingsModel);
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      },
    })
  }

  paymentModeChanged(event: Event) {
    event.preventDefault();
    const isChecked = (event.target as HTMLInputElement).checked;
    this.paymentSettingsModel.payLaterForOnlineRegistration = isChecked;
    this.paymentSettingsForm.get("payLaterForOnlineRegistration")?.setValue(isChecked);
  }

  updatePaymentSettings() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.paymentSettingsForm.invalid) { return; }

    this.isBlocked = true;
    this.paymentSettingsModel = Object.assign({}, this.paymentSettingsModel, this.paymentSettingsForm.value);

    this.eventSettingsService.updatePaymentSettings(this.eventId, this.paymentSettingsModel).subscribe({
      next: ()=> {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.successfullyUpdated("Payment Settings");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      },
    })
  }
}
