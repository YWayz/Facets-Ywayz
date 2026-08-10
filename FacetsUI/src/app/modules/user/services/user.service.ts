import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { UserModel } from '../models/user.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService {

  constructor() {
    super();
  }

  create(userModel: UserModel): Observable<ResponseResult<UserModel>> {
    return this.post<ResponseResult<UserModel>>('security/users', userModel);
  }

  getAll(searchModel: SearchRequestModel): Observable<ResponseResult<UserModel[]>> {
    return this.get<ResponseResult<UserModel[]>>(`security/users?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&searchQuery=${searchModel.searchTerm}`);
  }

  getById(id: string): Observable<ResponseResult<UserModel>> {
    return this.get<ResponseResult<UserModel>>(`security/users/${id}`);
  }

  update(userModel: UserModel) {
    return this.put(`security/users/${userModel.id}`, userModel)
  }

  deleteUser(id: string): Observable<object> {
    return this.delete(`security/users/${id}`);
  }
}
