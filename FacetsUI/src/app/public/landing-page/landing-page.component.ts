import { Component, Host, OnInit, inject } from '@angular/core';
import { PublicSiteService } from '../services/public-site.service';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { PublicSiteEventSummaryModel } from '../models/public-site-event-summary.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { Router } from '@angular/router';
import { appConstant } from 'src/app/core/extensions/app-constants';

@Component({
  selector: 'facets-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.scss']
})
export class LandingPageComponent implements OnInit {

  publicSiteEventsSummaryModel = new Array<PublicSiteEventSummaryModel>();
  upComingEventsSummaryModel = new Array<PublicSiteEventSummaryModel>();
  onGoingEventsSummaryModel = new Array<PublicSiteEventSummaryModel>();
  display: "Home" | "Ongoing" | "UpComing" = 'Home';
  searchRequestModel = new SearchRequestModel(100, 1);

  publicSiteService = inject(PublicSiteService);
  toasterService = inject(ToasterService);
  router = inject(Router);

  ngOnInit(): void {
    this.getEvents();
  }

  getEvents() {
    this.publicSiteService.getAllEvents(this.searchRequestModel).subscribe({
      next: (res: ResponseResult<PublicSiteEventSummaryModel[]>) => {
        this.publicSiteEventsSummaryModel = res.data;
        this.onGoingEventsSummaryModel = this.publicSiteEventsSummaryModel.filter(s => s.isUpComming == false);
        this.upComingEventsSummaryModel = this.publicSiteEventsSummaryModel.filter(s => s.isUpComming == true);

        this.onGoingEventsSummaryModel.forEach(event => {
          if (event.logoURL == null) {
            event.logoURL = '../../assets/images/no-image2.png';
          }
        });

        this.upComingEventsSummaryModel.forEach(event => {
          if (event.logoURL == null) {
            event.logoURL = '../../assets/images/no-image2.png';
          }
        });
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      },
    });
  }

  navigateToVisitorRegistration(eventId: string) {
    localStorage.setItem(appConstant.selectedEventId, eventId)
    this.router.navigate([`visitor/register`]);
  }

  show(type: "Home" | "Ongoing" | "UpComing") {
    this.display = type;

    window.scroll({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });
  }

  goToAdmin() {
    this.router.navigate(['admin']);
  }
}
