import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SharedService } from 'src/app/core/services/shared.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { CounterAssignmentStatusModel } from 'src/app/modules/event/models/counter-assignment-status.model';
import { RegistrationCounterService } from 'src/app/modules/event/services/registration-counter.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { VisitorRegistrationOtpComponent } from "../visitor-registration-otp/visitor-registration-otp.component";
import { VisitorRegistrationAttachmentsComponent } from "../visitor-registration-attachments/visitor-registration-attachments.component";
import { VisitorRegistrationPassComponent } from "../visitor-registration-pass/visitor-registration-pass.component";
import { VisitorRegistrationProfileComponent } from "../visitor-registration-profile/visitor-registration-profile.component";
import { VisitorRegistrationPaymentComponent } from "../visitor-registration-payment/visitor-registration-payment.component";
import { VisitorRegistrationSuccessComponent } from "../visitor-registration-success/visitor-registration-success.component";
import { VisitorVerificationModel } from '../../models/visitor-verification.model';
import { PavilionService } from 'src/app/modules/event/services/pavilion.service';
import { PublicSiteService } from '../../services/public-site.service';
import { PublicSiteEventSummaryModel } from '../../models/public-site-event-summary.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { VisitorRegistrationPavilionComponentimplements } from "../visitor-registration-pavilion/visitor-registration-pavilion.component";

@Component({
    selector: 'facets-visitor-registration-overview',
    templateUrl: './visitor-registration-overview.component.html',
    styleUrls: ['./visitor-registration-overview.component.scss'],
    standalone: true,
    imports: [SharedModule, VisitorRegistrationOtpComponent, VisitorRegistrationAttachmentsComponent, VisitorRegistrationPassComponent, VisitorRegistrationProfileComponent, VisitorRegistrationPaymentComponent, VisitorRegistrationSuccessComponent, VisitorRegistrationPavilionComponentimplements]
})
export class VisitorRegistrationOverviewComponent implements OnInit, OnDestroy {  

  isProfile = false;
  isOtp = false;
  isAttachment = false;
  isPass = false;
  isPayment = false;
  isRegistrationSuccess = false;
  hasCounterAssigned = false;
  isPavilion = false;
  isPavilionSessionExist = false;

  visitorId: string;
  nic: string;
  identityNumber: string;
  model: VisitorVerificationModel;
  visitorRegistrationId: string;
  finalAmount: number;
  selectedPassCategories: string[];

  router = inject(Router);
  registrationCounterService = inject(RegistrationCounterService);
  toasterService = inject(ToasterService);
  pavilionService = inject(PavilionService);
  sharedService = inject(SharedService);
  activatedRoute = inject(ActivatedRoute);
  publicSiteService = inject(PublicSiteService);

  ngOnInit(): void {
    this.sharedService.isUpdateOtp = false;
    this.sharedService.isUpdateProfile = false;
    this.sharedService.isUpdatePass = false;
    this.sharedService.isUpdateAttachment = false;
    this.ensureEventSelected(() => {
      this.checkPavilionSessionsExist();
      this.goToProfile();
    });
  }

  // A visitor who opens the registration page directly (a shared link, a bookmark) has not picked an
  // event on the landing page. Default to the most recently created event; the list is newest first.
  private ensureEventSelected(next: () => void) {
    if (localStorage.getItem(appConstant.selectedEventId)) {
      next();
      return;
    }

    this.publicSiteService.getAllEvents(new SearchRequestModel(1, 1)).subscribe({
      next: (res: ResponseResult<PublicSiteEventSummaryModel[]>) => {
        const newest = res.data?.[0];
        if (newest) {
          localStorage.setItem(appConstant.selectedEventId, newest.id);
          next();
        } else {
          this.toasterService.warning('There is no event open for registration at the moment.');
          this.router.navigate(['/']);
        }
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
        this.router.navigate(['/']);
      }
    });
  }

  setPassCategoryData(passCategories: string[]) {
    this.selectedPassCategories = passCategories;
  }

  setEventVisitorRegistrationId(visitorRegistrationId: string) {
    this.visitorRegistrationId = visitorRegistrationId;
  }

  setPayment(finalAmount: number) {
    this.finalAmount = finalAmount;
  }

  goToProfile(nic?: string) {
    this.isOtp = false;
    this.isProfile = true;
    this.isAttachment = false;
    if (nic != undefined) this.nic = nic;
  }

  goToPavilion(visitorId?: string) {
    this.isProfile = false;
    this.isAttachment = false;
    this.isPass = false;
    this.isPavilion = true;
    this.isPayment = false;
    if(visitorId != undefined) this.visitorId = visitorId;
  }

  goToOtp(model: VisitorVerificationModel) {
    this.isProfile = false;
    this.isOtp = true;
    this.isAttachment = false;

    if (model.nicNumber !== undefined) {
      this.identityNumber = model.nicNumber;
    }
    else {
      this.identityNumber = model.identificationNumber;
    }

    if (model != undefined) this.model = model;
  }

  goToAttachment(visitorId: string) {
    this.isProfile = false;
    this.isOtp = false;
    this.isAttachment = true;
    this.isPass = false;
    this.visitorId = visitorId;
  }

  goToPass(visitorId: string) {
    this.isProfile = false;
    this.isOtp = false;
    this.isAttachment = false;
    this.isPass = true;
    this.visitorId = visitorId;
  }

  goToPayment() {
    this.isOtp = false;
    this.isProfile = false;
    this.isPavilion = false;
    this.isAttachment = false;
    this.isPass = false;
    this.isPayment = true;
  }

  goToPaymentBack() {
    this.isOtp = false;
    this.goToPass(this.visitorId);
    this.isPavilion = false;
    this.isPayment = false;
  }

  goToRegisterSuccess() {
    this.isOtp = false;
    this.isProfile = false;
    this.isAttachment = false;
    this.isPass = false;
    this.isPavilion = false;
    this.isPayment = false;
    this.isRegistrationSuccess = true;
  }

  ngOnDestroy(): void {
    localStorage.clear();
    this.router.navigate(['']);
  }

  checkPavilionSessionsExist() {
    this.pavilionService.checkPavilionSessionsExist().subscribe({
      next: (result: ResponseResult<boolean>) => {
        this.isPavilionSessionExist = result.data;
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    })
  }
}
