import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { UserRoleModel } from '../models/user-role.model';
import { UpdateUserRoleModel } from '../models/update-user-role.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { UpdateUserRoleClaimModel } from '../models/update-user-role-claims.model';

@Injectable({
  providedIn: 'root'
})
export class UserRoleService extends BaseService {

  constructor() {
    super();
  }

  create(userRoleModel: UserRoleModel): Observable<ResponseResult<UserRoleModel>> {
    return this.post<ResponseResult<UserRoleModel>>('security/roles', userRoleModel);
  }

  getAll(searchRequestModel: SearchRequestModel): Observable<ResponseResult<UserRoleModel[]>> {
    return this.get<ResponseResult<UserRoleModel[]>>(`security/roles?searchQuery=${searchRequestModel.searchTerm}`);
  }
  
  getById(roleId: string): Observable<ResponseResult<UserRoleModel>> {
    return this.get<ResponseResult<UserRoleModel>>(`security/roles/${roleId}/permission-templates`);
  }
  
  updateRole(roleId: string, updateUserRole: UpdateUserRoleClaimModel) {
    return this.put(`security/roles/${roleId}`, updateUserRole);
  }
  
  deleteRole(roleId: string) {
    return this.delete(`security/roles/${roleId}`);
  }
}
