import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { PassCategoryModel } from '../models/pass-category.model';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { UpdateVisitorPassCategoryTypeModel } from '../models/update-visitor-pass-catery-type.model';

@Injectable({
  providedIn: 'root'
})
export class PassCategoryService extends BaseService {

  constructor() { 
    super();
  }

  create(eventId: string, passCategoryModel: PassCategoryModel): Observable<ResponseResult<PassCategoryModel>> {
    return this.post<ResponseResult<PassCategoryModel>>(`events/${eventId}/pass-categories`, passCategoryModel);
  }

  update(eventId: string, id: string, passCategoryModel: PassCategoryModel) {
    return this.put(`events/${eventId}/pass-categories/${id}`, passCategoryModel);
  }

  updatePassCategoryType(eventId: string, id: string, updateVisitorPassCategoryTypeModel: UpdateVisitorPassCategoryTypeModel) {
    return this.put(`events/${eventId}/pass-categories/${id}/visitor-pass-category-type`, updateVisitorPassCategoryTypeModel);
  }


  getAll(eventId: string, ): Observable<ResponseResult<PassCategoryModel[]>> {
    return this.get<ResponseResult<PassCategoryModel[]>>(`events/${eventId}/pass-categories`);
  }

  getPassCategory(eventId: string, id: string): Observable<ResponseResult<PassCategoryModel>> {
    return this.get<ResponseResult<PassCategoryModel>>(`events/${eventId}/pass-categories/${id}`);
  }

  deleteCategory(eventId: string, id: string) {
    return this.delete(`events/${eventId}/pass-categories/${id}`);
  }
}
