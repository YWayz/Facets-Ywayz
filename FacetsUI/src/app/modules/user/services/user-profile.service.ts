import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { UserProfileModel } from '../models/user-profile.model';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { UpdateUserPasswordModel } from '../models/update-user-password.model';
import { UpdateUserProfileModel } from '../models/update-user-profile.model';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService extends BaseService {

  constructor() {
    super();
  }

  getById(id: string): Observable<ResponseResult<UserProfileModel>> {
    return this.get<ResponseResult<UserProfileModel>>(`security/users/${id}/profile`);
  }

  updateProfile(id: string, userProfile: UserProfileModel) {
    return this.put(`security/users/${id}/profile`, userProfile);
  }

  changePassword(id: string, updateUserPasswordModel: UpdateUserPasswordModel) {
    return this.put(`security/users/${id}/profile/change-password`, updateUserPasswordModel);
  }

  uploadImage(id: string, files: File[]) {
    return this.filePost(`security/users/${id}/profile/image`, files)
  }

}
