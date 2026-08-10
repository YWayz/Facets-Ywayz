import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { PaymentSettingsModel } from '../models/payment-settings.model';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';

@Injectable({
  providedIn: 'root'
})
export class EventSettingsService extends BaseService {

  constructor() {
    super();
  }

  getPaymentSettings(eventId: string): Observable<ResponseResult<PaymentSettingsModel>> {
    return this.get<ResponseResult<PaymentSettingsModel>>(`events/${eventId}/settings/payment-settings`);
  }

  updatePaymentSettings(eventId: string, updatePaymentSettingsModel: PaymentSettingsModel) {
    return this.put(`events/${eventId}/settings/update-payment-settings`, updatePaymentSettingsModel)
  }

}
