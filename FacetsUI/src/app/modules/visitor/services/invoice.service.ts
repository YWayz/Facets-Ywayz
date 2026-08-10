import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { InvoicePaymentModel } from '../models/invoice-payment.model';
import { InvoiceModel } from '../models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService extends BaseService {

  constructor() {
    super();
  }

  getInvoicePaymentByRegistration(visitorId: string, eventDateId?: string): Observable<ResponseResult<InvoicePaymentModel>> {
    let url = `visitors/${visitorId}/invoice`;

    if (eventDateId != null || eventDateId != undefined || eventDateId != '')
      url = `${url}?eventDateIds=${eventDateId}`

    return this.get<ResponseResult<InvoicePaymentModel>>(url);
  }

  getInvoicesByRegistration(visitorRegistrationId: string, eventDateIds?: string, paymentStatuses?: string): Observable<ResponseResult<InvoiceModel[]>> {
    let url = `registrations/${visitorRegistrationId}/invoices`;

    if (eventDateIds != null && eventDateIds != undefined)
      url = `${url}?eventDateIds=${eventDateIds}`

    if (paymentStatuses != null && paymentStatuses != undefined)
      url = `${url}?paymentStatuses=${paymentStatuses}`

    return this.get<ResponseResult<InvoiceModel[]>>(url);
  }

  checkPaymentStatus(invoiceId: string): Observable<ResponseResult<boolean>> {
    return this.get<ResponseResult<boolean>>(`invoices/${invoiceId}/payment-status`);
  }
}
