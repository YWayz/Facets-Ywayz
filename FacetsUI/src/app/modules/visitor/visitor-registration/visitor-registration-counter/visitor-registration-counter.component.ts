import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChildren, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormControlName, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Observable, fromEvent, map, merge, startWith } from 'rxjs';
import { CounterType, OnSitePayingMode, appConstant } from 'src/app/core/extensions/app-constants';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { RegistrationCounterLookupModel } from 'src/app/shared/models/registration-counter-lookup.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { AssignRegistrationCounterModel } from '../../models/assigned-registration-counter.model';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { RegistrationCounterService } from 'src/app/modules/event/services/registration-counter.service';
import { UserAssignedRegistrationCounterModel } from '../../models/user-assigned-registration-counter.model';
import { PaymentSettingsModel } from 'src/app/modules/event/models/payment-settings.model';
import { EventSettingsService } from 'src/app/modules/event/services/event-settings.service';

@Component({
  selector: 'facets-visitor-registration-counter',
  templateUrl: './visitor-registration-counter.component.html',
  styleUrls: ['./visitor-registration-counter.component.scss']
})
export class VisitorRegistrationCounterComponent implements OnInit {

  isFormSubmitted = false;
  isBlocked = false;
  filteredOptions: Observable<RegistrationCounterLookupModel[]>;

  eventId = '';

  counterTypes: string[] = [];

  registrationCounterForm: FormGroup;
  searchRequstModel = new SearchRequestModel(100, 1);
  registrationCounterLookupModel: RegistrationCounterLookupModel[]
  assignRegistrationCounterModel = new AssignRegistrationCounterModel();
  validationModel: ValidationModel = new ValidationModel();
  paymentSettingsModel = new PaymentSettingsModel();

  formBuilder = inject(FormBuilder);
  lookupService = inject(LookupsService);
  toasterService = inject(ToasterService);
  authService = inject(AuthService);
  registrationCounterService = inject(RegistrationCounterService);
  eventSettingsService = inject(EventSettingsService);

  @Input() isRegistrationCounter: boolean;
  @Output() isProfile = new EventEmitter<boolean>();
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor() {
    this.validationModel.validationMessages = {
      counter: {
        required: 'Counter is required',
        invalid: 'invalid counter'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);

    this.eventId = localStorage.getItem(appConstant.selectedEventId)!;
  }

  ngOnInit(): void {
    this.createRegistrationCounterForm();
    this.getPaymentSettings();
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.registrationCounterForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.registrationCounterForm, this.isFormSubmitted);
  }

  createRegistrationCounterForm() {
    this.registrationCounterForm = this.formBuilder.group({
      counter: ['']
    });
  }

  getRegistrationCounter() {
    this.isBlocked = true;

    if (this.paymentSettingsModel.onSitePayingMode == OnSitePayingMode.payAtRegistration) {
      this.counterTypes.push(CounterType.RegistrationAndPayment);
    }
    else if(this.paymentSettingsModel.onSitePayingMode == OnSitePayingMode.payAtPassGeneration){
      this.counterTypes.push(CounterType.RegistrationOnly);
    }

    this.lookupService.getAllRegistrationCounter(localStorage.getItem(appConstant.selectedEventId)!, this.searchRequstModel, this.counterTypes, false).subscribe({
      next: (result: ResponseResult<RegistrationCounterLookupModel[]>) => {
        this.isBlocked = false;
        this.registrationCounterLookupModel = result.data;
        this.filteredOptions = this.registrationCounterForm.get('counter')!.valueChanges.pipe(
          startWith(''),
          map(value => this._filter(value || '')),
        );
        this.setCounterValidators();

      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  private _filter(value: string): RegistrationCounterLookupModel[] {
    const filterValue = value.toLowerCase();

    return this.registrationCounterLookupModel.filter(option => option.name.toLowerCase().includes(filterValue));
  }

  counterValidator(field: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const group = control.parent;
      const counter = group!.get(field);
      const counters = this.registrationCounterLookupModel.map(m => m.name);
      if (counters.indexOf(counter?.value) !== -1) return null;
      return counter?.value != "" ? { invalid: control.value } : null;
    }
  }

  setCounterValidators() {
    this.registrationCounterForm.controls['counter'].setValidators([Validators.required, this.counterValidator('counter')]);
    this.registrationCounterForm.controls['counter'].updateValueAndValidity();
  }

  save() {
    this.isFormSubmitted = true;
    this.validate();

    if (this.registrationCounterForm.invalid) { return; }

    this.isBlocked = true;
    const counterId = this.registrationCounterLookupModel.find(f => f.name == this.registrationCounterForm.value.counter)?.id!;
    this.assignRegistrationCounterModel.userId = this.authService.userId!;

    this.registrationCounterService.createRegistrationCounterAssignmment(counterId, this.assignRegistrationCounterModel)
      .subscribe({
        next: (res: ResponseResult<UserAssignedRegistrationCounterModel>) => {
          this.isProfile.emit(true);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

  getPaymentSettings() {
    this.eventSettingsService.getPaymentSettings(this.eventId).subscribe({
      next: (res: ResponseResult<PaymentSettingsModel>) => {
        this.paymentSettingsModel = res.data;

        this.getRegistrationCounter();
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      },
    })
  }

}
