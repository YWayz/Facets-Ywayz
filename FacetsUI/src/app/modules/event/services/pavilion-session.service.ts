import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { CreatePavilionSessionModel } from '../models/create-pavilion-session.model';
import { PavilionSessionModel } from '../models/pavilion-session.model';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { UpdatePavilionSessionModel } from '../models/update-pavilion-session.model';

@Injectable({
  providedIn: 'root'
})
export class PavilionSessionService extends BaseService {

  constructor() {
    super();
  }

  createPavilionSession(eventId: string, pavilionId: string, createPavilionSessionModel: CreatePavilionSessionModel): Observable<ResponseResult<PavilionSessionModel>> {
    return this.post<ResponseResult<PavilionSessionModel>>(`events/${eventId}/pavilions/${pavilionId}/pavilion-sessions`, createPavilionSessionModel);
  }

  getAllPavilionSessions(eventId: string, pavilionId: string): Observable<ResponseResult<PavilionSessionModel[]>> {
    return this.get<ResponseResult<PavilionSessionModel[]>>(`events/${eventId}/pavilions/${pavilionId}/pavilion-sessions`);
  }

  updatePavilion(eventId: string, pavilionId: string, pavilionSessionId: string, updatePavilionModel: UpdatePavilionSessionModel) {
    return this.put(`events/${eventId}/pavilions/${pavilionId}/pavilion-sessions/${pavilionSessionId}`, updatePavilionModel);
  }
  
  deletePavilionSession(eventId: string, pavilionId: string, pavilionSessionId: string) {
    return this.delete(`events/${eventId}/pavilions/${pavilionId}/pavilion-sessions/${pavilionSessionId}`);
  }
}
