import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChildren, inject } from '@angular/core';
import { FormGroup, FormBuilder, FormControlName, ValidatorFn, AbstractControl, Validators } from '@angular/forms';
import { Observable, fromEvent, merge, startWith, map } from 'rxjs';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { RegistrationCounterService } from 'src/app/modules/event/services/registration-counter.service';
import { RegistrationCounterLookupModel } from 'src/app/shared/models/registration-counter-lookup.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { AssignRegistrationCounterModel } from '../../models/assigned-registration-counter.model';
import { UserAssignedRegistrationCounterModel } from '../../models/user-assigned-registration-counter.model';
import { Router } from '@angular/router';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'facets-counter-selection',
  templateUrl: './counter-selection.component.html',
  styleUrls: ['./counter-selection.component.scss']
})
export class CounterSelectionComponent implements OnInit {

  isFormSubmitted = false;
  isBlocked = false;
  filteredOptions: Observable<RegistrationCounterLookupModel[]>;

  counterTypes: string[] = ['PaymentOnly', 'RegistrationAndPayment'];

  registrationCounterForm: FormGroup;
  searchRequstModel = new SearchRequestModel(100, 1);
  registrationCounterLookupModel: RegistrationCounterLookupModel[]
  assignRegistrationCounterModel = new AssignRegistrationCounterModel();
  validationModel: ValidationModel = new ValidationModel();

  router = inject(Router);
  formBuilder = inject(FormBuilder);
  lookupService = inject(LookupsService);
  toasterService = inject(ToasterService);
  authService = inject(AuthService);
  registrationCounterService = inject(RegistrationCounterService);

  dialogRef = inject(MatDialogRef<CounterSelectionComponent>);

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor() {
    this.validationModel.validationMessages = {
      counter: {
        required: 'Counter is required',
        invalid: 'invalid counter'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createRegistrationCounterForm();
    this.getRegistrationCounter();
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
          // this.isProfile.emit(true);
          this.router.navigate(['/admin/visitor/pass-generation']);
          this.dialogRef.close();
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }
  discard() {
    this.router.navigate(['/admin/event']);
    this.dialogRef.close();
  }
}

