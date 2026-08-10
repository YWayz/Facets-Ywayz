import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { CreatePavilionModel } from '../models/create-pavilion.model';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { PavilionModel } from '../models/pavilion.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { UpdatePavilionModel } from '../models/update-pavilion.model';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { PavilionSummaryModel } from '../../visitor/models/pavilion-summary.model';

@Injectable({
  providedIn: 'root'
})
export class PavilionService extends BaseService {

  constructor() {
    super();
  }

  createPavilion(eventId: string, createPavilionModel: CreatePavilionModel): Observable<ResponseResult<PavilionModel>> {
    return this.post<ResponseResult<PavilionModel>>(`events/${eventId}/pavilions`, createPavilionModel);
  }

  getPavilionById(eventId: string, id: string): Observable<ResponseResult<PavilionModel>> {
    return this.get<ResponseResult<PavilionModel>>(`events/${eventId}/pavilions/${id}`);
  }

  getAllPavilions(eventId: string, searchModel: SearchRequestModel): Observable<ResponseResult<PavilionModel[]>> {
    return this.get<ResponseResult<PavilionModel[]>>(`events/${eventId}/pavilions?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}`);
  }

  updatePavilion(eventId: string, id: string, updatePavilionModel: UpdatePavilionModel) {
    return this.put(`events/${eventId}/pavilions/${id}`, updatePavilionModel);
  }

  deletePavilion(eventId: string, id: string) {
    return this.delete(`events/${eventId}/pavilions/${id}`);
  }

  updatePavilionStatus(eventId: string, id: string, body: { pavilionStatus: string }) {
    return this.put(`events/${eventId}/pavilions/${id}/status`, body)
  }
//Live Event Id Replace as Request 86d0a7c8-be73-47c2-041e-08dc119fbf38 : local c7c1b543-a444-4b1d-3f0c-08dcd9764f6b
  checkPavilionSessionsExist() {
    // return this.get<ResponseResult<boolean>>(`events/${localStorage.getItem(appConstant.selectedEventId)}/pavilions/check-pavilion-sessions-exists`);
     return this.get<ResponseResult<boolean>>(`events/${'86d0a7c8-be73-47c2-041e-08dc119fbf38'}/pavilions/check-pavilion-sessions-exists`);
  }

  getAll(pavilionStatus?: string): Observable<ResponseResult<PavilionSummaryModel[]>> {
    let url = `events/${localStorage.getItem(appConstant.selectedEventId)}/pavilions`;

    if (pavilionStatus != undefined && pavilionStatus != null)
      url = `${url}?pavilionStatus=${pavilionStatus}`;

    return this.get<ResponseResult<PavilionSummaryModel[]>>(url);
  }
}
