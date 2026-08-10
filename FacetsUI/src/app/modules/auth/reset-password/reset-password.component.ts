import { Component, ElementRef, OnInit, ViewChildren } from '@angular/core';
import { FormGroup, FormControlName, FormBuilder, Validators } from '@angular/forms';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { ResetPasswordModel } from '../models/reset-password.model';
import { ActivatedRoute } from '@angular/router';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { AuthService } from '../services/auth.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { Observable, fromEvent, merge } from 'rxjs';
import { ErrorResponse } from 'src/app/core/models/error-response.model';

@Component({
  selector: 'facets-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit{
  isFormSubmitted = false;
  isBlocked = false;
  isPasswordHide = true;
  isConfirmPasswordHide = true;
  isPasswordReset = false;

  resetPasswordForm: FormGroup;
  resetPasswordModel = new ResetPasswordModel();

  validationModel: ValidationModel = new ValidationModel();
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute, private toasterService: ToasterService, private authService: AuthService) {
  
    this.validationModel.validationMessages = {
      newPassword: {
        required: 'New password is required',
      },
      confirmPassword: {
        required: 'Confirm new password is required',
      },
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.resetPasswordCreateForm();
  }

  resetPasswordCreateForm() {
    this.resetPasswordForm = this.fb.group({
      newPassword: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.resetPasswordForm.valueChanges, ...controlBlurs).subscribe(value => {
      this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.resetPasswordForm, this.isFormSubmitted);
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.resetPasswordForm, this.isFormSubmitted);
  }

  resetPassword() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.resetPasswordForm.invalid) { return; }
    this.isBlocked = true;
    this.resetPasswordModel = Object.assign({}, this.resetPasswordModel, this.resetPasswordForm.value);
    const isMatch = this.resetPasswordModel.newPassword == this.resetPasswordModel.confirmPassword;
    if(!isMatch) {
      this.isBlocked = false;
      return this.toasterService.warning("Confirm password and password does not match");
    }
    this.resetPasswordModel.email = this.activatedRoute.snapshot.queryParams.email;
    this.resetPasswordModel.token = this.activatedRoute.snapshot.queryParams.pwdresettoken;
    this.authService.resetPassword(this.resetPasswordModel)
      .subscribe({
        next: () => {
          this.isFormSubmitted = false;
          this.isBlocked = false;
          this.isPasswordReset = true;
          this.toasterService.success("Password reset successfully");
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

}
