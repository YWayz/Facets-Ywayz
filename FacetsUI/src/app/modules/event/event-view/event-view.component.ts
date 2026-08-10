import { ChangeDetectionStrategy, Component, OnInit, ViewChild, inject } from '@angular/core';
import { EventModel } from '../models/event.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { EventService } from '../services/event.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { Router } from '@angular/router';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { ModalService } from 'src/app/core/services/modal.service';
import { EventPermissions, PassCategoryPermissions, PavilionPermissions, RegistrationCounterPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { SharedService } from 'src/app/core/services/shared.service';

@Component({
  selector: 'facets-event-view',
  changeDetection: ChangeDetectionStrategy.Default,
  templateUrl: './event-view.component.html',
  styleUrls: ['./event-view.component.scss']
})
export class EventViewComponent implements OnInit {

  isBlocked = false;
  events = new Array<EventModel>();
  searchModel = new SearchRequestModel(10, 1);
  pageSizeOptions: number[] = [10, 25, 50, 100];

  registrationCounterPermissions = RegistrationCounterPermissions;
  superAdminPermissions = SuperAdminPermissions;
  passCategoryPermissions = PassCategoryPermissions;
  eventPermissions = EventPermissions;
  pavilionPermissions = PavilionPermissions;

  @ViewChild(MatPaginator) paginator: MatPaginator;

  eventService = inject(EventService);
  toasterService = inject(ToasterService);
  modalService = inject(ModalService);
  sharedService = inject(SharedService);

  router = inject(Router);

  ngOnInit(): void {
    this.getEvents();
  }

  getEvents() {
    this.isBlocked = true;
    this.eventService.getAll(this.searchModel)
      .subscribe({
        next: (res: ResponseResult<EventModel[]>) => {
          this.events = res.data;
          this.searchModel.totalRecords = res.totalRecordCount;
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    this.getEvents();
  }

  eventStatusUpdate(event: any, eventModel: EventModel) {
    let status = event.target?.checked;
    event.preventDefault();

    let options = {
      title: 'Update Event Status',
      message: 'Are you sure you want to update the status of the event?',
    };
    this.modalService.displayDialog(ConfirmationModalComponent, options);
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        let body = { eventStatus: status ? "Active" : "Inactive" }
        this.isBlocked = true;
        this.eventService.updateStatus(eventModel.id, body)
          .subscribe({
            next: (res: any) => {
              eventModel.eventStatus = status ? "Active" : "Inactive";
              this.sharedService.setEventStatus(true);
              this.isBlocked = false;
              this.toasterService.success("Event status has been successfully updated");
            },
            error: (err: ErrorResponse) => {
              this.isBlocked = false;
              this.toasterService.error(err);
            }
          })
      }
    });
  }

  navigateToProfile(id: string) {
    this.router.navigate(['admin/event/profile', id]);
  }

  navigateToRegistrationSettings(eventId: string) {
    this.router.navigate([`admin/event/${eventId}/configuration/reg-settings`])
  }

  navigateEventPass(eventId: string) {
    this.router.navigate([`admin/event/${eventId}/configuration/event-pass`])
  }
}
