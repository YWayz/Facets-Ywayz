import { AfterViewInit, Component, ElementRef, Inject, OnInit, ViewChildren, inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { Observable, fromEvent, merge } from 'rxjs';
import { LoginModel } from '../models/login.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { Router } from '@angular/router';
import { patterns } from 'src/app/core/extensions/rejex-pattern';

@Component({
  selector: 'facets-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, AfterViewInit {

  isFormSubmitted = false;
  isRequested = false;

  loginModel = new LoginModel();
  userPermissionNames = new Array<string>();
  validationModel: ValidationModel = new ValidationModel();

  loginForm: FormGroup;

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  authService = inject(AuthService);
  formBuilder = inject(FormBuilder);
  router = inject(Router) as Router;
  toasterService = inject(ToasterService);

  constructor() {
    this.validationModel.validationMessages = {
      email: {
        required: 'Email is required',
      },
      password: {
        required: 'Password is required',
      },
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createLoginForm();
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.loginForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.loginForm, this.isFormSubmitted);
  }

  createLoginForm() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  authenticate() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.loginForm.invalid) { return; }

    this.isRequested = true;
    this.loginModel = Object.assign({}, this.loginModel, this.loginForm.value);
    this.authService.login(this.loginModel)
      .subscribe({
        next: (res: ResponseResult<string>) => {
          this.isFormSubmitted = false;
          localStorage.setItem(appConstant.jwtTokenName, JSON.stringify(res.data));
          this.setUserPermissions(JSON.stringify(res.data));
        },
        error: (err: ErrorResponse) => {
          this.isFormSubmitted = this.isRequested = false;
          this.toasterService.error(err);
        }
      });
  }

  setUserPermissions(res: any) {
    let result = JSON.parse(res);
    result.claims.forEach((claim: any) => {
      this.userPermissionNames.push(claim.claimValue);
    });
    localStorage.setItem('claims', JSON.stringify(this.userPermissionNames));
    this.router.navigate(['/admin']);
  }

  openResetPasswordLink() {
    this.router.navigate(['/reset-password-link']);
  }
}
