import { Component, ElementRef, Inject, OnInit, ViewChildren, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormControlName, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { DateFilterFn } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable, fromEvent, merge } from 'rxjs';
import { convertDateToUtc, convertOnlyDate } from 'src/app/core/extensions/helpers';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { PassCategoryRatesModel } from 'src/app/modules/event/models/pass-category-rates.model';
import { PassCategorySettingsService } from 'src/app/modules/event/services/pass-category-settings.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';

@Component({
  selector: 'facets-pass-rates-update',
  templateUrl: './pass-rates-update.component.html',
  styleUrls: ['./pass-rates-update.component.scss']
})
export class PassRatesUpdateComponent implements OnInit {

  isFormSubmitted = false;
  isBlocked = false;
  applyOnlineRegistrationDiscountedRate = false;
  applyEarlyRegistrationDiscountedRate = false;
  applyEntireEventDiscountedRate = false;
  applyFlatRate = false;
  minDate = new Date();
  maxDate = new Date();

  passRatesForm: FormGroup;
  passCategoryRatesModel: PassCategoryRatesModel;
  validationModel: ValidationModel = new ValidationModel();

  formBuilder = inject(FormBuilder);
  passCategorySettingsService = inject(PassCategorySettingsService);
  toasterService = inject(ToasterService);
  dialogRef = inject(MatDialogRef<PassRatesUpdateComponent>);

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor(@Inject(MAT_DIALOG_DATA) public dialogData: { data: { id: string, isEdit: boolean, passCategoryId: string, isChargable: boolean, passCategoryName: string, eventId: string } }) {
    this.validationModel.validationMessages = {
      rate: {
        required: 'Rate is required',
        min: 'Rate cannot be zero'
      },
      discountedRate: {
        required: 'Discount rate is required',
        max: 'Discount rate cannot be equal to or greater than the rate',
      },
      earlyRegistrationDiscountedRateValidUntil: {
        required: 'Early registration discount date is required',
        max: 'Early registration date cannot be on or before the registration start date and cannot be after end date.'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createPassRatesForm();
    if (this.dialogData.data.isEdit) {
      this.getPassRate();
    }
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.passRatesForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.passRatesForm, this.isFormSubmitted);
  }

  createPassRatesForm() {
    this.passRatesForm = this.formBuilder.group({
      rate: ['', Validators.required],
      discountedRate: ['', Validators.required],
      applyEarlyRegistrationDiscountedRate: [false],
      applyOnlineRegistrationDiscountedRate: [false],
      applyEntireEventDiscountedRate: [false],
      earlyRegistrationDiscountedRateValidUntil: [new Date()],
      rateType: ['PerDayRate']
    });
  }

  getPassRate() {
    this.isBlocked = true;
    this.passCategorySettingsService.getPassRate(this.dialogData.data.eventId, this.dialogData.data.passCategoryId, this.dialogData.data.id).subscribe({
      next: (result: ResponseResult<PassCategoryRatesModel>) => {
        this.isBlocked = false;
        this.passCategoryRatesModel = result.data
        this.passCategoryRatesModel.rateType == 'None' ? this.passCategoryRatesModel.rateType = 'PerDayRate' : this.passCategoryRatesModel.rateType;
        this.passRatesForm.patchValue(this.passCategoryRatesModel);

        this.applyEarlyRegistrationDiscountedRate = this.passCategoryRatesModel.applyEarlyRegistrationDiscountedRate;
        this.applyOnlineRegistrationDiscountedRate = this.passCategoryRatesModel.applyOnlineRegistrationDiscountedRate;
        this.applyEntireEventDiscountedRate = this.passCategoryRatesModel.applyEntireEventDiscountedRate;
        
        this.passCategoryRatesModel.rateType == 'FlatRate' ? this.applyFlatRate = true: this.applyFlatRate = false;

        this.minDate = new Date(convertOnlyDate(this.passCategoryRatesModel.visitorRegistrationStartsOn));
        this.minDate.setDate(this.minDate.getDate() + 1);

        this.maxDate = new Date(convertOnlyDate(this.passCategoryRatesModel.visitorRegistrationEndsOn));

        this.passRatesForm.get('rate')?.markAsTouched();
        this.passRatesForm.get('discountedRate')?.markAsTouched();

        this.enableDisableDiscountRate();
        this.setValidations();
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    })
  }

  enableDisableDiscountRate() {
    if (!this.applyEarlyRegistrationDiscountedRate && !this.applyOnlineRegistrationDiscountedRate && !this.applyEntireEventDiscountedRate) this.passRatesForm.get('discountedRate')?.disable();
    else this.passRatesForm.get('discountedRate')?.enable();
  }

  setValidations() {
    this.passRatesForm.controls['rate'].setValidators([Validators.required, Validators.min(0.00)]);
    this.passRatesForm.controls['rate'].updateValueAndValidity();

    this.passRatesForm.controls['discountedRate'].setValidators([Validators.required, Validators.min(0.00), this.discountRateValidator('discountedRate')]);
    this.passRatesForm.controls['discountedRate'].updateValueAndValidity();

    if (this.passCategoryRatesModel.applyEarlyRegistrationDiscountedRate)
      this.discountDateValidator();
  }

  discountDateValidator() {
    this.passRatesForm.controls['earlyRegistrationDiscountedRateValidUntil'].setValidators([Validators.required, Validators.min(0.00)]);
    this.passRatesForm.controls['earlyRegistrationDiscountedRateValidUntil'].updateValueAndValidity();
  }

  discountRateValidator(field: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const group = control.parent;
      const discountRate = group!.get(field);
      const isGreateThan = Number(discountRate!.value) >= Number(this.passRatesForm.get('rate')?.value)
      return isGreateThan ? { max: control.value } : null;
    }
  }

  discountValidUntilDateValidator(field: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const group = control.parent;
      const discountValidUntilDate = group!.get(field);
      const discountValidUntilDateString = convertOnlyDate(discountValidUntilDate!.value)
      const visitorStartDate = convertOnlyDate(this.passCategoryRatesModel.visitorRegistrationStartsOn);
      const visitorEndDate = convertOnlyDate(this.passCategoryRatesModel.visitorRegistrationEndsOn);
      const isGreateThan = (discountValidUntilDateString > visitorStartDate && discountValidUntilDateString <= visitorEndDate)
      return !isGreateThan ? { max: control.value } : null;
    }
  }

  applyOnlineRegistrationDiscountedRateChanged(event: Event) {
    this.applyOnlineRegistrationDiscountedRate = (event.target as HTMLInputElement).checked;
    this.enableDisableDiscountRate();
  }

  applyEarlyRegistrationDiscountedRateChanged(event: Event) {
    this.applyEarlyRegistrationDiscountedRate = (event.target as HTMLInputElement).checked;
    this.enableDisableDiscountRate();
    this.passRatesForm.get('earlyRegistrationDiscountedRateValidUntil')?.setValue(null);

    if (!this.applyEarlyRegistrationDiscountedRate) {
      this.passRatesForm.get('discountedRate')?.setValue(0);
      this.passRatesForm.get('earlyRegistrationDiscountedRateValidUntil')?.clearValidators()
      this.passRatesForm.get('earlyRegistrationDiscountedRateValidUntil')?.updateValueAndValidity();
    }
    else {
      this.discountDateValidator();
    }
  }

  changeSearchValue() {
    const applyFlatRateValue = this.passRatesForm.get('rateType')?.value;
    if (applyFlatRateValue === 'FlatRate') {
      this.applyFlatRate = true;
    } else {
      this.applyFlatRate = false;
    }
  }

  applyEntireEventDiscountedRateChanged(event: Event) {
    this.applyEntireEventDiscountedRate = (event.target as HTMLInputElement).checked;
    this.enableDisableDiscountRate();
  }

  update() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.passRatesForm.invalid) { return; }

    this.isBlocked = true;
    this.passCategoryRatesModel = Object.assign({}, this.passCategoryRatesModel, this.passRatesForm.value);
    this.passCategoryRatesModel.isChargeable = this.dialogData.data.isChargable;

    if (this.passCategoryRatesModel.earlyRegistrationDiscountedRateValidUntil != null || this.passCategoryRatesModel.earlyRegistrationDiscountedRateValidUntil != undefined)
      this.passCategoryRatesModel.earlyRegistrationDiscountedRateValidUntil = convertDateToUtc(this.passCategoryRatesModel.earlyRegistrationDiscountedRateValidUntil);
    this.passCategorySettingsService.update(this.dialogData.data.eventId, this.passCategoryRatesModel.passCategoryId, this.passCategoryRatesModel.id, this.passCategoryRatesModel).subscribe({
      next: () => {
        this.isBlocked = false;
        this.dialogRef.close();
        this.toasterService.successfullyUpdated("Pass category rate");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }

  discard() {
    this.dialogRef.close();
  }
}
