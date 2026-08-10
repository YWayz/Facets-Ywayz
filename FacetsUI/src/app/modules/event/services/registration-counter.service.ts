import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { RegistrationCounterModel } from '../models/registration-counter.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { Observable } from 'rxjs';
import { CounterAssignmentStatusModel } from '../models/counter-assignment-status.model';
import { UserAssignedRegistrationCounterModel } from '../../visitor/models/user-assigned-registration-counter.model';
import { AssignRegistrationCounterModel } from '../../visitor/models/assigned-registration-counter.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';

@Injectable({
  providedIn: 'root'
})
export class RegistrationCounterService extends BaseService {

  constructor() { 
    super();
  }

  create(eventId: string, registrationCounterModel: RegistrationCounterModel): Observable<ResponseResult<RegistrationCounterModel>> {
    return this.post<ResponseResult<RegistrationCounterModel>>(`events/${eventId}/registration-counter`, registrationCounterModel);
  }

  createRegistrationCounterAssignmment(counterId: string, assignRegistrationCounterModel: AssignRegistrationCounterModel): Observable<ResponseResult<UserAssignedRegistrationCounterModel>> {
    return this.post<ResponseResult<UserAssignedRegistrationCounterModel>>(`registration-counters/${counterId}/counter-assignment`, assignRegistrationCounterModel);
  }

  update(eventId: string, id: string, registrationCounterModel: RegistrationCounterModel) {
    return this.put(`events/${eventId}/registration-counter/${id}`, registrationCounterModel);
  }
  
  unLockCounter(eventId: string, id: string) {
    return this.put(`events/${eventId}/registration-counter/${id}/unlock`, {});
  }

  getAll(eventId: string, searchModel: SearchRequestModel, status: boolean | undefined): Observable<ResponseResult<RegistrationCounterModel[]>> {
    const isLocked = status ?? '';
    return this.get<ResponseResult<RegistrationCounterModel[]>>(`events/${eventId}/registration-counter?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&isLocked=${isLocked}`);
  }

  getRegistrationCounter(eventId: string, id: string): Observable<ResponseResult<RegistrationCounterModel>> {
    return this.get<ResponseResult<RegistrationCounterModel>>(`events/${eventId}/registration-counter/${id}`);
  }

  deleteRegistrationCounter(eventId: string, id: string) {
    return this.delete(`events/${eventId}/registration-counter/${id}`);
  }

  registrationCounterAssignment(counterTypes?: string[]) {
    let apiUrl = "registration-counters/check-assignment?";

    if (counterTypes != null) {
      counterTypes.forEach(counterType => {
        apiUrl = `${apiUrl}&counterTypes=${counterType}`;
      });
    }
    return this.get<ResponseResult<CounterAssignmentStatusModel>>(apiUrl);
  }
}
