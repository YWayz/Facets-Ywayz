import { Component, inject } from '@angular/core';
import { getMenuItems } from '../navigation.data';
import { Router } from '@angular/router';
import { EventModel } from 'src/app/modules/event/models/event.model';
import { EventService } from 'src/app/modules/event/services/event.service';
import { SearchRequestModel } from '../../models/search-request.model';
import { ToasterService } from '../../services/toaster.service';
import { KeyValue } from '../../models/key-value.model';
import { appConstant } from '../../extensions/app-constants';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { isNull } from '../../extensions/helpers';
import { LogoutModel } from 'src/app/modules/auth/models/logout.model';
import { ErrorResponse } from '../../models/error-response.model';
import { SharedService } from '../../services/shared.service';
import { LookupsService } from '../../services/lookups.service';
import { ResponseResult } from '../../models/response-result.model';
import { PassGenerationPermissions } from '../../extensions/permission-constants';

@Component({
  selector: 'facets-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {

  openSidebar: boolean = true;
  isSuperAdmin = false

  menuSidebar = getMenuItems();
  events = new Array<EventModel>();
  eventKeyValues = new Array<KeyValue<string, string>>();
  searchModel = new SearchRequestModel(100, 1);
  activeEventName = "No Active Exhibitions"
  firstName: string | undefined = "";
  lastName: string | undefined = "";
  userRole = "";

  passGenerationPermissions = PassGenerationPermissions;

  router = inject(Router) as Router;
  eventService = inject(EventService);
  toasterService = inject(ToasterService);
  authService = inject(AuthService);
  sharedService = inject(SharedService);
  lookupService = inject(LookupsService);

  constructor() {
    const authData = this.authService.authData;
    authData!.roles[0] == "superadmin" ? this.isSuperAdmin = true : this.isSuperAdmin = false;
    
    this.firstName = authData?.firstName;
    this.lastName = authData?.lastName;
    this.userRole = authData!.roles[0];

    this.getEvents();

    this.sharedService.isEventStatusChanged.subscribe((p: Boolean) => {
      if (p)
        this.getEvents();
    })

  }

  getEvents() {
    this.lookupService.getActiveAssignedEvents().subscribe({
      next: (res: ResponseResult<KeyValue<string, string>[]>) => {
        this.eventKeyValues = res.data;
        this.sharedService.setEventStatus(false);
        this.setDefaultEvent();
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    });
  }

  setDefaultEvent() {
    if (this.eventKeyValues.length == 0)
      return;

    const selectedEventId = localStorage.getItem(appConstant.selectedEventId);
    const stored = selectedEventId ? this.eventKeyValues.find(p => p.key == selectedEventId) : undefined;
    if (stored) {
      this.activeEventName = stored.value;
      return;
    }

    // Nothing chosen yet, or the stored event is no longer available to this user:
    // default to the most recently created event (the list is newest first).
    const event = this.eventKeyValues[0];
    this.activeEventName = event.value;
    localStorage.setItem(appConstant.selectedEventId, event.key)
  }

  showSubmenu(itemEl: HTMLElement) {
    itemEl.classList.toggle("showMenu");
  }

  logout() {
    this.authService.logout(new LogoutModel(this.authService.userId!)).subscribe({
      next: () => {
        localStorage.clear();
        this.router.navigate(['login']);
      },
      error: (err: ErrorResponse) => {
        localStorage.clear();
        this.router.navigate(['login']);
        this.toasterService.error(err);
      }
    })
  }

  userProfile() {
    this.router.navigate(['admin/user-management/user-profile']);
  }

  changeEvent(event: KeyValue<string, string>) {
    this.activeEventName = event.value;
    localStorage.setItem("SelectedEventId", event.key);
    this.router.navigate(['/admin']);
  }
}
