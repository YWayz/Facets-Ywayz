import { Component, OnInit, inject } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { InvoiceService } from 'src/app/modules/visitor/services/invoice.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { PaymentRequestModel } from '../../models/payment-request.model';
import { GatewayModel } from '../../models/gateway.model';
import { OnepayService } from '../../services/onepay.service';

@Component({
  selector: 'facets-visitor-registration-online-payment-success',
  templateUrl: './visitor-registration-online-payment-success.component.html',
  styleUrls: ['./visitor-registration-online-payment-success.component.scss'],
  standalone: true,
  imports: [SharedModule]
})
export class VisitorRegistrationOnlinePaymentSuccessComponent implements OnInit {

  isPaid: boolean;
  invoiceId: string | undefined;
  isBlocked = false;
  isDisableButton = false;
  gatewayModel = new GatewayModel();

  router = inject(Router);
  activatedRoute = inject(ActivatedRoute);
  invoiceService = inject(InvoiceService);
  toasterService = inject(ToasterService);
  onepayService = inject(OnepayService);

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe({
      next: (qParam: Params) => {
        this.invoiceId = qParam['invoiceId'];
        if(this.invoiceId != null || this.invoiceId != undefined) {
          this.checkInvoicePaymentStatus(this.invoiceId);
        }
      }
    })
  }

  // OnePay's confirmation can arrive after the browser is redirected here. Poll for up to a minute
  // before showing "failed"; a premature failure message led visitors to pay a second time.
  isConfirming = true;
  private readonly maxStatusChecks = 12;
  private readonly statusCheckIntervalMs = 5000;

  checkInvoicePaymentStatus(invoiceId: string, attempt: number = 1) {
    this.invoiceService.checkPaymentStatus(invoiceId).subscribe({
      next: (result: ResponseResult<boolean>) => {
        this.isPaid = result.data;
        if (this.isPaid || attempt >= this.maxStatusChecks) {
          this.isConfirming = false;
        } else {
          setTimeout(() => this.checkInvoicePaymentStatus(invoiceId, attempt + 1), this.statusCheckIntervalMs);
        }
      },
      error: (error: ErrorResponse) => {
        this.isConfirming = false;
        this.invoiceId = undefined;
        this.toasterService.error(error);
      }
    })
  }

  goBackHome() {
    // this.router.navigate(['/'])
    window.location.href = 'https://www.facetssrilanka.com/';
  }

  requestPayment() {
    this.isBlocked = true;
    this.isDisableButton = true;

    const paymentRequestModel = new PaymentRequestModel();
    paymentRequestModel.invoiceId = this.invoiceId!;
    this.onepayService.requestPayment(paymentRequestModel).subscribe({
      next: (result: ResponseResult<GatewayModel>) => {
        this.isBlocked = false;
        this.gatewayModel = result.data;    
        window.location.href = this.gatewayModel.redirect_url;
      },
      error: (err: ErrorResponse) => {
        this.isDisableButton = false;
        this.toasterService.error(err);
      }
    })
  }
}
