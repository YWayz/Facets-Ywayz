import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChildren, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormControlName, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { patterns } from 'src/app/core/extensions/rejex-pattern';
import { Observable, fromEvent, map, merge, startWith } from 'rxjs';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { SharedService } from 'src/app/core/services/shared.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { KeyValue } from 'src/app/core/models/key-value.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { VisitorSearchModel } from '../../models/visitor-search.model';
import { VisitorsService } from '../../services/visitors.service';
import { VisitorModel } from '../../models/visitor.model';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { ActivatedRoute, Params } from '@angular/router';
import { FileModel } from 'src/app/shared/models/file.model';

@Component({
  selector: 'facets-visitor-registration-profile',
  templateUrl: './visitor-registration-profile.component.html',
  styleUrls: ['./visitor-registration-profile.component.scss']
})
export class VisitorRegistrationProfileComponent implements OnInit, AfterViewInit {

  isFormSubmitted = false;
  isBlocked = false;
  isView = false;
  isPayLater = false;
  verificationNumber = '';
  filteredOptions: Observable<KeyValue<string, string>[]>;
  countries = new Array<KeyValue<string, string>>();
  fileList: File[] = [];

  visitorRegistrationForm: FormGroup;
  visitorModel = new VisitorModel();
  fileModel = new Array<FileModel>();
  visitorSearchModel = new VisitorSearchModel();
  validationModel: ValidationModel = new ValidationModel();

  formBuilder = inject(FormBuilder);
  sharedService = inject(SharedService);
  toasterService = inject(ToasterService);
  visitorsService = inject(VisitorsService);
  lookupService = inject(LookupsService);
  activatedRoute = inject(ActivatedRoute);

  @Input() isOtp: boolean;
  @Input() visitorId: string;
  @Output() isAttachment = new EventEmitter<string>();
  @Output() isRegistrationCounter = new EventEmitter<boolean>();
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
        pattern: 'Please enter valid email format'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createVisitorRegistrationForm();
    this.activatedRoute.queryParams.subscribe({
      next: (qParam: Params) => {
        const nicPassport = qParam['nicPassport'];
        if (nicPassport != undefined) {
          this.verificationNumber = nicPassport;
          this.isPayLater = true;
          this.sharedService.isPayLater = true;
        };
      }
    })
    this.getCountries();
    if (this.sharedService.isUpdateProfile) {
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
      email: [null, Validators.pattern(patterns.email)],
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
    this.isView = true;
    this.sharedService.isUpdateProfile = false;
    this.visitorModel = new VisitorModel();
    this.visitorModel.mapImage();
    this.visitorSearchModel = visitorSearchModel;
    this.visitorRegistrationForm.get('companyName')?.setValue('');
    this.visitorRegistrationForm.get('address.address')?.setValue('');
    this.visitorRegistrationForm.get('countryId')?.setValue('Sri Lanka');
    if (this.visitorSearchModel.isRegisteredToFacets) {
      this.getVisitorProfile();
    }
    else {
      this.visitorRegistrationForm.patchValue(this.visitorSearchModel);
    }
  }
  getVisitorProfile() {
    this.isBlocked = true;
    const visitorId = this.visitorModel.id ?? this.visitorSearchModel.visitorId ?? this.visitorId;
    this.visitorsService.getById(visitorId)
      .subscribe({
        next: (res: ResponseResult<VisitorModel>) => {
          this.visitorModel = res.data;
          this.verificationNumber = this.visitorModel.passportNumber! ?? this.visitorModel.nicNumber!;
          this.visitorModel.countryId = this.countries.find(f => f.key == this.visitorModel.countryId)?.value!;
          this.visitorRegistrationForm.patchValue(this.visitorModel);
          this.getVisitorProfileImage(this.visitorModel.id);
          this.sharedService.isUpdateProfile = true;
          this.sharedService.nicPassport = this.verificationNumber;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  getVisitorProfileImage(visitorId: string) {
    this.isBlocked = true;
    this.visitorsService.getVisitorDocuments(visitorId, 'attachmentTypes=profileImage')
      .subscribe({
        next: (res: ResponseResult<FileModel[]>) => {
          this.fileModel = res.data;
          this.visitorModel.imageUrl = this.fileModel[this.fileModel.length - 1].uri;
          this.isBlocked = false;
          this.isView = true;
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
    this.visitorModel.countryId = this.countries.find(f => f.value == this.visitorModel.countryId)?.key!;
    this.visitorModel.email = this.visitorModel.email != '' ? this.visitorModel.email : null;

    let isSriLankaNumber = this.sharedService.isSriLankaNumber(this.visitorModel.mobileNumber);

    if (isSriLankaNumber == false && this.visitorModel.email == null) {
      this.toasterService.warning("Please enter your email");
      this.isBlocked = false;
      this.isFormSubmitted = false;
      return;
    }

    this.visitorsService.create(this.visitorModel)
      .subscribe({
        next: (res: ResponseResult<VisitorModel>) => {
          this.sharedService.nicPassport = this.visitorSearchModel.nicNumber ?? this.visitorSearchModel.passportNumber;
          this.uploadImage(res.data.id)
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
    this.visitorModel = Object.assign({}, this.visitorModel, this.visitorRegistrationForm.value);
    this.visitorModel.countryId = this.countries.find(f => f.value == this.visitorModel.countryId)?.key!;
    this.visitorModel.email = this.visitorModel.email != '' ? this.visitorModel.email : null;

    const visitorId = this.visitorModel.id ?? this.visitorSearchModel.visitorId;

    let isSriLankaNumber = this.sharedService.isSriLankaNumber(this.visitorModel.mobileNumber);

    if (isSriLankaNumber == false && this.visitorModel.email == null) {
      this.toasterService.warning("Please enter your email");
      this.isBlocked = false;
      this.isFormSubmitted = false;
      return;
    }

    this.visitorsService.update(visitorId, this.visitorModel).subscribe({
      next: () => {
        this.sharedService.nicPassport = this.visitorSearchModel.nicNumber ?? this.visitorSearchModel.passportNumber;
        if (this.fileList.length != 0) {
          this.uploadImage(visitorId)
        }
        else {
          this.passCategoryDataSelection();
          this.isAttachment.emit(visitorId);
          this.sharedService.isUpdateProfile = true;
          this.sharedService.isUpdateAttachment = true;
        }
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }

  uploadImage(visitorId: string) {
    this.visitorId = visitorId;
    const formData = new FormData();
    for (let index = 0; index < this.fileList.length; index++) {
      formData.append(`files[${index}].key`, 'profileImage');
      formData.append(`files[${index}].value`, this.fileList[index]);
    }
    this.visitorsService.uploadDocument(visitorId, formData)
      .subscribe({
        next: () => {
          this.passCategoryDataSelection();
          this.isAttachment.emit(this.visitorId);
          this.sharedService.isUpdateProfile = true;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

  passCategoryDataSelection() {
    if (this.visitorSearchModel.isAssocifyMember ?? this.visitorModel.isAssocifyMember) {
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
    this.isView = false;
  }

  save() {
    if (!this.sharedService.isUpdateProfile && !this.visitorSearchModel.isRegisteredToFacets) {
      this.createVisitor();
    }
    else {
      this.updateVisitor();
    }
  }
}
