import { Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChildren, inject } from '@angular/core';
import { FormGroup, FormControlName, FormBuilder, Validators } from '@angular/forms';
import { Observable, fromEvent, merge } from 'rxjs';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { ResetPasswordModel } from '../models/reset-password.model';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'facets-reset-password-link',
  templateUrl: './reset-password-link.component.html',
  styleUrls: ['./reset-password-link.component.scss']
})
export class ResetPasswordLinkComponent implements OnInit {

  isPasswordHide = true;
  isConfirmPasswordHide = true;

  isFormSubmitted = false;
  isBlocked = false;
  displayStyle = "none";
  displaySuccessMessageStyle = "none";

  resetPasswordLinkForm: FormGroup;
  resetPasswordModel: ResetPasswordModel;

  @Output() isResetPasswordLinkChange = new EventEmitter<boolean>;

  validationModel: ValidationModel = new ValidationModel();
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  router = inject(Router) as Router;

  constructor(private fb: FormBuilder, private authService: AuthService, private toasterService: ToasterService) { 
    this.validationModel.validationMessages = {
      email: {
        required: 'Email is required',
        pattern: 'Please enter valid email format'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void { 
    this.resetPasswordLinkCreateForm();
  }

  resetPasswordLinkCreateForm() {
    this.resetPasswordLinkForm = this.fb.group({
      email: ['', [Validators.required]]
    });
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.resetPasswordLinkForm.valueChanges, ...controlBlurs).subscribe(value => {
      this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.resetPasswordLinkForm, this.isFormSubmitted);
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.resetPasswordLinkForm, this.isFormSubmitted);
  }

  sendResetEmail() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.resetPasswordLinkForm.invalid) { return; }
    this.isBlocked = true;
    this.resetPasswordModel = Object.assign({}, this.resetPasswordModel, this.resetPasswordLinkForm.value);
    this.authService.forgotPassword(this.resetPasswordModel)
      .subscribe({
        next: () => {
          this.isFormSubmitted = false;
          this.isBlocked = false;
          this.displaySuccessMessageStyle = 'block';
          this.toasterService.success("Reset password email send");
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

  navigateHome(){
    this.router.navigate(['/admin'])
  }
}
