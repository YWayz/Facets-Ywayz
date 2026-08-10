import { AfterViewInit, Component, ElementRef, OnChanges, OnInit, SimpleChanges, ViewChildren, inject } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { TeamMemberService } from '../services/team-member.service';
import { TeamMemberModel } from '../models/team-member.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { EventTeamMemberService } from '../services/event-team-member.service';
import { SuperAdminPermissions, TeamMemberRegistrationPermissions } from 'src/app/core/extensions/permission-constants';
import { AuthService } from '../../auth/services/auth.service';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { UpdateTeamMemberModel } from '../models/update-team-member.model';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { Observable, fromEvent, map, merge, startWith } from 'rxjs';
import { KeyValue } from '@angular/common';
import { FileModel } from 'src/app/shared/models/file.model';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { Title } from 'src/app/core/enums/title.enum';
import { patterns } from 'src/app/core/extensions/rejex-pattern';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { ModalService } from 'src/app/core/services/modal.service';

@Component({
  selector: 'facets-team-member-profile',
  templateUrl: './team-member-profile.component.html',
  styleUrls: ['./team-member-profile.component.scss']
})
export class TeamMemberProfileComponent implements OnInit, AfterViewInit, OnChanges {

  superAdminPermissions = SuperAdminPermissions;
  teamMemberRegistrationPermissions = TeamMemberRegistrationPermissions;

  isAttachmentHidePermission = false;
  isBlocked = false;
  isFormSubmitted = false;
  isEdit = false;
  isView = false;

  id = '';
  teamMemberId = '';
  imageURL: string;
  eventId: string | null;
  pageNumber = 1;

  searchRequestModel = new SearchRequestModel(100, 1);

  teamMemberForm: FormGroup;
  updateTeamMemberModel: UpdateTeamMemberModel = new UpdateTeamMemberModel();

  teamMember = new TeamMemberModel();
  filteredOptions: Observable<KeyValue<string, string>[]>;

  fileList: File[] = [];
  nicFileList: File[] = [];
  otherFileList: File[] = [];

  nicFileNames = new Array<string>();
  nicFileModels = new Array<FileModel>();
  otherFileNames = new Array<string>();
  otherFileModels = new Array<FileModel>();

  passCategories: Array<KeyValue<string, string>>;
  countries = new Array<KeyValue<string, string>>();

  validationModel: ValidationModel = new ValidationModel();

  authService = inject(AuthService);
  lookUpService = inject(LookupsService);
  teamMemberService = inject(TeamMemberService);
  eventTeamMemberService = inject(EventTeamMemberService);
  toasterService = inject(ToasterService);
  modalService = inject(ModalService);
  router = inject(Router);

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor(public formBuilder: FormBuilder, private activatedRoute: ActivatedRoute) {
    this.eventId = localStorage.getItem(appConstant.selectedEventId)

    this.validationModel.validationMessages = {
      firstName: {
        required: 'First name is required',
      },
      lastName: {
        required: 'Last name is required',
      },
      countryId: {
        required: 'Country is required',
      },
      mobileNumber: {
        required: 'Mobile number is required',
        pattern: 'Please enter valid mobile number'
      },
      email: {
        pattern: 'Please enter valid email format'
      },
      imageURL: {
        required: 'Profile image is required',
      },
      passCategoryId: {
        required: 'Pass Category is required',
      },
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe({
      next: (params: Params) => {
        const pageNumber = params['pageNumber'];
        if (pageNumber != undefined || pageNumber != null) {
          this.pageNumber = pageNumber;
        }
      }
    });

    this.updateTeamMemberForm();
    if (!this.authService.hasPermissionAuthorization([this.superAdminPermissions.all, this.teamMemberRegistrationPermissions.viewAttachments])) {
      this.isAttachmentHidePermission = true;
    }

    this.eventId = localStorage.getItem(appConstant.selectedEventId);
    this.activatedRoute.params.subscribe((params: Params) => {
      this.id = params['id'];
      if (this.id != "" && this.id != undefined) {
        this.teamMemberId = this.id;
        this.getCountries();
        this.getPassCategories();
        this.getTeamMemberById(this.id);
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.isBlocked = true;
    if (this.teamMemberId != "") {
      this.isEdit = true;
      this.editTeamMember(this.teamMemberId);
      this.isBlocked = false;
    }
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.teamMemberForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.teamMemberForm, this.isFormSubmitted);
  }

  editTeamMember(id: string) {
    this.isEdit = true;
    this.patchUser(this.teamMember);
  }

  updateTeamMemberForm() {
    this.teamMemberForm = this.formBuilder.group({
      nicNumber: [''],
      passportNumber: [''],
      identityType: [''],
      title: [0, Title],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      countryId: ['', Validators.required],
      passCategoryId: ['', Validators.required],
      email: [null, Validators.pattern(patterns.email)],
      address: this.formBuilder.group({
        address: [''],
      }),
      mobileNumber: ['', [Validators.required, Validators.pattern(patterns.mobile)]],
      eventId: [this.eventId!],
      imageURL: [''],
      companyName: [''],
    });
  }

  patchUser(teamMemberModel: TeamMemberModel) {
    this.isBlocked = true;
    this.teamMember = teamMemberModel;
    this.imageURL = this.teamMember.imageURL;
    this.teamMemberForm.patchValue({
      id: teamMemberModel.id,
      title: teamMemberModel.title,
      identityType: teamMemberModel.identityType,
      nicNumber: teamMemberModel.nicNumber,
      passportNumber: teamMemberModel.passportNumber,
      countryId: teamMemberModel.countryId,
      countryName: teamMemberModel.countryName,
      firstName: teamMemberModel.firstName,
      lastName: teamMemberModel.lastName,
      mobileNumber: teamMemberModel.mobileNumber,
      email: teamMemberModel.email,
      address: teamMemberModel.address,
      passCategoryId: teamMemberModel.passCategoryId,
      passCategoryName: teamMemberModel.passCategoryName,
      imageURL: this.imageURL,
      companyName: teamMemberModel.companyName
    });

    this.updateTeamMemberModel.imageURL = this.imageURL;

    if (this.teamMember.countryId === null) {
      const country = this.countries.find(f => f.value == 'Sri Lanka');
      this.teamMemberForm.patchValue({
        countryId: country?.value
      })
    }
    else {
      const country = this.countries.find(f => f.key == this.teamMember.countryId);
      this.teamMemberForm.patchValue({
        countryId: country?.value
      })
    }

    this.filteredOptions = this.teamMemberForm.get('countryId')!.valueChanges.pipe(
      startWith(''),
      map(value => this._filter(value || '')),
    );

    this.getTeamMemberAttachments(this.teamMemberId);
    this.isBlocked = false;
  }

  private getTeamMemberAttachments(teamMemberId: string) {
    this.isBlocked = true;
    this.teamMemberService.getTeamMemberDocuments(teamMemberId).subscribe({
      next: (res: ResponseResult<FileModel[]>) => {
        this.nicFileModels = res.data;
        this.nicFileModels.forEach(file => {
          if (file.extenstionData?.attachmentType == 'NIC') {
            this.nicFileModels.push(file);
            this.nicFileNames.push(file.fileName);
          }
          else if (file.extenstionData?.attachmentType == 'OtherAttachment') {
            this.otherFileModels.push(file);
            this.otherFileNames.push(file.fileName);
          }
          this.isBlocked = false;
        });
      },
    });
  }

  uploadImage(id: string) {
    this.isBlocked = true;
    if (this.fileList.length == 0) {
      this.isFormSubmitted = false;
      this.isBlocked = false;
      return;
    }

    this.teamMemberService.uploadImage(id, this.fileList)
      .subscribe({
        next: (res: any) => {
          this.isFormSubmitted = false;
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  getCountries() {
    this.lookUpService.getCountries().subscribe({
      next: (result: ResponseResult<KeyValue<string, string>[]>) => {
        this.countries = result.data;
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

  getTeamMemberById(teamMemberId: string) {
    this.isBlocked = true;
    this.teamMemberService.getTeamMemberById(teamMemberId)
      .subscribe({
        next: (res: ResponseResult<TeamMemberModel>) => {
          Object.assign(this.teamMember, res.data);
          this.patchUser(this.teamMember);
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  getPassCategories() {
    this.isBlocked = true;
    this.lookUpService.getPassCategories(this.searchRequestModel, this.eventId!, '&passCategoryType=TeamMember').subscribe({
      next: (res: ResponseResult<KeyValue<string, string>[]>) => {
        this.passCategories = res.data;
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
        this.isBlocked = false;
      },
    })
  }

  cancelTeamMemberEvent(teamMemberId: string) {
    let options = {
      title: 'Cancel Team Member from Event',
      message: 'Do you want to cancel Team Member from the Event?',
    };

    this.modalService.displayDialog(ConfirmationModalComponent, options);

    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.eventTeamMemberService.cancelTeamMemberEvent(this.eventId!, teamMemberId).subscribe({
          next: () => {
            this.isBlocked = false;
            this.toasterService.success("Team Member Cancelled from event sucessfull", "Team Member Cancel from Event");
            this.router.navigate(['/admin/team-member'])
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.toasterService.error(err)
          }
        });
      }
    })
  }

  getFiles(event: File[]) {
    if (event.length > 0) {
      this.fileList = event;
    }
  }

  getNICFiles(files: File[]) {
    this.nicFileList = files;
  }

  getOtherFiles(files: File[]) {
    this.otherFileList = files;
  }

  discard() {
    this.isEdit = false;
  }

  updateTeamMember(teamMemberId: string) {
    this.isFormSubmitted = true;
    this.validate();
    if (this.teamMemberForm.invalid) { return; }

    this.isBlocked = true;

    this.updateTeamMemberModel = Object.assign(this.updateTeamMemberModel, this.teamMemberForm.value);
    this.updateTeamMemberModel.countryId = this.countries.find(f => f.value == this.updateTeamMemberModel.countryId)?.key!;

    this.teamMemberService.updateTeamMember(teamMemberId, this.updateTeamMemberModel).subscribe({
      next: () => {
        this.isEdit = false;
        this.isBlocked = false;
        this.uploadImage(this.teamMemberId);
        this.toasterService.successfullyUpdated("Team Member");
        this.router.navigate(['admin/team-member']);
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
        this.isBlocked = false;
      },
    });
  }

  navigateToTeamMeberGeneration(teamMemberId: string) {
    this.router.navigate([`admin/team-member/pass-generation/print/${teamMemberId}`])
  }

  goToTeamMembersList() {
    this.router.navigate([`admin/team-member`], { queryParams: { pageNumber: this.pageNumber } })
  }

  blackListed(teammemberId : string){
    this.teamMemberService.blaklistTeamMember(teammemberId).subscribe({
      next: (response) => {
        console.log('Success:', response);
        this.goToTeamMembersList()
      },
      error: (err) => {
        console.error('Error:', err);
      }
    });
}

removeFromBlackList(teammemberId : string){
  this.teamMemberService.RemoveblaklistTeamMember(teammemberId).subscribe({
    next: (response) => {
      console.log('Success:', response);
      this.goToTeamMembersList()
    },
    error: (err) => {
      console.error('Error:', err);
    }
  });
}





}

