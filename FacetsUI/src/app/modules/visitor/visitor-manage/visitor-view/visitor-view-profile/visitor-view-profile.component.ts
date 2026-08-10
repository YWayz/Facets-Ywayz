import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChildren, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormControlName, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { VisitorModel } from '../../../models/visitor.model';
import { VisitorsService } from '../../../services/visitors.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ActivatedRoute, Params } from '@angular/router';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { FileModel } from 'src/app/shared/models/file.model';
import { patterns } from 'src/app/core/extensions/rejex-pattern';
import { KeyValue } from '@angular/common';
import { Observable, fromEvent, merge, startWith, map } from 'rxjs';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { ModalService } from 'src/app/core/services/modal.service';
import { VisitorCancelRegistrationComponent } from './visitor-cancel-registration/visitor-cancel-registration.component';
import { VisitorPassRegistrationModel } from '../../../models/visitor-pass-registration.model';
import { VisitorRegistrationService } from '../../../services/visitor-registration.service';
import { VisitorBlacklistComponent } from './visitor-blacklist/visitor-blacklist.component';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { VisitorPermissions, VisitorRegistrationPermissions } from 'src/app/core/extensions/permission-constants';

@Component({
  selector: 'facets-visitor-view-profile',
  templateUrl: './visitor-view-profile.component.html',
  styleUrls: ['./visitor-view-profile.component.scss']
})
export class VisitorViewProfileComponent implements OnInit, AfterViewInit {

  isEdit = false;
  isFormSubmitted = false;
  isBlocked = false;
  visitorId: string;
  visitorRegistrationId: string;
  filteredOptions: Observable<KeyValue<string, string>[]>;
  countries = new Array<KeyValue<string, string>>();
  visitorAttendanceSchedule: any[] = [];
  fileList: File[] = [];
  visitorPermissions = VisitorPermissions;
  visitorRegistrationPermissions = VisitorRegistrationPermissions;

  visitorPassRegistration = new VisitorPassRegistrationModel();
  visitorModel = new VisitorModel();
  fileModel: FileModel[];
  visitorForm: FormGroup;
  validationModel: ValidationModel = new ValidationModel();

  visitorsService = inject(VisitorsService);
  toasterService = inject(ToasterService);
  activatedRoute = inject(ActivatedRoute);
  formBuilder = inject(FormBuilder);
  modalService = inject(ModalService);
  lookupService = inject(LookupsService);
  visitorRegistrationService = inject(VisitorRegistrationService);

  @Output() sendFullName = new EventEmitter<string>();
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
      nicPassportNumber: {
        required: 'NIC/Passport is required'
      },
      email: {
        pattern: 'Please enter valid email format'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }


  ngOnInit(): void {
    this.createVisitorProfileForm();
    this.getCountries();
    this.activatedRoute.params.subscribe((params: Params) => {
      this.visitorId = params['visitorId']
      this.visitorRegistrationId = params['registrationId'];
      if (this.visitorId != null || this.visitorId != undefined || this.visitorId != '') {
        this.getVisitorProfile(this.visitorId);
      }
    });
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.visitorForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.visitorForm, this.isFormSubmitted);
  }

  createVisitorProfileForm() {
    this.visitorForm = this.formBuilder.group({
      visitorIdentityType: ['', Validators.required],
      nicPassportNumber: ['', Validators.required],
      countryId: [''],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      mobileNumber: ['', [Validators.required, Validators.pattern(patterns.mobile)]],
      companyName: [''],
      email: ['', Validators.pattern(patterns.email)],
      address: this.formBuilder.group({
        address: ['']
      })
    });

    this.setCountryValidators();
  }

  setCountryValidators() {
    this.visitorForm.controls['countryId'].setValidators([Validators.required, this.countryValidator('countryId')]);
    this.visitorForm.controls['countryId'].updateValueAndValidity();
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

  getCountries() {
    this.lookupService.getCountries().subscribe({
      next: (result: ResponseResult<KeyValue<string, string>[]>) => {
        this.countries = result.data;
        this.filteredOptions = this.visitorForm.get('countryId')!.valueChanges.pipe(
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

  getVisitorProfile(visitorId: string) {
    this.isBlocked = true;
    this.visitorsService.getById(visitorId)
      .subscribe({
        next: (res: ResponseResult<VisitorModel>) => {
          this.isBlocked = false;
          this.isEdit = false;
          this.visitorModel = res.data;
          this.sendFullName.emit(this.visitorModel.firstName + ' ' + this.visitorModel.lastName);
          this.visitorModel.countryId = this.countries.find(f => f.key == this.visitorModel.countryId)?.value!;

          if (this.visitorModel.visitorIdentityType == 'NIC') this.visitorForm.get('nicPassportNumber')?.setValue(this.visitorModel.nicNumber);
          else this.visitorForm.get('nicPassportNumber')?.setValue(this.visitorModel.passportNumber);

          this.visitorForm.patchValue(this.visitorModel);
          this.getVisitorProfileImage(this.visitorModel.id);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  getVisitorPassRegistration(visitorId: string) {
    this.isBlocked = true;
    this.visitorRegistrationService.getVisitorPassRegistration(visitorId, this.visitorRegistrationId)
      .subscribe({
        next: (res: ResponseResult<VisitorPassRegistrationModel>) => {
          this.isBlocked = false;
          this.visitorPassRegistration = res.data;
          this.cancelVisitorRegistration();
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  blackListed() {
    this.modalService.displayDialog(VisitorBlacklistComponent, {
      data: {
        fullName: this.visitorModel.firstName + ' ' + this.visitorModel.lastName,
        visitorId: this.visitorId,
      }
    });
    this.modalService.confirmed().subscribe(() => this.getVisitorProfile(this.visitorId));
  }

  removeFromBlackList() {
    let options = {
      title: 'Confirmation Message',
      message: 'Are you sure you want to remove the visitor from blacklist ?',
    };
    this.modalService.displayDialog(ConfirmationModalComponent, options);
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.visitorsService.removeFromBlackList(this.visitorId).subscribe({
          next: () => {
            this.isBlocked = false;
            this.toasterService.successfullyDeleted("Removed visitor from blacklist successfully");
            this.getVisitorProfile(this.visitorId);
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.toasterService.error(err);
          }
        });
      }
    });
  }

  cancelVisitorRegistration() {
    this.visitorAttendanceSchedule = [];
    const registeredEventDates = this.visitorPassRegistration.attendanceSchedules.map(m => m.registeredEventDate);

    const toFindDuplicates = (arr: any) => registeredEventDates.filter((item: any, index: any) => arr.indexOf(item) !== index)
    let duplicateElements = toFindDuplicates(registeredEventDates);

    duplicateElements = [...new Set(duplicateElements)]
    if(duplicateElements.length > 0) {
      duplicateElements.forEach(element => {
        const isCancelled = this.visitorPassRegistration.attendanceSchedules.filter(f => f.registeredEventDate == element).every(e => e.cancelled);

        if(isCancelled) {
          const visitorAttendanceSchedule = this.visitorPassRegistration.attendanceSchedules.filter(f => f.registeredEventDate == element && f.cancelled);
          const selectedVisitorAttendanceSchedule = visitorAttendanceSchedule[visitorAttendanceSchedule.length - 1]
          this.visitorAttendanceSchedule.push(selectedVisitorAttendanceSchedule)
        }
        else {
          const visitorAttendanceSchedule = this.visitorPassRegistration.attendanceSchedules.filter(f => f.registeredEventDate == element && !f.cancelled);
          const selectedVisitorAttendanceSchedule = visitorAttendanceSchedule[visitorAttendanceSchedule.length - 1]
          this.visitorAttendanceSchedule.push(selectedVisitorAttendanceSchedule)
        }
      });

      const visitorAttendanceSchedule = this.visitorPassRegistration.attendanceSchedules.filter(item => !this.visitorAttendanceSchedule.map(m => m.registeredEventDate).includes(item.registeredEventDate));
      this.visitorAttendanceSchedule.push(...visitorAttendanceSchedule)
    }
    else {
      this.visitorAttendanceSchedule.push(...this.visitorPassRegistration.attendanceSchedules)
    }

    this.modalService.displayDialog(VisitorCancelRegistrationComponent, {
      data: {
        visitorRegistrationId: this.visitorRegistrationId,
        attendanceSchedules: this.visitorAttendanceSchedule
      }
    });
    this.modalService.confirmed().subscribe(() => this.getVisitorProfile(this.visitorId));
  }

  getFiles(event: File[]) {
    if (event.length > 0) {
      this.fileList = event;      
    }
  }

  getVisitorProfileImage(visitorId: string) {
    this.isBlocked = true;
    this.visitorsService.getVisitorDocuments(visitorId, 'attachmentTypes=profileImage')
      .subscribe({
        next: (res: ResponseResult<FileModel[]>) => {
          this.fileModel = res.data;
          this.visitorModel.imageUrl = this.fileModel[this.fileModel.length - 1].uri;
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  uploadImage(visitorId: string) {
    const formData = new FormData();
    for (let index = 0; index < this.fileList.length; index++) {
      formData.append(`files[${index}].key`, 'profileImage');
      formData.append(`files[${index}].value`, this.fileList[index]);
    }
    this.visitorsService.uploadDocument(visitorId, formData)
      .subscribe({
        next: () => {
          this.isBlocked = false;
          this.getVisitorProfile(this.visitorId);
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

    if (this.visitorForm.invalid) { return; }

    this.isBlocked = true;
    this.visitorModel = Object.assign({}, this.visitorModel, this.visitorForm.value);
    this.visitorModel.countryId = this.countries.find(f => f.value == this.visitorModel.countryId)?.key!;

    if (this.visitorModel.visitorIdentityType == 'NIC') this.visitorModel.nicNumber = this.visitorForm.value.nicPassportNumber
    else this.visitorModel.passportNumber = this.visitorForm.value.nicPassportNumber

    this.visitorsService.update(this.visitorModel.id, this.visitorModel).subscribe({
      next: () => {
        if (this.fileList.length != 0) {
          this.uploadImage(this.visitorModel.id);
        }
        else {
          this.getVisitorProfile(this.visitorId);
        }
        this.toasterService.successfullyUpdated("Visitor profile");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }

  cancelRegistration() {
    this.getVisitorPassRegistration(this.visitorId);
  }
}
