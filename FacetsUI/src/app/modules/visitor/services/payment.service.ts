import { Injectable } from '@angular/core';
import { PaymentModel } from '../models/payment.model';
import { BaseService } from 'src/app/core/services/base.service';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { VisitorModel } from '../models/visitor.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService extends BaseService {

  constructor() { 
    super();
  }

  create(paymentModel: PaymentModel) {
    return this.post('payments', paymentModel);
  }
}
