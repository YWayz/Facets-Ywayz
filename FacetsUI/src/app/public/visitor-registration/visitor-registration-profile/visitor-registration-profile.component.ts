import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChildren, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormControlName, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { patterns } from 'src/app/core/extensions/rejex-pattern';
import { Observable, fromEvent, map, merge, of, startWith, switchMap } from 'rxjs';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { SharedService } from 'src/app/core/services/shared.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { KeyValue } from 'src/app/core/models/key-value.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { Router } from '@angular/router';
import { FileModel } from 'src/app/shared/models/file.model';
import { SharedModule } from 'src/app/shared/shared.module';
import { VisitorSearchModel } from 'src/app/modules/visitor/models/visitor-search.model';
import { VisitorModel } from 'src/app/modules/visitor/models/visitor.model';
import { VisitorsService } from 'src/app/modules/visitor/services/visitors.service';
import { VerifyNicPassportComponent } from "../../../shared/components/registration/verify-nic-passport/verify-nic-passport.component";
import { VisitorVerificationModel } from '../../models/visitor-verification.model';
import { PublicSiteService } from '../../services/public-site.service';
import { OTPType } from 'src/app/core/extensions/app-constants';
import { UpdateVisitorModel } from '../../models/update-visitor.model';

@Component({
  selector: 'facets-visitor-registration-profile',
  templateUrl: './visitor-registration-profile.component.html',
  styleUrls: ['./visitor-registration-profile.component.scss'],
  standalone: true,
  imports: [SharedModule, VerifyNicPassportComponent]
})
export class VisitorRegistrationProfileComponent implements OnInit, AfterViewInit {

  isDisableNicPassport = false;
  isFormSubmitted = false;
  isBlocked = false;
  isView = false;
  isProfileImageViewOnly = false;
  verificationNumber = '';
  filteredOptions: Observable<KeyValue<string, string>[]>;
  countries = new Array<KeyValue<string, string>>();
  fileList: File[] = [];

  visitorRegistrationForm: FormGroup;
  updateVisitorModel: UpdateVisitorModel;
  visitorModel = new VisitorModel();
  visitorVerificationModel = new VisitorVerificationModel();
  fileModel = new Array<FileModel>();
  visitorSearchModel = new VisitorSearchModel();
  validationModel: ValidationModel = new ValidationModel();

  formBuilder = inject(FormBuilder);
  sharedService = inject(SharedService);
  toasterService = inject(ToasterService);
  publicSiteService = inject(PublicSiteService);
  visitorsService = inject(VisitorsService);
  lookupService = inject(LookupsService);
  router = inject(Router);

  @Input() nic: string;
  @Output() isOtp = new EventEmitter<any>();
  @Output() isAttachment = new EventEmitter<string>();
  @Output() sendDataForPassCategory = new EventEmitter<string[]>();
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor() {
    this.validationModel.validationMessages = {
      firstName: {
        required: 'First name is required',
      },
      lastName: {
        required: 'Last name is required',
      },
      mobileNumber: {
        required: 'Mobile number is required',
        pattern: 'Please enter valid mobile number'
      },
      countryId: {
        required: 'Country is required',
        invalidCountry: 'invalid country'
      },
      email: {
        required: 'Email is required',
        pattern: 'Please enter valid email format'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createVisitorRegistrationForm();
    this.getCountries();
    if (this.sharedService.isUpdateOtp == true) {
      this.getVisitorProfile();
    }
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.visitorRegistrationForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.visitorRegistrationForm, this.isFormSubmitted);
  }

  createVisitorRegistrationForm() {
    this.visitorRegistrationForm = this.formBuilder.group({
      visitorIdentityType: ['', Validators.required],
      nicNumber: [''],
      passportNumber: [''],
      countryId: [''],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      mobileNumber: ['', [Validators.required, Validators.pattern(patterns.mobile)]],
      companyName: [''],
      email: ['', [Validators.pattern(patterns.email), Validators.required]],
      address: this.formBuilder.group({
        address: ['']
      })
    });

    this.setCountryValidators();
  }

  getFiles(event: File[]) {
    if (event.length > 0) {
      this.fileList = event;
    }
  }

  getCountries() {
    this.lookupService.getCountries().subscribe({
      next: (result: ResponseResult<KeyValue<string, string>[]>) => {
        this.countries = result.data;
        const country = this.countries.find(f => f.value == 'Sri Lanka');
        this.visitorRegistrationForm.get('countryId')!.setValue(country?.value)
        this.filteredOptions = this.visitorRegistrationForm.get('countryId')!.valueChanges.pipe(
          startWith(''),
          map(value => this._filter(value || '')),
        );
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    });
  }

  private _filter(value: string): KeyValue<string, string>[] {
    const filterValue = value.toLowerCase();

    return this.countries.filter(option => option.value.toLowerCase().includes(filterValue));
  }

  patchVisitor(visitorSearchModel: VisitorSearchModel) {
    this.sharedService.isUpdateProfile = false;
    this.visitorModel = new VisitorModel();
    this.visitorModel.mapImage();
    this.visitorSearchModel = visitorSearchModel;
    this.visitorRegistrationForm.get('companyName')?.setValue('');
    this.visitorRegistrationForm.get('address.address')?.setValue('');
    if (this.visitorSearchModel.isRegisteredToFacets) {
      this.getVisitorProfile();
    }
    else {
      this.visitorRegistrationForm.patchValue(this.visitorSearchModel);
    }
  }

  verifyVisitor(visitorVerificationModel: VisitorVerificationModel) {
    this.isProfileImageViewOnly = false;
    this.sharedService.isUpdateProfile = false;

    this.visitorSearchModel = new VisitorSearchModel();

    this.visitorRegistrationForm.reset();
    this.fileList = [];
    this.fileModel = [];
    this.visitorModel.imageUrl = '../../../../../assets/images/no-image.png';

    Object.assign(this.visitorVerificationModel, visitorVerificationModel);

    if (visitorVerificationModel.visitorStatus == 'BlackListed') {
      return this.toasterService.warning("This User is blacklisted", "Blacklisted User")
    }
    else if (visitorVerificationModel.available == true) {
      visitorVerificationModel.otpType = OTPType.PublicSearchVisitorDetails;
      this.isOtp.emit(visitorVerificationModel);
    }
    else {
      this.isView = true;
      this.sharedService.isUpdateOtp = false;
      this.visitorRegistrationForm.patchValue({
        visitorIdentityType: visitorVerificationModel.identityType,
      })
      this.nic = visitorVerificationModel.identificationNumber;
      this.visitorVerificationModel = visitorVerificationModel;
      if (visitorVerificationModel.identityType == 'nic') {
        this.visitorRegistrationForm.patchValue({
          nicNumber: visitorVerificationModel.identificationNumber
        })
      } else {
        this.visitorRegistrationForm.patchValue({
          passportNumber: visitorVerificationModel.identificationNumber
        });
      }

      const country = this.countries.find(f => f.value == 'Sri Lanka');
      this.visitorRegistrationForm.get('countryId')!.setValue(country?.value)
      this.filteredOptions = this.visitorRegistrationForm.get('countryId')!.valueChanges.pipe(
        startWith(''),
        map(value => this._filter(value || '')),
      );
    }
  }
  getVisitorProfile() {
    this.isBlocked = true;

    this.publicSiteService
      .searchVisitor(
        this.nic ??
        this.visitorModel.nicNumber ??
        this.visitorSearchModel.nicNumber
      )
      .pipe(
        switchMap((res: ResponseResult<VisitorSearchModel>): Observable<ResponseResult<VisitorModel>> => {
          this.visitorSearchModel = res.data;
          if (this.visitorSearchModel.isAssocifyMember == true && this.visitorSearchModel.isRegisteredToFacets == false) {
            this.verificationNumber = this.visitorSearchModel.nicNumber;
            this.visitorRegistrationForm.patchValue(this.visitorSearchModel);

            // Assocify identity type is only nic
            this.visitorRegistrationForm.patchValue({
              visitorIdentityType: 'nic',
            })
            this.isView = true;
            this.isBlocked = false;
            this.sharedService.isUpdateProfile = false;
            return of();
          }
          else if (this.visitorSearchModel.isRegisteredToFacets == true) {
            this.isDisableNicPassport = true;
          }

          return this.publicSiteService.getById(
            this.visitorSearchModel.visitorId
          );
        }),
        switchMap((res: ResponseResult<VisitorModel>) => {
          this.visitorModel = res.data;
          this.verificationNumber =
            this.visitorModel.passportNumber ?? this.visitorModel.nicNumber!;
          this.visitorModel.countryId = this.countries.find(
            (f) => f.key == this.visitorModel.countryId
          )?.value!;
          this.visitorRegistrationForm.patchValue(this.visitorModel);
          this.getVisitorProfileImage(this.visitorModel.id);
          this.sharedService.isUpdateProfile = true;
          this.isView = true;
          this.isBlocked = false;
          return of(res.data);
        })
      )
      .subscribe({
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        },
      });
  }

  getVisitorProfileImage(visitorId: string) {
    this.isBlocked = true;
    this.publicSiteService.getVisitorDocuments(visitorId, 'attachmentTypes=profileImage')
      .subscribe({
        next: (res: ResponseResult<FileModel[]>) => {
          this.fileModel = res.data;
          this.visitorModel.imageUrl = this.fileModel[this.fileModel.length - 1].uri;
          if (this.visitorModel.imageUrl != "" || this.visitorModel.imageUrl != undefined) {
            this.isProfileImageViewOnly = true;
          }
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  countryValidator(field: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const group = control.parent;
      const country = group!.get(field);
      const countries = this.countries.map(m => m.value);
      if (countries.indexOf(country?.value) !== -1) return null;
      return country?.value != "" ? { invalidCountry: control.value } : null;
    }
  }

  createVisitor() {
    this.isFormSubmitted = true;
    this.validate();

    if (this.visitorRegistrationForm.invalid) { return; }
    if (this.fileList.length === 0) { return this.toasterService.warning('Profile pic is required') }

    this.isBlocked = true;
    this.visitorModel = Object.assign({}, this.visitorModel, this.visitorRegistrationForm.value);
    if (this.visitorModel.email === '') {
      this.visitorModel.email = null;
    }
    this.visitorModel.countryId = this.countries.find(f => f.value == this.visitorModel.countryId)?.key!;

    this.publicSiteService.registerNewVisitor(this.visitorModel)
      .subscribe({
        next: (res: ResponseResult<VisitorModel>) => {
          this.uploadImage(res.data.id);
          this.sharedService.phoneNumber = this.sharedService.isSriLankanNumber ? res.data.mobileNumber : res.data.email!;
          this.sharedService.visitorModelJson = JSON.stringify(this.visitorModel);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

  updateVisitor() {
    this.isFormSubmitted = true;
    this.validate();

    if (this.visitorRegistrationForm.invalid) { return; }

    this.isBlocked = true;
    this.updateVisitorModel = Object.assign({}, this.updateVisitorModel, this.visitorRegistrationForm.value);

    if (this.updateVisitorModel.email === '') {
      this.updateVisitorModel.email = null;
    }
    this.updateVisitorModel.countryId = this.countries.find(f => f.value == this.updateVisitorModel.countryId)?.key!;

    this.publicSiteService.updateVisitor(this.visitorModel.id, this.updateVisitorModel)
      .subscribe({
        next: () => {
          if (this.fileList.length != 0) {
            this.updateProfileImage(this.visitorModel.id);
          }
          this.sharedService.visitorModelJson = JSON.stringify(this.updateVisitorModel);
          this.sharedService.isUpdateAttachment = true;
          this.passCategoryDataSelection();
          this.isAttachment.emit(this.visitorModel.id);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }


  uploadImage(visitorId: string) {
    const formData = new FormData();
    for (let index = 0; index < this.fileList.length; index++) {
      formData.append(`files[${index}].key`, 'profileImage');
      formData.append(`files[${index}].value`, this.fileList[index]);
    }
    this.publicSiteService.uploadDocument(visitorId, formData)
      .subscribe({
        next: () => {
          this.passCategoryDataSelection();

          this.visitorVerificationModel.isAssocifyMember = this.visitorSearchModel.isAssocifyMember;
          this.visitorVerificationModel.isRegisteredToFacets = this.visitorSearchModel.isRegisteredToFacets;
          this.visitorVerificationModel.firstName = this.visitorSearchModel.firstName;
          this.visitorVerificationModel.lastName = this.visitorSearchModel.lastName;
          this.visitorVerificationModel.nicNumber = this.visitorSearchModel.nicNumber;
          this.visitorVerificationModel.passportNumber = this.visitorSearchModel.passportNumber;
          this.visitorVerificationModel.phoneNumber = this.visitorSearchModel.phoneNumber;
          this.visitorVerificationModel.email = this.visitorSearchModel.email;
          this.visitorVerificationModel.visitorId = this.visitorSearchModel.visitorId;
          this.visitorVerificationModel.visitorIdentityType = this.visitorSearchModel.visitorIdentityType;

          this.visitorVerificationModel.otpType = OTPType.NewVisitorOnlineRegistration;

          this.isOtp.emit(this.visitorVerificationModel);

          this.sharedService.isUpdateProfile = true;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

  updateProfileImage(visitorId: string) {
    const formData = new FormData();
    for (let index = 0; index < this.fileList.length; index++) {
      formData.append(`files[${index}].key`, 'profileImage');
      formData.append(`files[${index}].value`, this.fileList[index]);
    }
    this.publicSiteService.uploadDocument(visitorId, formData)
      .subscribe({
        next: () => {
          this.passCategoryDataSelection();
          this.isAttachment.emit(this.visitorModel.id);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

  passCategoryDataSelection() {
    if (this.visitorSearchModel.isAssocifyMember) {
      this.sendDataForPassCategory.emit(['AssocifyMember', 'LocalAndForeignVisitor', 'LocalBuyer', 'ForeignBuyer', 'Custom_Visitor']);
    }
    else {
      this.sendDataForPassCategory.emit(['LocalAndForeignVisitor', 'LocalBuyer', 'ForeignBuyer', 'Custom_Visitor']);
    }
  }

  setCountryValidators() {
    this.visitorRegistrationForm.controls['countryId'].setValidators([Validators.required, this.countryValidator('countryId')]);
    this.visitorRegistrationForm.controls['countryId'].updateValueAndValidity();
  }

  back() {
    this.router.navigate(['/']);
  }

  save() {
    this.isFormSubmitted = true;
    this.validate();

    if (this.visitorRegistrationForm.invalid) { return; }

    if (this.fileModel.length == 0) {
      if (this.fileList.length == 0) { return this.toasterService.warning('Profile pic is required') }
    }

    let mobile = this.visitorRegistrationForm.get('mobileNumber')?.value;
    let visitorIdentityType = this.visitorRegistrationForm.get('visitorIdentityType')?.value;
    let nicNumber = this.visitorRegistrationForm.get('nicNumber')?.value;
    let passportNumber = this.visitorRegistrationForm.get('passportNumber')?.value;

    let countryName = this.visitorRegistrationForm.get('countryId')?.value;
    let countryId = this.countries.find(s => s.value == countryName)?.key;

    let firstName = this.visitorRegistrationForm.get('firstName')?.value;
    let lastName = this.visitorRegistrationForm.get('lastName')?.value;
    let companyName = this.visitorRegistrationForm.get('companyName')?.value;
    let email = this.visitorRegistrationForm.get('email')?.value;
    let address = this.visitorRegistrationForm.get('address.address')?.value;

    let isSriLankaNumber = this.sharedService.isSriLankaNumber(mobile);

    this.sharedService.isSriLankanNumber = isSriLankaNumber;

    if (isSriLankaNumber == false && email == null) {
      this.toasterService.warning("Please enter your email");
      return;
    }

    if (this.sharedService.isUpdateOtp == false) {
      this.createVisitor();
    }
    else if (
      this.visitorSearchModel.isAssocifyMember == true && this.visitorSearchModel.isRegisteredToFacets
      == false) {
      this.createVisitor();
    } 
    else if (this.fileList.length != 0) {
      this.updateProfileImage(this.visitorModel.id);
    }
    else if (
      this.visitorModel.mobileNumber != mobile ||
      this.visitorModel.visitorIdentityType != visitorIdentityType ||
      this.visitorModel.nicNumber != nicNumber ||
      this.visitorModel.passportNumber != passportNumber ||
      this.visitorModel.firstName != firstName ||
      this.visitorModel.lastName != lastName ||
      this.visitorModel.companyName != companyName ||
      this.visitorModel.countryId != countryName ||
      this.visitorModel.address?.address != address ||
      this.visitorModel.email == email
    ) {
      this.updateVisitor();
    }
    //else {

      if (this.visitorModel.otpVerified == true && this.visitorModel.otpVerificationRequired == true) {
        this.sharedService.isUpdateProfile = true;
        this.sharedService.isUpdateAttachment = true;
        this.sharedService.visitorModelJson = JSON.stringify(this.visitorModel);
        this.passCategoryDataSelection();
        this.isAttachment.emit(this.visitorModel.id);
      }
      else if (this.visitorModel.otpVerificationRequired == true && this.visitorModel.otpVerified == false) {
        let idNumber = this.visitorModel.nicNumber != undefined ? this.visitorModel.nicNumber : this.visitorModel.passportNumber;
        this.visitorVerificationModel.nicNumber = idNumber!;
        this.sharedService.visitorModelJson = JSON.stringify(this.visitorModel);
        this.visitorVerificationModel.otpType = OTPType.NewVisitorOnlineRegistration;
        this.visitorVerificationModel.isRegisteredToFacets = true;

        this.isOtp.emit(this.visitorVerificationModel);
      }
      else if (this.visitorModel.registeredOnline == false) {
        this.passCategoryDataSelection();
        this.sharedService.visitorModelJson = JSON.stringify(this.visitorModel);
        this.isAttachment.emit(this.visitorModel.id);
      }
   // }
  }
}
