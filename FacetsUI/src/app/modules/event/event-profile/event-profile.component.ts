import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { EventModel } from '../models/event.model';
import { EventService } from '../services/event.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { EventDetailModel } from '../models/event-detail.model';
import { EventPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';

@Component({
  selector: 'facets-event-profile',
  templateUrl: './event-profile.component.html',
  styleUrls: ['./event-profile.component.scss']
})
export class EventProfileComponent implements OnInit {

  isBlocked = false;
  id = '';

  eventDetailModel: EventDetailModel = new EventDetailModel();
  superAdminPermissions = SuperAdminPermissions;
  eventPermissions = EventPermissions;

  eventService = inject(EventService);
  toasterService = inject(ToasterService);

  constructor(public formBuilder: FormBuilder, private activatedRoute: ActivatedRoute,
    private router: Router) {
  }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => {
      this.id = params['id'];
      if (this.id != "" && this.id != undefined) {
        this.getEventById(this.id);
      }
    });
  }

  getEventById(eventId: string) {
    this.isBlocked = true;
    this.eventService.getById(eventId)
      .subscribe({
        next: (res: ResponseResult<EventDetailModel>) => {
          Object.assign(this.eventDetailModel, res.data);
          if (this.eventDetailModel.logoURL == null) {
            this.eventDetailModel.setLogoUrl();
          }

          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  navigateToEdit() {
    this.router.navigate(['admin/event/edit', this.id]);
  }
}
