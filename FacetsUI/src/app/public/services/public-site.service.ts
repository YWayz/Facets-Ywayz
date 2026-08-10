import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { BaseService } from 'src/app/core/services/base.service';
import { PublicSiteEventSummaryModel } from '../models/public-site-event-summary.model';
import { VisitorVerificationModel } from '../models/visitor-verification.model';
import { VisitorSearchModel } from 'src/app/modules/visitor/models/visitor-search.model';
import { GenerateOTPModel } from '../models/generate-otp.model';
import { VerifyOTPModel } from '../models/verify-otp.model';
import { PublicUserAuthenticatedModel } from '../models/public-user-authenticated.model';
import { VisitorModel } from 'src/app/modules/visitor/models/visitor.model';
import { environment } from 'src/environments/environment';
import { HttpHeaders } from '@angular/common/http';
import { FileModel } from 'src/app/shared/models/file.model';
import { VisitorRegisterSummaryModel } from 'src/app/modules/visitor/models/visitor-register-summary.model';
import { VisitorRegisterModel } from 'src/app/modules/visitor/models/visitor-register.model';
import { VisitorEventRegistrationModel } from 'src/app/modules/visitor/models/visitor-event-registration.model';
import { CreatePaymentModel } from '../models/create-payment.model';
import { UpdateOnlineVisitorRegistrationModel } from '../models/update-online-visitor-registration.model';
import { UpdateVisitorModel } from '../models/update-visitor.model';
import { PaymentModel } from '../models/payment.model';
import { PassCategoryModel } from 'src/app/modules/event/models/pass-category.model';
import { PavilionSummaryModel } from 'src/app/modules/visitor/models/pavilion-summary.model';
import { appConstant } from 'src/app/core/extensions/app-constants';

@Injectable({
  providedIn: 'root'
})
export class PublicSiteService extends BaseService {

  constructor() {
    super();
  }

  getAllEvents(searchModel: SearchRequestModel): Observable<ResponseResult<PublicSiteEventSummaryModel[]>> {
    return this.get<ResponseResult<PublicSiteEventSummaryModel[]>>(`public/events?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}`);
  }

  visitorAvailabilityCheck(identificationNumber: string): Observable<ResponseResult<VisitorVerificationModel>> {
    return this.get<ResponseResult<VisitorVerificationModel>>(`public/visitors/${identificationNumber}/availability-check`);
  }

  searchVisitor(searchValue: string): Observable<ResponseResult<VisitorSearchModel>> {
    return this.get<ResponseResult<VisitorSearchModel>>(`public/visitors/search?searchValue=${searchValue}`);
  }

  getById(visitorId: string): Observable<ResponseResult<VisitorModel>> {
    return this.get<ResponseResult<VisitorModel>>(`public/visitors/${visitorId}`);
  }

  generateOtp(generateOtpModel: GenerateOTPModel): Observable<ResponseResult<string>>  {
    return this.post<ResponseResult<string>>(`public/otp-manager/send`, generateOtpModel);
  }

  verifyOtp(verifyOTPModel: VerifyOTPModel): Observable<ResponseResult<PublicUserAuthenticatedModel>> {
    return this.post<ResponseResult<PublicUserAuthenticatedModel>>(`public/otp-manager/verify`, verifyOTPModel);
  }

  uploadDocument(visitorId: string, formData: FormData) {
    return this.http.post(`${environment.baseEndPoint}/public/visitors/${visitorId}/documents`, formData, {
      headers: new HttpHeaders({
        'enctype': 'multipart/form-data'
      })
    });
  }

  getVisitorDocuments(visitorId: string, queryString?: string): Observable<ResponseResult<FileModel[]>> {
    return this.get<ResponseResult<FileModel[]>>(`public/visitors/${visitorId}/documents?${queryString}`);
  }

  getRegistrations(searchModel: SearchRequestModel, visitorId: string, countryId?: string, visitorStatus?: string): Observable<ResponseResult<VisitorRegisterSummaryModel[]>> {
    let url = `public/registrations?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&searchQuery=${searchModel.searchTerm}`

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

  getVisitorRegistration(visitorId: string): Observable<ResponseResult<VisitorRegisterModel>> {
    return this.get<ResponseResult<VisitorRegisterModel>>(`public/visitors/${visitorId}/registration`);
    // return this.get<ResponseResult<VisitorRegisterModel>>(`public/visitors/${visitorId}`);
  }

  registerNewVisitor(visitorModel: VisitorModel): Observable<ResponseResult<VisitorModel>> {
    return this.post<ResponseResult<VisitorModel>>('public/visitors', visitorModel);
  }

  updateVisitor(visitorId: string, updateVisitorModel: UpdateVisitorModel) {
    return this.put(`public/visitors/${visitorId}`, updateVisitorModel);
  }

  create(visitorEventRegistrationModel: VisitorEventRegistrationModel): Observable<ResponseResult<VisitorRegisterModel>> {
    return this.post<ResponseResult<VisitorRegisterModel>>('public/registrations', visitorEventRegistrationModel);
  }

  updateVisitorRegistration(visitorRegistrationId: string, updateOnlineVisitorRegistrationModel: UpdateOnlineVisitorRegistrationModel) {
    return this.put(`public/registrations/${visitorRegistrationId}`, updateOnlineVisitorRegistrationModel);
  }

  createPayment(createPaymentModel: CreatePaymentModel): Observable<ResponseResult<PaymentModel>> {
    return this.post<ResponseResult<PaymentModel>>('public/payments', createPaymentModel);
  }

  getPassCategory(eventId: string, id: string): Observable<ResponseResult<PassCategoryModel>> {
    return this.get<ResponseResult<PassCategoryModel>>(`public/events/${eventId}/pass-categories/${id}`);
  }
  
  getAllPavilions(): Observable<ResponseResult<PavilionSummaryModel[]>> {
    return this.get<ResponseResult<PavilionSummaryModel[]>>(`public/events/${localStorage.getItem(appConstant.selectedEventId)}/pavilions`);
  }
}
