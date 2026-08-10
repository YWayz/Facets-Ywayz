import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { VisitorPassTemplateModel } from '../models/visitor-pass-template.model';
import { PassVerificationModel } from '../models/pass-verification.model';
import { QRVerifiedVisitorModel } from '../models/qr-verified-visitor.model';

@Injectable({
  providedIn: 'root'
})
export class VisitorPassService extends BaseService {

  constructor() {
    super();
  }

  getTemplate(visitorId: string, eventDateId: string): Observable<ResponseResult<VisitorPassTemplateModel>> {
    return this.get<ResponseResult<VisitorPassTemplateModel>>(`visitor-passes/template/${localStorage.getItem(appConstant.selectedEventId)}/${visitorId}/${eventDateId}`);
  }

  getVisitorVerificationDetails(visitorId: string, eventDateId: string): Observable<ResponseResult<PassVerificationModel>> {
    return this.get<ResponseResult<PassVerificationModel>>(`visitor-passes/verification-details/event/${localStorage.getItem(appConstant.selectedEventId)}/${visitorId}/${eventDateId}`);
  }

  update(passVerificationModel: PassVerificationModel): Observable<ResponseResult<QRVerifiedVisitorModel>> {
    return this.putWithReturn<ResponseResult<QRVerifiedVisitorModel>>(`visitor-passes/verify-qr`, passVerificationModel);
  }

  updatePavilion(passVerificationModel: PassVerificationModel): Observable<ResponseResult<QRVerifiedVisitorModel>> {
    return this.putWithReturn<ResponseResult<QRVerifiedVisitorModel>>(`visitor-passes/verify-qr/pavilion-session`, passVerificationModel);
  }

  updatePassPrintStatus(visitorId: string, attendanceScheduleId: string) {
    return this.put(`visitors/${visitorId}/attendance/${attendanceScheduleId}/mark-pass-as-printed`);
  }
}
