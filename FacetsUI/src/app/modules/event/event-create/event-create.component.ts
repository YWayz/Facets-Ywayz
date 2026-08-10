import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, ViewChildren, inject } from '@angular/core';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Observable, Subject, fromEvent, merge } from 'rxjs';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { EventService } from '../services/event.service';
import { EventModel } from '../models/event.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { MatCalendar } from '@angular/material/datepicker';
import { ModalService } from 'src/app/core/services/modal.service';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { EventDetailModel } from '../models/event-detail.model';
import { EventPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';

@Component({
  selector: 'facets-event-create',
  templateUrl: './event-create.component.html',
  styleUrls: ['./event-create.component.scss']
})
export class EventCreateComponent implements OnInit, AfterViewInit {

  eventForm: FormGroup;
  eventModel: EventModel = new EventModel();
  selectedEventDates: any[] = [];
  startDate: Date | string = new Date();
  endDate: Date | string = new Date();
  fileList: File[] = [];
  superAdminPermissions = SuperAdminPermissions;
  eventPermissions = EventPermissions;

  isFormSubmitted = false;
  isBlocked = false;
  isEdit = false;
  id = '';
  validationModel: ValidationModel = new ValidationModel();

  router = inject(Router);
  eventService = inject(EventService);
  toasterService = inject(ToasterService);
  modalService = inject(ModalService);

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];
  @ViewChild('calendar') calendar: MatCalendar<Date>;

  constructor(public formBuilder: FormBuilder, private activatedRoute: ActivatedRoute) {
    this.validationModel.validationMessages = {
      name: {
        required: 'Event name is required',
      },
      visitorRegistrationStartDate: {
        required: 'Visitor Registration Start Date is required',
      },
      visitorRegistrationEndDate: {
        required: 'Visitor Registration End Date is required',
      },
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createEventForm();

    this.activatedRoute.params.subscribe((params: Params) => {
      this.id = params['id'];
      if (this.id != "" && this.id != undefined) {
        this.isEdit = true;
        this.getEventById(this.id);
      }
    });
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.eventForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.eventForm, this.isFormSubmitted);
  }

  createEventForm() {
    this.eventForm = this.formBuilder.group({
      name: ['', [Validators.required]],
      description: [''],
      eventDates: [[]],
      visitorRegistrationStartDate: [new Date(), Validators.required],
      visitorRegistrationEndDate: [new Date(), Validators.required]
    });
  }

  getEventById(eventId: string) {
    this.isBlocked = true;
    this.eventService.getById(eventId)
      .subscribe({
        next: (res: ResponseResult<EventDetailModel>) => {
          this.eventModel = Object.assign(this.eventModel, res.data);
          this.eventModel.eventDates = res.data.eventDates.map(m => m.value);
          this.patchEvent(this.eventModel);
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  patchEvent(event: EventModel) {
    this.eventModel = Object.assign(this.eventModel, event);
    this.eventForm.patchValue(event);
    this.startDate = event.visitorRegistrationStartDate;
    this.endDate = event.visitorRegistrationEndDate;
    event.eventDates.forEach(date => {
      this.onSelect(new Date(date), this.calendar);
    });

    if (this.eventModel.logoURL == null)
      this.eventModel.setLogoUrl();
  }

  onStartDateSelect(event: any) {
    this.eventModel.visitorRegistrationStartDate = event;
  }

  onEndDateSelect(event: any) {
    this.eventModel.visitorRegistrationEndDate = event;
  }

  onSelect(event: any, calender: any) {
    const date = this.getShortDate(event);
    const index = this.selectedEventDates.findIndex(x => x == date);
    if (index < 0) this.selectedEventDates.push(date);
    else this.selectedEventDates.splice(index, 1);
    calender.updateTodaysDate();
  }

  isSelected = (event: Date) => {
    const date = this.getShortDate(event);
    return this.selectedEventDates.find(x => x == date) ? "special-date" : ''
  };

  getShortDate(event: any) {
    return `${event.getFullYear()}-${("00" + (event.getMonth() + 1)).slice(-2)}-${("00" + event.getDate()).slice(-2)}`
  }

  create() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.eventForm.invalid) { return; }

    if (this.selectedEventDates.length == 0)
      return this.toasterService.warning("Atleast single event date must be selected");

    this.isBlocked = true;
    this.eventModel = Object.assign(this.eventModel, this.eventForm.value);
    this.eventModel.eventDates = [];
    this.eventModel.mapModel(this.startDate, this.endDate, this.selectedEventDates);

    this.eventService.create(this.eventModel)
      .subscribe({
        next: (res: ResponseResult<EventModel>) => {
          this.uploadImage(res.data.id, "CREATE");
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  update() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.eventForm.invalid) { return; }

    this.isBlocked = true;
    this.eventModel = Object.assign(this.eventModel, this.eventForm.value);
    this.eventModel.eventDates = [];
    this.eventModel.mapModel(this.startDate, this.endDate, this.selectedEventDates);

    this.eventService.update(this.eventModel)
      .subscribe({
        next: (res: any) => {
          this.uploadImage(this.id, "UPDATE");
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  uploadImage(eventId: string, action: "CREATE" | "UPDATE") {
    if (this.fileList.length == 0) {
      this.isFormSubmitted = false;
      this.isBlocked = false;
      this.afterUploadNavigation(action);
      return;
    }

    this.eventService.uploadImage(eventId, this.fileList)
      .subscribe({
        next: (res: any) => {
          this.isFormSubmitted = false;
          this.isBlocked = false;
          this.afterUploadNavigation(action);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        }
      })
  }

  afterUploadNavigation(action: "CREATE" | "UPDATE") {
    if (action == "CREATE") {
      this.router.navigate(['admin/event']);
      this.toasterService.successfullyUpdated("Event");
    } else {
      this.router.navigate(['admin/event/profile', this.id]);
      this.toasterService.successfullyUpdated("Event");
    }
  }

  back() {
    if (this.isEdit) {
      this.router.navigate(['admin/event/profile/', this.id]);
    }
    this.router.navigate(['admin/event']);
  }

  getFiles(event: File[]) {
    if (event.length > 0) {
      this.fileList = event;      
    }
  }

  deleteEvent() {
    let options = {
      title: 'Delete Event',
      message: 'Are you sure, do you want to delete this event?',
    };
    this.modalService.displayDialog(ConfirmationModalComponent, options);
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.eventService.deleteEvent(this.id)
          .subscribe({
            next: (res: any) => {
              this.isBlocked = false;
              this.toasterService.success("Event has been successfully deleted");
              this.router.navigate(['/admin/event'])
            },
            error: (err: ErrorResponse) => {
              this.isBlocked = false;
              this.toasterService.error(err);
            }
          })
      }
    });
  }
}
