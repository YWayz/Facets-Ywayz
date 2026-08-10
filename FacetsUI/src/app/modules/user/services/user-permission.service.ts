import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { UserRoleModel } from '../models/user-role.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { Observable } from 'rxjs';
import { PermissionModel } from '../models/user-permission.model';
import { UpdateUserRoleClaimModel } from '../models/update-user-role-claims.model';

@Injectable({
  providedIn: 'root'
})
export class UserPermissionService extends BaseService {

  constructor() {
    super();
  }

  getAll(): Observable<ResponseResult<PermissionModel[]>> {
    return this.get<ResponseResult<PermissionModel[]>>(`security/app-permissions`);
  }

  getPermissionsByRole(roleId: string) {
    return this.get<ResponseResult<UserRoleModel>>(`security/roles/${roleId}/permission-templates`);
  }

  update(roleId: string, updateRoleClaim: UpdateUserRoleClaimModel) {
    return this.put(`security/roles/${roleId}`, updateRoleClaim);
  }
}
