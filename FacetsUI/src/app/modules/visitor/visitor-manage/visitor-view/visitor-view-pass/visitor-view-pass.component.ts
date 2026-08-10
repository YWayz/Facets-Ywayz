import { Component, OnInit, inject } from '@angular/core';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { VisitorRegistrationService } from '../../../services/visitor-registration.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { VisitorPassRegistrationModel } from '../../../models/visitor-pass-registration.model';
import { ActivatedRoute, Params } from '@angular/router';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { InvoiceService } from '../../../services/invoice.service';
import { InvoiceLineItemModel, InvoiceModel } from '../../../models/invoice.model';

@Component({
  selector: 'facets-visitor-view-pass',
  templateUrl: './visitor-view-pass.component.html',
  styleUrls: ['./visitor-view-pass.component.scss']
})
export class VisitorViewPassComponent implements OnInit {

  isBlocked = false;

  visitorPassRegistration = new VisitorPassRegistrationModel();
  invoiceModel = new Array<InvoiceModel>();
  invoiceLineItemsModel = new Array<InvoiceLineItemModel>();

  toasterService = inject(ToasterService);
  visitorRegistrationService = inject(VisitorRegistrationService);
  invoiceService = inject(InvoiceService);
  activatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => {
      const visitorId = params['visitorId']
      const visitorRegistrationId = params['registrationId']
      if (visitorId != null || visitorId != undefined || visitorId != '') {
        this.getVisitorPassRegistration(visitorId, visitorRegistrationId);
      }
    });
  }

  getVisitorPassRegistration(visitorId: string, visitorRegistrationId: string) {
    this.isBlocked = true;
    this.visitorRegistrationService.getVisitorPassRegistration(visitorId, visitorRegistrationId)
      .subscribe({
        next: (res: ResponseResult<VisitorPassRegistrationModel>) => {
          this.isBlocked = false;
          this.visitorPassRegistration = res.data;
          this.getInvoicesByRegistration();
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  getInvoicesByRegistration() {
    this.isBlocked = true;
    this.invoiceService.getInvoicesByRegistration(this.visitorPassRegistration.visitorRegistrationId)
      .subscribe({
        next: (res: ResponseResult<InvoiceModel[]>) => {
          this.invoiceModel = res.data;
          for (let invoice of this.invoiceModel) {
            this.invoiceLineItemsModel.push(...invoice.invoiceLineItems);
          }

          this.visitorPassRegistration.attendanceSchedules.forEach(schedule => {
            schedule.amount = this.invoiceLineItemsModel.find(f => f.visitorAttendanceScheduleId == schedule.attendanceScheduleId)?.amount ?? 0.00;
          })
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }
}
