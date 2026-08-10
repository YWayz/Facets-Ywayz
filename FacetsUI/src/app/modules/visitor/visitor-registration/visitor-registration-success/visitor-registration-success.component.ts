import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { OnSitePayingMode } from 'src/app/core/extensions/app-constants';
import { SharedService } from 'src/app/core/services/shared.service';

@Component({
  selector: 'facets-visitor-registration-success',
  templateUrl: './visitor-registration-success.component.html',
  styleUrls: ['./visitor-registration-success.component.scss']
})
export class VisitorRegistrationSuccessComponent {

  router = inject(Router);
  sharedService = inject(SharedService);

  onSitePayingModel = OnSitePayingMode;

  navigateToPassGeneration() {
    this.sharedService.onSitePayingMode = OnSitePayingMode.payAtRegistration;
    this.router.navigate([`admin/visitor/pass-generation`], { queryParams: { nicPassport: this.sharedService.nicPassport } })    
  }

  navigateToVisitorRegistration() {
    this.sharedService.onSitePayingMode = OnSitePayingMode.payAtRegistration;
    this.router.navigateByUrl(`/`, { skipLocationChange: true }).then(() => {
      this.router.navigate([`admin/visitor/register`]);
    });
  }
}