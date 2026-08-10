import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { PaymentRequestModel } from '../models/payment-request.model';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { GatewayModel } from '../models/gateway.model';

@Injectable({
  providedIn: 'root'
})
export class OnepayService extends BaseService {

  constructor() {
    super();
  }

  requestPayment(PaymentRequestModel: PaymentRequestModel): Observable<ResponseResult<GatewayModel>> {
    return this.post<ResponseResult<GatewayModel>>(`onepay/request-payment`, PaymentRequestModel);
  }
}
