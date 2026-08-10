import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { VisitorSearchModel } from '../models/visitor-search.model';
import { VisitorModel } from '../models/visitor.model';
import { environment } from 'src/environments/environment';
import { HttpHeaders } from '@angular/common/http';
import { FileModel } from 'src/app/shared/models/file.model';
import { VisitorActivityModel } from '../models/visitor-activity.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { MarkAsBlacklistedModel } from '../models/mark-as-blacklisted.model';

@Injectable({
  providedIn: 'root'
})
export class VisitorsService extends BaseService {

  constructor() {
    super();
  }

  searchVisitor(searchValue: string): Observable<ResponseResult<VisitorSearchModel>> {
    return this.get<ResponseResult<VisitorSearchModel>>(`visitors/search?searchValue=${searchValue}`);
  }

  create(visitorModel: VisitorModel): Observable<ResponseResult<VisitorModel>> {
    return this.post<ResponseResult<VisitorModel>>('visitors', visitorModel);
  }

  getById(visitorId: string): Observable<ResponseResult<VisitorModel>> {
    return this.get<ResponseResult<VisitorModel>>(`visitors/${visitorId}`);
  }

  update(visitorId: string, visitorModel: VisitorModel) {
    return this.put(`visitors/${visitorId}`, visitorModel);
  }

  markAsBlacklisted(visitorId: string, markAsBlacklistedModel: MarkAsBlacklistedModel) {
    return this.put(`visitors/${visitorId}/mark-as-blacklisted`, markAsBlacklistedModel);
  }

  removeFromBlackList(visitorId: string) {
    return this.put(`visitors/${visitorId}/remove-from-blacklist`);
  }

  uploadDocument(visitorId: string, formData: FormData) {
    return this.http.post(`${environment.baseEndPoint}/visitors/${visitorId}/documents`, formData, {
      headers: new HttpHeaders({
        'enctype': 'multipart/form-data'
      })
    });
  }

  deleteAttachment(visitorId: string, id: string) {
    return this.delete(`visitors/${visitorId}/documents/${id}`);
  }

  getVisitorDocuments(visitorId: string, queryString?: string): Observable<ResponseResult<FileModel[]>> {
    return this.get<ResponseResult<FileModel[]>>(`visitors/${visitorId}/documents?${queryString}`);
  }

  getVisitorActivities(searchModel: SearchRequestModel, visitorId: string): Observable<ResponseResult<VisitorActivityModel[]>> {
    return this.get<ResponseResult<VisitorActivityModel[]>>(`visitors/${visitorId}/activities?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&searchQuery=${searchModel.searchTerm}`);
  }
}
