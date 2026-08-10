import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { UpdatePassCategoryPavilionRateModel } from '../models/update-pass-category-pavilion-rate';

@Injectable({
  providedIn: 'root'
})
export class PassCategoriesPavilionSettingsService extends BaseService {

  constructor() { 
    super();
  }

  update(eventId: string, pavilionId: string, updatePassCategoryPavilionRateModel: UpdatePassCategoryPavilionRateModel) {
    return this.put(`events/${eventId}/pavilions/${pavilionId}/pass-categories-pavilion-settings/rates`, updatePassCategoryPavilionRateModel);
  }
}
