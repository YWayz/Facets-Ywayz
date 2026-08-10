import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { VisitorEventRegistrationModel } from '../models/visitor-event-registration.model';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { VisitorRegisterModel } from '../models/visitor-register.model';
import { VisitorRegisterSummaryModel } from '../models/visitor-register-summary.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { VisitorPassRegistrationModel } from '../models/visitor-pass-registration.model';
import { CancelVisitorAttendanceModel } from '../models/cancel-visitor-attendance.model';
import { convertOnlyDate } from 'src/app/core/extensions/helpers';
import { VisitorPassModel } from '../models/visitor-pass.model';
import { PavilionSessionVisitorModel } from '../models/pavilion-sessionvisitor.model';

@Injectable({
  providedIn: 'root'
})
export class VisitorRegistrationService extends BaseService {

  queryString: string;

  constructor() {
    super();
  }

  create(visitorEventRegistrationModel: VisitorEventRegistrationModel): Observable<ResponseResult<VisitorRegisterModel>> {
    return this.post<ResponseResult<VisitorRegisterModel>>('registrations', visitorEventRegistrationModel);
  }

  update(visitorRegistrationId: string, visitorRegistrationModel: VisitorRegisterModel) {
    return this.put(`registrations/${visitorRegistrationId}`, visitorRegistrationModel);
  }

  getById(id: string): Observable<ResponseResult<VisitorRegisterModel>> {
    return this.get<ResponseResult<VisitorRegisterModel>>(`registrations/${id}`);
  }

  getVisitorRegistration(visitorId: string): Observable<ResponseResult<VisitorRegisterModel>> {
    return this.get<ResponseResult<VisitorRegisterModel>>(`visitors/${visitorId}/registration`);
  }

  getRegistrations(searchModel: SearchRequestModel, visitorId: string, countryId?: string, visitorStatus?: string, registrationCancelled: boolean = false): Observable<ResponseResult<VisitorRegisterSummaryModel[]>> {
    let url = `registrations?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&searchQuery=${encodeURIComponent(searchModel.searchTerm)}&registrationCancelled=${registrationCancelled}`

    if (visitorStatus != "" && visitorStatus != null) {
      url = `${url}&visitorStatus=${visitorStatus}`
    }

    if (countryId != "") {
      url = `${url}&countryId=${countryId}`
    }

    if (visitorId != "") {
      url = `${url}&visitorId=${visitorId}`
    }

    return this.get<ResponseResult<VisitorRegisterSummaryModel[]>>(url);
  }

  getVisitorPassRegistration(visitorId: string, visitorRegistrationId: string, eventDateId?: string): Observable<ResponseResult<VisitorPassRegistrationModel>> {
    let url = `registrations/${visitorId}/pass/registrations/${visitorRegistrationId}`;

    if (eventDateId != null && eventDateId != undefined)
      url = `${url}?eventDateIds=${eventDateId}`

    return this.get<ResponseResult<VisitorPassRegistrationModel>>(url);
  }


  getVisitorPassesByDates(searchModel: SearchRequestModel, eventDateId: string, attendanceSheduleId?: string, isInvoiced?: boolean, isPrinted: boolean = true): Observable<ResponseResult<VisitorPassModel[]>> {
    let url = `event-dates/${eventDateId}/attendees?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&searchTerm=${encodeURIComponent(searchModel.searchTerm)}`;

    if (isInvoiced != undefined)
      url = `${url}&isInvoiced=${isInvoiced}`;

    if (attendanceSheduleId != undefined)
      url = `${url}&attendanceSheduleId=${attendanceSheduleId}`

    if (isPrinted)
      url = `${url}&isPrinted=${isPrinted}`;

    return this.get<ResponseResult<VisitorPassModel[]>>(url);
  }


  getPavilionVisitors(searchModel: SearchRequestModel, eventDate?: string, pavilionName?: string, pavilionStatus?: boolean): Observable<ResponseResult<PavilionSessionVisitorModel[]>> {
    let url = `pavilion-sessions?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&searchTerm=${searchModel.searchTerm}`;

    if (pavilionName != undefined)
      url = `${url}&pavilionName=${pavilionName}`;

    if (eventDate != undefined)
      url = `${url}&eventDate=${eventDate}`

    if (pavilionStatus != undefined)
      url = `${url}&pavilionStatus=${pavilionStatus}`

    return this.get<ResponseResult<PavilionSessionVisitorModel[]>>(url);
  }

  cancelAttendenceSchedule(visitorRegistrationId: string, cancelVisitorAttendanceModel: CancelVisitorAttendanceModel) {
    return this.put(`registrations/${visitorRegistrationId}/cancel-attendance-schedules`, cancelVisitorAttendanceModel);
  }
}
