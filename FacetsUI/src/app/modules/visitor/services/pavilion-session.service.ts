import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { PavilionSessionSummaryModel } from '../models/pavilion-session-summary.model';
import { VisitorPavilionSessionAttendanceScheduleModel } from '../models/visitor-pavilion-session-attendance-schedule.model';

@Injectable({
  providedIn: 'root'
})
export class PavilionSessionService extends BaseService {

  constructor() { 
    super();
  }

  getAll(pavilionId: string): Observable<ResponseResult<PavilionSessionSummaryModel[]>> {
    return this.get<ResponseResult<PavilionSessionSummaryModel[]>>(`events/${localStorage.getItem(appConstant.selectedEventId)}/pavilions/${pavilionId}/pavilion-sessions`);
  }

  getVisitorPavilionSessions(visitorRegistrationId: string): Observable<ResponseResult<VisitorPavilionSessionAttendanceScheduleModel[]>> {
    return this.get<ResponseResult<VisitorPavilionSessionAttendanceScheduleModel[]>>(`registrations/${visitorRegistrationId}/pavilion-sessions`);
  }
}
