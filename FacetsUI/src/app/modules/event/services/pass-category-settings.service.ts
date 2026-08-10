import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { PassCategoryRatesModel } from '../models/pass-category-rates.model';
import { UpdatePassCategoryPavilionRateModel } from '../models/update-pass-category-pavilion-rate';

@Injectable({
  providedIn: 'root'
})
export class PassCategorySettingsService extends BaseService {

  constructor() { 
    super();
  }

  getAll(eventId: string): Observable<ResponseResult<PassCategoryRatesModel[]>> {
    return this.get<ResponseResult<PassCategoryRatesModel[]>>(`events/${eventId}/pass-categories/rates`);
  }

  getPassRate(eventId: string, passCategoryId: string, id: string): Observable<ResponseResult<PassCategoryRatesModel>> {
    return this.get<ResponseResult<PassCategoryRatesModel>>(`events/${eventId}/pass-categories/${passCategoryId}/rates/${id}`);
  }

  update(eventId: string, passCategoryId: string, id: string, passCategoryRateModel: PassCategoryRatesModel) {
    return this.put(`events/${eventId}/pass-categories/${passCategoryId}/rates/${id}`, passCategoryRateModel);
  }
  
  updateIsChargeableStatus(eventId: string, passCategoryId: string, id: string, isChargeable: boolean) {
    return this.put(`events/${eventId}/pass-categories/${passCategoryId}/rates/${id}/status`, { isChargeable: isChargeable });
  }
  
  updatePavilionRates(eventId: string, updatePassCategoryPavilionRateModel: UpdatePassCategoryPavilionRateModel) {
    return this.put(`events/${eventId}/pass-categories/pavilions/rates`, updatePassCategoryPavilionRateModel);
  }
}