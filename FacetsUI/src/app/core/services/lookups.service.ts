import { Injectable } from '@angular/core';
import { BaseService } from "./base.service";
import { KeyValue } from "../models/key-value.model";
import { Observable, map, shareReplay } from "rxjs";
import { ResponseResult } from "../models/response-result.model";
import { SearchRequestModel } from "../models/search-request.model";
import { TimeZoneModel } from '../models/time-zone.model';
import { RegistrationCounterLookupModel } from 'src/app/shared/models/registration-counter-lookup.model';
import { PassCategoryType } from '../enums/pass-category-type.enum';
import { PassCategoryModel } from 'src/app/modules/event/models/pass-category.model';
import { PavilionSessionSummaryModel } from 'src/app/modules/visitor/models/pavilion-session-summary.model';

@Injectable({
  providedIn: 'root'
})
export class LookupsService extends BaseService {

  cachedTimezone$: Observable<ResponseResult<TimeZoneModel[]>>;

  constructor() {
    super();
  }

  getTimeZoneList(): Observable<ResponseResult<TimeZoneModel[]>> {
    if (!this.cachedTimezone$) {
      this.cachedTimezone$ = this.get<ResponseResult<TimeZoneModel[]>>('lookups/time-zones').pipe(
        map(response => response),
        shareReplay(1)
      );
    }
    return this.cachedTimezone$;
  }

  getAllEvents(searchModel: SearchRequestModel): Observable<ResponseResult<KeyValue<string, string>[]>> {
    return this.get<ResponseResult<KeyValue<string, string>[]>>(`lookups/Events?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}`)
      .pipe(
        map((response => response)),
      )
  }

  getAllRegistrationCounter(eventId: string, searchModel: SearchRequestModel, counterTypes?: string[], isLocked?: boolean): Observable<ResponseResult<RegistrationCounterLookupModel[]>> {
    let apiUrl = `lookups/events/${eventId}/registration-counter?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}`

    if (isLocked != null)
      apiUrl = `${apiUrl}&isLocked=${isLocked}`;

    if (counterTypes != null) {
      counterTypes.forEach(counterType => {
        apiUrl = `${apiUrl}&counterTypes=${counterType}`;
      });
    }


    return this.get<ResponseResult<RegistrationCounterLookupModel[]>>(apiUrl)
      .pipe(map((response => response)),
      )
  }

  //As Request Live eventId = 77D32DDB-D3E9-4EA2-C002-08DF09F52193 local :'c7c1b543-a444-4b1d-3f0c-08dcd9764f6b'
  getPassCategories(searchModel: SearchRequestModel, eventId: string, queryString?: string): Observable<ResponseResult<KeyValue<string, string>[]>> {
    return this.get<ResponseResult<KeyValue<string, string>[]>>(`lookups/events/${eventId}/pass-categories?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}${queryString}`)
      .pipe(
        map((response => response)),
      )
  }

  getAllPassCategories(searchModel: SearchRequestModel, eventId: string): Observable<ResponseResult<KeyValue<string, string>[]>> {
    return this.get<ResponseResult<KeyValue<string, string>[]>>(`lookups/events/${eventId}/pass-categories?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}`)
      .pipe(
        map((response => response)),
      )
  }

  getPassCategoryById(eventId: string, passCategoryId: string): Observable<ResponseResult<PassCategoryModel>> {
    return this.get<ResponseResult<PassCategoryModel>>(`lookups/events/${eventId}/pass-categories/${passCategoryId}`)
  }

  getCountries(): Observable<ResponseResult<KeyValue<string, string>[]>> {
    return this.get<ResponseResult<KeyValue<string, string>[]>>('lookups/countries');
  }

  getActiveAssignedEvents(): Observable<ResponseResult<KeyValue<string, string>[]>> {
    return this.get<ResponseResult<KeyValue<string, string>[]>>(`lookups/active-assigned-events?pageSize=100&pageNumber=1`)
      .pipe(
        map((response => response)),
      )
  }

  getAssignedEventUsers(eventId: string): Observable<ResponseResult<KeyValue<string, string>[]>> {
    return this.get<ResponseResult<KeyValue<string, string>[]>>(`lookups/events/${eventId}/assigned-users`);
  }

  getPavilions(eventId: string): Observable<ResponseResult<KeyValue<string, string>[]>> {
    return this.get<ResponseResult<KeyValue<string, string>[]>>(`lookups/events/${eventId}/pavilions`);
  }

  getPavilionSessions(eventId: string, pavilionId: string): Observable<ResponseResult<PavilionSessionSummaryModel[]>> {
    return this.get<ResponseResult<PavilionSessionSummaryModel[]>>(`lookups/events/${eventId}/pavilions/${pavilionId}/pavilion-sessions`);
  }
}