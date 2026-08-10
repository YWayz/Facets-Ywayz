import { AfterViewInit, Component, ElementRef, Inject, OnInit, ViewChildren, inject } from '@angular/core';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { PavilionModel } from 'src/app/modules/event/models/pavilion.model';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { Observable, fromEvent, merge, of, switchMap } from 'rxjs';
import { EventDateSelectionModel } from 'src/app/modules/report/models/event-date-selection.model';
import { EventService } from 'src/app/modules/event/services/event.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { EventDetailModel } from 'src/app/modules/event/models/event-detail.model';
import { checkTimeOverLap, convertOnlyDate } from 'src/app/core/extensions/helpers';
import { CreatePavilionSessionModel } from 'src/app/modules/event/models/create-pavilion-session.model';
import { CreatePavilionModel } from 'src/app/modules/event/models/create-pavilion.model';
import { PavilionService } from 'src/app/modules/event/services/pavilion.service';
import { PavilionSessionService } from 'src/app/modules/event/services/pavilion-session.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { PavilionSessionModel } from 'src/app/modules/event/models/pavilion-session.model';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { PavilionPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { UpdatePavilionSessionModel } from 'src/app/modules/event/models/update-pavilion-session.model';
import { ModalService } from 'src/app/core/services/modal.service';
import * as moment from 'moment';

@Component({
  selector: 'facets-pavilion-create',
  templateUrl: './pavilion-create.component.html',
  styleUrls: ['./pavilion-create.component.scss']
})
export class PavilionCreateComponent implements OnInit, AfterViewInit {
  isFormSubmitted = false;
  isBlocked = false;
  isPavilionCreated = false;
  isAllowedToEdit = false;
  isDisabled = false;
  disableAddSessionButton = false;

  eventId = '';
  pavilionId = '';

  pavilionForm: FormGroup;
  pavilionModel: PavilionModel;
  createPavilionModel = new CreatePavilionModel();
  createSessionModel: CreatePavilionSessionModel;
  createPavilionSessionModels = new Array<CreatePavilionSessionModel>();

  pavilionSessionId = '';

  superAdminPermissions = SuperAdminPermissions;
  pavilionPermissions = PavilionPermissions;

  createPavilionSessionModel: CreatePavilionSessionModel;
  updatePavilionSessionModel: UpdatePavilionSessionModel;
  pavilionSessionModels = new Array<PavilionSessionModel>();
  filteredPavilionSessionModels = new Array<PavilionSessionModel>();
  pavilionSessionModel: PavilionSessionModel;

  validationModel: ValidationModel = new ValidationModel();

  selectedEventDate: string;
  eventDates = new Array<EventDateSelectionModel>();

  formBuilder = inject(FormBuilder);
  eventService = inject(EventService);
  pavilionService = inject(PavilionService);
  pavilionSessionService = inject(PavilionSessionService);
  modalService = inject(ModalService);
  toasterService = inject(ToasterService);

  dialogRef = inject(MatDialogRef<PavilionCreateComponent>);

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor(@Inject(MAT_DIALOG_DATA) public dialogData: { data: { id: string, isEdit: boolean, eventId: string } }) {
    this.validationModel.validationMessages = {
      name: {
        required: 'Pavilion name is required',
      },
      allowedVisitorCount: {
        required: 'Allowed visitor count is required',
        pattern: 'Allowed visitor count should be more than 0'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createPavilionForm();
    if (this.dialogData.data.eventId != '' || this.dialogData.data.eventId != undefined) {
      this.eventId = this.dialogData.data.eventId;
    }

    this.getEventDatesByEventId(this.eventId);

    if (this.dialogData.data.isEdit) {
      this.getPavilion();
      this.getPavilionSessions();
    }
  }

  getPavilion() {
    this.isBlocked = true;
    this.pavilionService.getPavilionById(this.dialogData.data.eventId, this.dialogData.data.id).subscribe({
      next: (res: ResponseResult<PavilionModel>) => {
        this.pavilionModel = res.data;
        this.isPavilionCreated = true;

        this.pavilionForm.patchValue(this.pavilionModel);
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
        this.isBlocked = false;
      },
    })
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.pavilionForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.pavilionForm, this.isFormSubmitted);
  }

  createPavilionForm() {
    this.pavilionForm = this.formBuilder.group({
      name: ['', Validators.required],
      allowedVisitorCount: [0, Validators.required],
      startTime: [new Date()],
      endTime: [new Date()]
    });
  }

  getEventDatesByEventId(eventId: string) {
    this.isBlocked = true;
    this.eventService.getById(eventId).subscribe({
      next: (res: ResponseResult<EventDetailModel>) => {
        res.data.eventDates.map(p => this.eventDates.push({ key: p.key, value: p.value, isSelected: false }));

        const today = convertOnlyDate(new Date());
        const eventDateSelectionModel = this.eventDates.find(f => convertOnlyDate(f.value) == today)
        if (eventDateSelectionModel != undefined) {
          const eventDateSelection = this.eventDates.filter(f => convertOnlyDate(f.value) == today)[0];
          eventDateSelection.isSelected = true;
        }

        this.selectedEventDate = convertOnlyDate(this.eventDates.find(s => s.isSelected)?.value!);

        this.isBlocked = false;
      },
    })
  }

  createPavilion() {
    const name = this.pavilionForm.get('name')?.value;
    if (name == undefined) {
      return this.toasterService.warning('Name is required');
    }

    this.isPavilionCreated = true;
  }

  update() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.pavilionForm.invalid) { return; }

    this.createPavilionModel.initializeValue(this.pavilionForm.get('name')?.value);

    this.isBlocked = true;

    this.pavilionService
      .updatePavilion(
        this.eventId,
        this.dialogData.data.id == undefined ? this.pavilionId : this.dialogData.data.id,
        this.createPavilionModel)
      .subscribe({
        next: () => {
          this.toasterService.successfullyUpdated("Pavilion");
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.toasterService.error(err);
          this.isBlocked = false;
        },
      })
  }

  discard() {
    this.dialogRef.close();
  }

  getPavilionSessions() {
    this.isBlocked = true;
    this.pavilionSessionService
      .getAllPavilionSessions(
        this.dialogData.data.eventId,
        this.dialogData.data.id == undefined ? this.pavilionId : this.dialogData.data.id)
      .subscribe({
        next: (res: ResponseResult<PavilionSessionModel[]>) => {
          this.pavilionSessionModels = res.data;

          let eventDateId = this.eventDates.find(d => d.isSelected)?.key;

          this.filteredPavilionSessionModels = this.pavilionSessionModels.filter(s => s.eventDateId == eventDateId);

          this.filteredPavilionSessionModels.forEach(pavilionSessionModel => {
            pavilionSessionModel.eventDate = convertOnlyDate(this.eventDates.find(s => s.key == eventDateId)?.value!);
          });

          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.toasterService.error(err);
          this.isBlocked = false;
        },
      });
  }

  selectEventDate(eventDateId: string) {
    this.eventDates = this.eventDates.map(eventDate => ({
      ...eventDate,
      isSelected: eventDate.key === eventDateId
    }));

    if (this.dialogData.data.isEdit) {
      this.filteredPavilionSessionModels = this.pavilionSessionModels.filter(s => s.eventDateId == eventDateId);

      this.filteredPavilionSessionModels.forEach(pavilionSessionModel => {
        pavilionSessionModel.eventDate = convertOnlyDate(this.eventDates.find(s => s.key == eventDateId)?.value!);
      });
    }
    else {
      this.filteredPavilionSessionModels = this.pavilionSessionModels.filter(s => s.eventDateId == eventDateId);

      this.filteredPavilionSessionModels.forEach(pavilionSessionModel => {
        pavilionSessionModel.eventDate = convertOnlyDate(this.eventDates.find(s => s.key == eventDateId)?.value!);
      });
    }

    // this.createPavilionForm();
    this.isAllowedToEdit = false;
  }

  addSession() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.pavilionForm.invalid) { return; }

    this.isBlocked = true;
    this.disableAddSessionButton = true;

    this.createPavilionSessionModel = Object.assign({}, this.createPavilionSessionModel, this.pavilionForm.value);

    this.createPavilionSessionModel.eventDateId = this.eventDates.find(s => s.isSelected)?.key!;
    this.createPavilionSessionModel.eventDate = convertOnlyDate(this.eventDates.find(s => s.isSelected)?.value!);

    this.SetSessionTimes();

    if (this.createPavilionSessionModel.eventDate == '' || this.createPavilionSessionModel.eventDate == undefined) {
      this.disableAddSessionButton = false;
      this.isBlocked = false;
      return this.toasterService.warning("Please select an event date", "Event date is required");
    }

    const visitorCount = this.pavilionForm.get('allowedVisitorCount')?.value;
    if (visitorCount == 0) {
      this.isBlocked = false;
      this.disableAddSessionButton = false;
      return this.toasterService.warning('Please enter count more than 0');
    }

    const isValidTime = moment(this.createPavilionSessionModel.startTime).isBefore(moment(this.createPavilionSessionModel.endTime));

    if (isValidTime === false) {

      this.toasterService.warning('End time should be passed Start time');
      this.isBlocked = false;
      this.disableAddSessionButton = false;
      return;
    }

    const isSameTime = moment(this.createPavilionSessionModel.startTime).isSame(moment(this.createPavilionSessionModel.endTime));

    if (isSameTime) {

      this.toasterService.warning('Start time and End time cannot be the same');
      this.isBlocked = false;
      this.disableAddSessionButton = false;
      return;
    }

    const hasOverLapped = checkTimeOverLap(this.pavilionSessionModels, this.createPavilionSessionModel);

    if (hasOverLapped) {
      this.toasterService.warning('Pavilion session time is already taken', 'Overlap');
      this.isBlocked = false;
      this.disableAddSessionButton = false;
      return;
    }

    if (this.pavilionModel == null || this.pavilionModel.id == undefined) {
      this.isFormSubmitted = true;
      this.validate();
      if (this.pavilionForm.invalid) { return; }

      this.createPavilionModel.initializeValue(this.pavilionForm.get('name')?.value);

      this.pavilionService.createPavilion(this.eventId, this.createPavilionModel)
        .pipe(switchMap((res: ResponseResult<PavilionModel>): Observable<ResponseResult<PavilionSessionModel>> => {
          this.pavilionForm.get('name')?.disable();
          this.pavilionModel = res.data;
          this.pavilionId = this.pavilionModel.id;
          this.toasterService.successfullyCreated("Pavilion");
          this.createPavilionSession();
          return of();
        }))
        .subscribe({
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.isFormSubmitted = false;
            this.disableAddSessionButton = false;
            this.toasterService.error(err);
          },
        })

    }
    else {
      this.createPavilionSession();
    }

  }

  private SetSessionTimes() {
    this.createPavilionSessionModel.startTime = new Date(this.createPavilionSessionModel.startTime).toISOString();
    this.createPavilionSessionModel.endTime = new Date(this.createPavilionSessionModel.endTime).toISOString();
  }

  createPavilionSession() {
    this.pavilionSessionService.createPavilionSession(this.eventId, this.pavilionId || this.dialogData.data.id, this.createPavilionSessionModel)
      .subscribe({
        next: (res: ResponseResult<PavilionSessionModel>) => {
          this.pavilionSessionModel = res.data;

          this.pavilionSessionModel.eventDate = convertOnlyDate(this.eventDates.find(s => s.key == this.pavilionSessionModel.eventDateId)?.value!);
          this.pavilionSessionModels.push(this.pavilionSessionModel);

          let eventDateId = this.eventDates.find(s => s.isSelected)?.key;

          this.filteredPavilionSessionModels = this.pavilionSessionModels.filter(s => s.eventDateId == eventDateId);

          this.isFormSubmitted = false;
          this.toasterService.successfullyCreated("Pavilion Session");
          this.disableAddSessionButton = false;
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.disableAddSessionButton = false;
          this.isFormSubmitted = false;
          this.isBlocked = false;
          this.toasterService.error(err);
        },
      })
  }

  isAllowToEdit(pavilionSessionId: string) {
    this.isBlocked = true;

    this.isAllowedToEdit = true;
    this.pavilionSessionId = pavilionSessionId;

    this.pavilionSessionModel = this.pavilionSessionModels.find(s => s.id == pavilionSessionId)!;
    this.pavilionForm.patchValue(this.pavilionSessionModel);

    if (this.pavilionSessionModel.isVisitorRegistered == true) {
      this.isDisabled = true;
    }

    this.isBlocked = false;
  }

  edit() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.pavilionForm.invalid) { return; }

    this.isBlocked = true;
    this.disableAddSessionButton = true;

    this.updatePavilionSessionModel = Object.assign({}, this.updatePavilionSessionModel, this.pavilionForm.value);
    this.updatePavilionSessionModel.eventDateId = this.eventDates.find(s => s.isSelected)?.key!;

    this.pavilionSessionService
      .updatePavilion(
        this.dialogData.data.eventId,
        this.dialogData.data.id == undefined ? this.pavilionId : this.dialogData.data.id,
        this.pavilionSessionId, this.updatePavilionSessionModel)
      .subscribe({
        next: () => {
          this.isFormSubmitted = false;
          this.isAllowedToEdit = false;
          this.isDisabled = false;
          this.disableAddSessionButton = false;
          this.pavilionSessionId = '';
          this.pavilionForm.get('startTime')?.reset(new Date());
          this.pavilionForm.get('endTime')?.reset(new Date());
          this.pavilionForm.get('allowedVisitorCount')?.reset(0);
          this.getPavilionSessions();
          this.toasterService.successfullyUpdated("Pavilion Session");
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isFormSubmitted = false;
          this.isDisabled = false;
          this.disableAddSessionButton = false;
          this.toasterService.error(err);
          this.isBlocked = false;
        },
      })
  }

  cancelEdit() {
    this.isBlocked = true;
    this.isDisabled = false;
    this.isAllowedToEdit = false;
    this.pavilionSessionId = '';
    this.pavilionForm.get('startTime')?.reset(new Date());
    this.pavilionForm.get('endTime')?.reset(new Date());
    this.pavilionForm.get('allowedVisitorCount')?.reset(0);
    this.isBlocked = false;
  }

  remove(pavilionSessionId: string) {
    let options = {
      title: 'Delete Pavilion Session',
      message: 'Are you sure you want to delete the pavilion session?'
    }

    if (this.pavilionSessionModels.length == 1) {
      return this.toasterService.warning('The Pavilion should contain atleast 1 session', 'Cannot delete session');
    }

    this.modalService.displayDialog(ConfirmationModalComponent, options);
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.pavilionSessionService
          .deletePavilionSession(
            this.eventId,
            this.pavilionId ? this.pavilionId : this.dialogData.data.id,
            pavilionSessionId)
          .subscribe({
            next: () => {
              this.isBlocked = false;
              this.getPavilionSessions();
              this.toasterService.successfullyDeleted('Pavilion Session');
            },
            error: (err: ErrorResponse) => {
              this.isBlocked = false;
              this.toasterService.error(err);
            },
          })
      }
    })
  }

}
