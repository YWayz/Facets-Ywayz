import { Component, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SharedService } from 'src/app/core/services/shared.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { CounterAssignmentStatusModel } from 'src/app/modules/event/models/counter-assignment-status.model';
import { PavilionService } from 'src/app/modules/event/services/pavilion.service';
import { RegistrationCounterService } from 'src/app/modules/event/services/registration-counter.service';

@Component({
  selector: 'facets-visitor-registration-overview',
  templateUrl: './visitor-registration-overview.component.html',
  styleUrls: ['./visitor-registration-overview.component.scss']
})
export class VisitorRegistrationOverviewComponent implements OnInit {

  isCounterSelection = false;
  isProfile = false;
  isOtp = false;
  isAttachment = false;
  isPass = false;
  isPavilion = false;
  isPayment = false;
  isRegistrationSuccess = false;
  hasCounterAssigned = false;
  isPavilionSessionExist = false;
  visitorId: string;
  finalAmount: number;
  selectedPassCategories: string[];

  counterTypes: string[] = ['RegistrationOnly', 'RegistrationAndPayment'];

  router = inject(Router);
  registrationCounterService = inject(RegistrationCounterService);
  pavilionService = inject(PavilionService);
  toasterService = inject(ToasterService);
  sharedService = inject(SharedService);

  ngOnInit(): void {
    this.isOtp = this.router.url.endsWith('/visitor/register') ? false : true;
    this.sharedService.isUpdateProfile = false;
    this.sharedService.isUpdatePass = false;
    this.sharedService.isUpdateAttachment = false;
    this.checkCounterAssignment();
    this.checkPavilionSessionsExist();
  }

  setPassCategoryData(passCategories: string[]) {
    this.selectedPassCategories = passCategories;
  }

  setPayment(finalAmount: number) {
    this.finalAmount = finalAmount;
  }
  
  goToRegistrationCounter() {
    this.isCounterSelection = true;
    this.isProfile = false;
  }

  goToProfile(visitorId?: string) {
    this.isCounterSelection = false;
    this.isProfile = true;
    this.isAttachment = false;
    this.hasCounterAssigned = true;
    if(visitorId != undefined) this.visitorId = visitorId;
  }

  goToAttachment(visitorId: string) {
    this.isProfile = false;
    this.isAttachment = true;
    this.isPass = false;
    this.visitorId = visitorId;
  }

  goToPass(visitorId: string) {
    this.isProfile = false;
    this.isAttachment = false;
    this.isPass = true;
    this.visitorId = visitorId;
  }

  goToPavilion(visitorId?: string) {
    this.isProfile = false;
    this.isAttachment = false;
    this.isPass = false;
    this.isPavilion = true;
    this.isPayment = false;
    if(visitorId != undefined) this.visitorId = visitorId;
  }

  goToPayment() {
    this.isProfile = false;
    this.isAttachment = false;
    this.isPass = false;
    this.isPavilion = false;
    this.isPayment = true;
  }

  goToPaymentBack() {
    this.goToPass(this.visitorId);
    this.isPavilion = false;
    this.isPayment = false;
  }

  goToRegisterSuccess() {
    this.isProfile = false;
    this.isAttachment = false;
    this.isPass = false;
    this.isPavilion = false;
    this.isPayment = false;
    this.isRegistrationSuccess = true;
  }

  checkCounterAssignment() {
    this.registrationCounterService.registrationCounterAssignment(this.counterTypes).subscribe({
      next: (result: ResponseResult<CounterAssignmentStatusModel>) => {
        this.hasCounterAssigned = result.data.hasCounterAssigned;
        this.hasCounterAssigned ? this.goToProfile() : this.goToRegistrationCounter(); 
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    })
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
