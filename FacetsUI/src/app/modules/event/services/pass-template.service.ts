import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { PassTemplateModel } from '../models/pass-template.model';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';

@Injectable({
  providedIn: 'root'
})
export class PassTemplateService extends BaseService {

  constructor() {
    super();
  }

  create(eventId: string, passTemplateModel: PassTemplateModel): Observable<ResponseResult<PassTemplateModel>> {
    return this.post<ResponseResult<PassTemplateModel>>(`events/${eventId}/pass-templates`, passTemplateModel);
  }

  getAll(searchModel: SearchRequestModel, eventId: string, passType: string): Observable<ResponseResult<PassTemplateModel[]>> {
    return this.get<ResponseResult<PassTemplateModel[]>>(`events/${eventId}/pass-templates?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&passType=${passType}`);
  }

  update(eventId: string, passTemplateModel: PassTemplateModel) {
    return this.put(`events/${eventId}/pass-templates/${passTemplateModel.id}`, passTemplateModel)
  }
}
