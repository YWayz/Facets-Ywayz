import { Component, OnInit, inject } from '@angular/core';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { UserService } from '../../services/user.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { UserModel } from '../../models/user.model';
import { UserProfileService } from '../../services/user-profile.service';
import { UserProfileModel } from '../../models/user-profile.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { patterns } from 'src/app/core/extensions/rejex-pattern';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { UpdateUserPasswordModel } from '../../models/update-user-password.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { Router } from '@angular/router';

@Component({
  selector: 'facets-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {

  isEdit = false;
  isBlocked = false;
  isFormSubmitted = false;

  user: UserProfileModel = new UserProfileModel();
  changePassword: UpdateUserPasswordModel = new UpdateUserPasswordModel();
  fileList: File[] = [];

  userForm: FormGroup;
  passwordForm: FormGroup;

  authService = inject(AuthService) as AuthService;
  userProfileService = inject(UserProfileService) as UserProfileService;
  toasterService = inject(ToasterService) as ToasterService;
  router = inject(Router);

  validationModel: ValidationModel = new ValidationModel();

  constructor(private formBuilder: FormBuilder) {
    this.validationModel.validationMessages = {
      userName: {
        required: 'Username is required'
      },
      email: {
        required: 'Email is required',
        pattern: 'Please enter valid email format'
      },
      firstName: {
        required: 'First name is required',
      },
      lastName: {
        required: 'Last name is required'
      },
      currentPassword: {
        required: 'Password is required'
      },
      newPassword: {
        required: 'Password is required'
      },
      confirmPassword: {
        required: 'Confirm password is required'
      },
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.userForm, this.isFormSubmitted);
  }

  ngOnInit(): void {
    this.createUserForm();
    this.createPasswordForm();
    this.getUserById(this.authService.userId!);
  }

  getUserById(id: string) {
    this.userProfileService.getById(id).subscribe({
      next: (res: ResponseResult<UserProfileModel>) => {
        this.user = res.data;
        this.patchUser(this.user);
      },
    })
  }

  getFiles(event: File[]) {
    if (event.length > 0) {
      this.fileList = event;      
    }
  }

  createUserForm() {
    this.userForm = this.formBuilder.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.pattern(patterns.email)]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      roles: ['', Validators.required],
      timeZone: ['Sri Lanka Standard Time'],
      imageURL: ['']
    });
  }

  createPasswordForm() {
    this.passwordForm = this.formBuilder.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required]],
      confirmPassword: ['', Validators.required],
    });
  }

  patchUser(userProfileModel: UserProfileModel) {
    this.user = Object.assign(userProfileModel);
    this.userForm.patchValue(this.user);
    this.userForm.disable();

    if (this.user.imageURL == null)
      this.user.imageURL = '../../assets/images/no-image.png'
  }

  edit() {
    this.userForm.enable();
    this.userForm.get("roles")!.disable()
    this.userForm.get("email")!.disable()
    this.isEdit = true;
  }

  updateChangePassword() {
    this.validate();
    if (this.passwordForm.invalid) { return; }

    this.isFormSubmitted = true;
    this.isBlocked = true;
    this.changePassword = Object.assign({}, this.changePassword, this.passwordForm.value);

    const isMatch = this.changePassword.newPassword == this.changePassword.confirmPassword;
    if (!isMatch) {
      this.isBlocked = false;
      return this.toasterService.warning("Confirm password and new password does not match");
    }

    this.userProfileService.changePassword(this.authService.userId!, this.changePassword).subscribe({
      next: () => {
        this.isFormSubmitted = false;
        this.isBlocked = false;
        this.isEdit = false;
        localStorage.clear();
        this.router.navigate(['login']);
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        return this.toasterService.errorSaving(err);
      },
    })
  }

  updateProfile() {
    this.validate();
    if (this.userForm.invalid) { return; }

    this.isBlocked = true;
    this.isFormSubmitted = true;
    this.user = Object.assign({}, this.user, this.userForm.value);

    this.userProfileService.updateProfile(this.authService.userId!, this.user).subscribe({
      next: () => {
        this.uploadImage(this.authService.userId!, "UPDATE");
      },
    })
  }

  discard() {
    this.isEdit = false;
  }

  uploadImage(id: string, action: "CREATE" | "UPDATE") {
    if (this.fileList.length == 0) {
      this.isFormSubmitted = false;
      this.isBlocked = false;
      this.afterUploadNavigation(action);
      return;
    }

    this.userProfileService.uploadImage(id, this.fileList)
      .subscribe({
        next: (res: any) => {
          this.isFormSubmitted = false;
          this.isBlocked = false;
          this.toasterService.successfullyUpdated("User");
          this.getUserById(this.authService.userId!);
          this.discard();
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  afterUploadNavigation(action: "CREATE" | "UPDATE") {
    this.toasterService.successfullyUpdated("User");
    this.router.navigate(['admin/user-management/user-profile']);
  }
}
