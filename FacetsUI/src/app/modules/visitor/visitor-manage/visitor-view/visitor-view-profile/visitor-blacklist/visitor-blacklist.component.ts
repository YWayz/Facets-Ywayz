import { AfterViewInit, Component, ElementRef, Inject, OnInit, ViewChildren, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormControlName, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable, fromEvent, merge } from 'rxjs';
import { convertOnlyDate } from 'src/app/core/extensions/helpers';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { MarkAsBlacklistedModel } from 'src/app/modules/visitor/models/mark-as-blacklisted.model';
import { VisitorsService } from 'src/app/modules/visitor/services/visitors.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';

@Component({
  selector: 'facets-visitor-blacklist',
  templateUrl: './visitor-blacklist.component.html',
  styleUrls: ['./visitor-blacklist.component.scss']
})
export class VisitorBlacklistComponent implements OnInit, AfterViewInit {

  isBlocked = false;
  isFormSubmitted = false;

  visitorBlacklistForm: FormGroup;
  markAsBlacklistedModel: MarkAsBlacklistedModel;
  validationModel: ValidationModel = new ValidationModel();

  formBuilder = inject(FormBuilder);
  toasterService = inject(ToasterService);
  visitorsService = inject(VisitorsService);
  dialogRef = inject(MatDialogRef<VisitorBlacklistComponent>);

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor(@Inject(MAT_DIALOG_DATA) public dialogData: { data: { fullName: string, visitorId: string } }) {
    this.validationModel.validationMessages = {
      reason: {
        required: 'Reason is required',
      },
      blackListUntil: {
        required: 'Until date is required',
        invalid: 'Please select a future date'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createBlacklistForm();
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.visitorBlacklistForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.visitorBlacklistForm, this.isFormSubmitted);
  }

  createBlacklistForm() {
    this.visitorBlacklistForm = this.formBuilder.group({
      reason: ['', Validators.required],
      blackListUntil: [new Date()]
    });
    this.untilDateSetValidator();
  }

  untilDateSetValidator() {
    this.visitorBlacklistForm.controls['blackListUntil'].setValidators([Validators.required, this.untilDateValidator('blackListUntil')]);
    this.visitorBlacklistForm.controls['blackListUntil'].updateValueAndValidity();
  }

  untilDateValidator(field: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const group = control.parent;
      const discountValidUntilDate = group!.get(field);
      const untilDateString = convertOnlyDate(discountValidUntilDate!.value);
      const currentDate = convertOnlyDate(new Date());
      const isGreateThan = untilDateString > currentDate;
      return !isGreateThan ? { invalid: control.value } : null;
    }
  }

  discard() {
    this.dialogRef.close();
  }

  update() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.visitorBlacklistForm.invalid) { return; }

    this.isBlocked = true;
    this.markAsBlacklistedModel = Object.assign({}, this.markAsBlacklistedModel, this.visitorBlacklistForm.value);
    this.visitorsService.markAsBlacklisted(this.dialogData.data.visitorId, this.markAsBlacklistedModel).subscribe({
      next: () => {
        this.isBlocked = false;
        this.dialogRef.close();
        this.toasterService.success("Blacklisted successfully");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }
}
