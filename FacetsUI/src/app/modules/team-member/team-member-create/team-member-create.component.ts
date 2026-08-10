import { AfterViewInit, Component, ElementRef, EventEmitter, OnInit, Output, ViewChildren, inject } from '@angular/core';
import { CreateTeamMemberModel } from '../models/create-team-member.model';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ModalService } from 'src/app/core/services/modal.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { Title } from 'src/app/core/enums/title.enum';
import { TeamMemberModel } from '../models/team-member.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { TeamMemberService } from '../services/team-member.service';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { KeyValue } from 'src/app/core/models/key-value.model';
import { SharedService } from 'src/app/core/services/shared.service';
import { TeamMemberSearchModel } from '../models/team-member-search.model';
import { patterns } from 'src/app/core/extensions/rejex-pattern';
import { Observable, fromEvent, map, merge, startWith } from 'rxjs';
import { AssignTeamMemberToEventModel } from '../models/assign-team-member-to-events.model';
import { EventTeamMemberService } from '../services/event-team-member.service';
import { TeamMemberDocumentModel } from '../models/team-member-document.model';
import { SuperAdminPermissions, TeamMemberRegistrationPermissions } from 'src/app/core/extensions/permission-constants';
import { UpdateTeamMemberModel } from '../models/update-team-member.model';
import { FileModel } from 'src/app/shared/models/file.model';

@Component({
  selector: 'facets-team-member-create',
  templateUrl: './team-member-create.component.html',
  styleUrls: ['./team-member-create.component.scss']
})
export class TeamMemberCreateComponent implements OnInit, AfterViewInit {

  superAdminPermissions = SuperAdminPermissions;
  teamMemberRegistrationPermissions = TeamMemberRegistrationPermissions;

  isFormSubmitted = false;
  isBlocked = false;
  isView = false;
  isUpdate = false;

  eventId: string | null;
  imageURL: string = '../../assets/images/no-image.png';
  imagePath: string;
  teamMemberId: string | null;
  nicFileDatas = new Array<any>();
  nicFileModels = new Array<FileModel>();
  otherFileDatas = new Array<any>();
  otherFileModels = new Array<FileModel>();

  teamMemberForm: FormGroup;
  teamMemberSearchModel: TeamMemberSearchModel;
  createTeamMemberModel: CreateTeamMemberModel = new CreateTeamMemberModel();
  updateTeamMemberModel: UpdateTeamMemberModel = new UpdateTeamMemberModel();
  assignTeamMemberToEventModel: AssignTeamMemberToEventModel = new AssignTeamMemberToEventModel();
  teamMemberModel: TeamMemberModel = new TeamMemberModel();
  teamMemberDocumentModel = new Array<TeamMemberDocumentModel>();

  searchRequestModel = new SearchRequestModel(100, 1);
  passCategories: Array<KeyValue<string, string>>;
  countries = new Array<KeyValue<string, string>>();
  filteredOptions: Observable<KeyValue<string, string>[]>;

  fileList: File[] = [];
  nicFileList: File[] = [];
  otherFileList: File[] = [];

  validationModel: ValidationModel = new ValidationModel();

  router = inject(Router);
  teamMemberService = inject(TeamMemberService);
  eventTeamMemberService = inject(EventTeamMemberService);
  lookUpService = inject(LookupsService);
  toasterService = inject(ToasterService);
  modalService = inject(ModalService);
  sharedService = inject(SharedService);

  @Output() isAttachment = new EventEmitter<boolean>();
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
      passCategoryId: {
        required: 'Pass Category is required',
      },
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createTeamMemberForm();
    this.getPassCategories();
    this.getCountries();
  }

  createTeamMemberForm() {
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

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.teamMemberForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.teamMemberForm, this.isFormSubmitted);
  }

  uploadImage(id: string) {
    this.teamMemberService.uploadImage(id, this.fileList)
      .subscribe({
        next: (res: any) => {
          this.isFormSubmitted = false;
          this.isBlocked = false;
          this.router.navigate(['admin/team-member']);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
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

  uploadNicImage(teamMemberId: string) {
    this.isBlocked = true;
    const formData = new FormData();
    this.InitializeNicAttachment(formData);

    this.InitializeOtherAttachment(formData);

    this.teamMemberService.uploadDocument(teamMemberId, formData)
      .subscribe({
        next: () => {
          this.isAttachment.emit(true);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      });
  }

  private InitializeOtherAttachment(formData: FormData) {
    for (let index = 0; index < this.otherFileList.length; index++) {
      formData.append(`files[${this.nicFileList.length + index}].key`, 'otherAttachment');
      formData.append(`files[${this.nicFileList.length + index}].value`, this.otherFileList[index]);
    }
  }

  private InitializeNicAttachment(formData: FormData) {
    for (let index = 0; index < this.nicFileList.length; index++) {
      formData.append(`files[${index}].key`, 'nic');
      formData.append(`files[${index}].value`, this.nicFileList[index]);
    }
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
    this.createTeamMemberForm();
    this.router.navigate(['admin/team-member'])
  }

  patchTeamMember(teamMemberSearchModel: TeamMemberSearchModel) {
    this.teamMemberSearchModel = teamMemberSearchModel;
    this.isView = true;
    this.imageURL = teamMemberSearchModel.imageURL;
    this.createTeamMemberModel.imageURL = this.imageURL;
    this.teamMemberForm.get('address.address')?.setValue('');
    
    this.teamMemberDocumentModel = [];
    this.nicFileModels = [];
    
    this.nicFileDatas = []
    this.otherFileDatas = []

    this.nicFileList = []
    this.otherFileList = []

    this.teamMemberForm.patchValue(teamMemberSearchModel);
    

    if (teamMemberSearchModel.teamMemberId == null || teamMemberSearchModel.teamMemberId == '') {
      this.isUpdate = false;
    }
    else {
      this.isUpdate = true;
    }

    let passCategory = this.passCategories.find(s => s.key == this.teamMemberForm.get('passCategoryId')?.value);
    if (passCategory == null) {
      this.teamMemberForm.get('passCategoryId')?.setValue('');
      this.getPassCategories();
    }

    if (teamMemberSearchModel.countryId === null) {
      const country = this.countries.find(f => f.value == 'Sri Lanka');
      this.teamMemberForm.patchValue({
        countryId: country?.value
      })
    }
    else {
      const country = this.countries.find(f => f.key == teamMemberSearchModel.countryId);
      this.teamMemberForm.patchValue({
        countryId: country?.value
      })
    }

    this.filteredOptions = this.teamMemberForm.get('countryId')!.valueChanges.pipe(
      startWith(''),
      map(value => this._filter(value || '')),
    );

    if (teamMemberSearchModel.teamMemberId !== null) {
      this.teamMemberId = teamMemberSearchModel.teamMemberId;
      this.getTeamMemberAttachments(teamMemberSearchModel);
      this.isUpdate = true;
    }
  }

  private getTeamMemberAttachments(teamMemberSearchModel: TeamMemberSearchModel) {
    this.teamMemberService.getTeamMemberDocuments(teamMemberSearchModel.teamMemberId).subscribe({
      next: (res: ResponseResult<FileModel[]>) => {
        this.nicFileModels = res.data;
        this.nicFileModels.forEach(file => {
          if (file.extenstionData?.attachmentType == 'NIC') {
            this.nicFileModels.push(file);
            this.nicFileDatas.push({ name: file.fileName, img: file.uri });
          }
          else if (file.extenstionData?.attachmentType == 'OtherAttachment') {
            this.otherFileModels.push(file);
            this.otherFileDatas.push({ name: file.fileName, img: file.uri });
          }
        });
      },
    });
  }

  updateRegisteredTeamMember(teamMemberId: string) {
    this.isFormSubmitted = true;
    this.validate();
    if (this.teamMemberForm.invalid) { return; }

    let passCategory = this.passCategories.find(s => s.key == this.teamMemberForm.get('passCategoryId')?.value);

    this.isBlocked = true;
    
    if (passCategory == null) {
      this.toasterService.warning("This Pass Category is not available for this event", "Invalid Pass Category");
      this.isBlocked = false;
      this.isFormSubmitted = false;
      return;
    }

    this.updateTeamMemberModel = Object.assign(this.updateTeamMemberModel, this.teamMemberForm.value);
    this.updateTeamMemberModel.countryId = this.countries.find(f => f.value == this.updateTeamMemberModel.countryId)?.key!;

    this.teamMemberService.updateTeamMember(teamMemberId, this.updateTeamMemberModel)
      .subscribe({
        next: () => {
          if (this.fileList.length > 0) {
            this.uploadImage(teamMemberId);
          }
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.isUpdate = false;
          this.router.navigate(['admin/team-member']);
          this.toasterService.successfullyUpdated("Team Member");
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  assignToEvent(teamMemberId: string) {

    this.isBlocked = true;

    let passCategory = this.passCategories.find(s => s.key == this.teamMemberForm.get('passCategoryId')?.value);

    if (passCategory == null) {
      this.toasterService.warning("This Pass Category is not available for this event", "Invalid Pass Category");
      this.isBlocked = false;
      this.isFormSubmitted = false;
      return;
    }

    this.assignTeamMemberToEventModel.initialize(teamMemberId, this.teamMemberForm.get('passCategoryId')?.value, this.eventId!)

    this.eventTeamMemberService.assignEventAndPassCategory(this.assignTeamMemberToEventModel)
      .subscribe({
        next: () => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.isUpdate = false;
          this.toasterService.success("Team Member Assigned to event successfully", "Team Member Assign to Event");
          this.router.navigate(['admin/team-member']);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  assignEventAndPassCategoryToTeamMember(teamMemberId: string) {
    this.isFormSubmitted = true;
    this.validate();
    if (this.teamMemberForm.invalid) { return; }

    this.assignTeamMemberToEventModel.initialize(teamMemberId, this.teamMemberForm.get('passCategoryId')?.value, this.eventId!)

    this.eventTeamMemberService.assignEventAndPassCategory(this.assignTeamMemberToEventModel)
      .subscribe({
        next: () => {
          this.isFormSubmitted = false;
          this.isUpdate = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  create() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.teamMemberForm.invalid) { return; }

    if (this.fileList.length === 0) { return this.toasterService.warning('Profile pic is required') }

    this.isBlocked = true;
    this.createTeamMemberModel = Object.assign(this.createTeamMemberModel, this.teamMemberForm.value);
    this.createTeamMemberModel.countryId = this.countries.find(f => f.value == this.createTeamMemberModel.countryId)?.key!;
    this.teamMemberService.register(this.createTeamMemberModel)
      .subscribe({
        next: (res: ResponseResult<TeamMemberModel>) => {
          this.assignEventAndPassCategoryToTeamMember(res.data.id);
          this.toasterService.success("Team member registered successfully", "Team Member Register");
          this.uploadNicImage(res.data.id);
          this.uploadImage(res.data.id);
          this.isFormSubmitted = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }
}


