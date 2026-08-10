import { AfterViewInit, Component, ElementRef, Inject, OnInit, ViewChildren, inject } from '@angular/core';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable, fromEvent, merge } from 'rxjs';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { RegistrationCounterModel } from 'src/app/modules/event/models/registration-counter.model';
import { RegistrationCounterService } from 'src/app/modules/event/services/registration-counter.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';

@Component({
  selector: 'facets-registration-counters-create',
  templateUrl: './registration-counters-create.component.html',
  styleUrls: ['./registration-counters-create.component.scss']
})
export class RegistrationCountersCreateComponent implements OnInit, AfterViewInit {
  
  isFormSubmitted = false;
  isBlocked = false;

  registrationCounterForm: FormGroup;
  registrationCounterModel: RegistrationCounterModel;
  validationModel: ValidationModel = new ValidationModel();

  formBuilder = inject(FormBuilder);
  registrationCounterService = inject(RegistrationCounterService);
  toasterService = inject(ToasterService);
  dialogRef = inject(MatDialogRef<RegistrationCountersCreateComponent>);
  
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor(@Inject(MAT_DIALOG_DATA) public dialogData: { data: { id: string, isEdit: boolean, eventId: string } }) {
    this.validationModel.validationMessages = {
      name: {
        required: 'Registration counter name is required',
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createRegistrationCounterForm();

    if(this.dialogData.data.isEdit) {
      this.getRegistrationCounter();
    }
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
      name: ['', Validators.required],
      description: [''],
      counterType: ['RegistrationAndPayment']
    });
  }

  getRegistrationCounter() {
    this.isBlocked = true;
    this.registrationCounterService.getRegistrationCounter(this.dialogData.data.eventId, this.dialogData.data.id).subscribe({
      next: (result: ResponseResult<RegistrationCounterModel>) => {
        this.isBlocked = false;
        this.registrationCounterModel = result.data
        this.registrationCounterForm.patchValue(this.registrationCounterModel);
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    })
  }

  create() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.registrationCounterForm.invalid) { return; }

    this.isBlocked = true;
    this.registrationCounterModel = Object.assign({}, this.registrationCounterModel, this.registrationCounterForm.value);
    this.registrationCounterService.create(this.dialogData.data.eventId, this.registrationCounterModel).subscribe({
      next: () => {
        this.isBlocked = false;
        this.dialogRef.close();
        this.toasterService.successfullyCreated("Registration counter");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }

  update() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.registrationCounterForm.invalid) { return; }

    this.isBlocked = true;
    this.registrationCounterModel = Object.assign({}, this.registrationCounterModel, this.registrationCounterForm.value);
    this.registrationCounterService.update(this.dialogData.data.eventId, this.registrationCounterModel.id, this.registrationCounterModel).subscribe({
      next: () => {
        this.isBlocked = false;
        this.dialogRef.close();
        this.toasterService.successfullyUpdated("Registration counter");
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
