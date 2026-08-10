import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { BaseService } from 'src/app/core/services/base.service';
import { LoginModel } from '../models/login.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { isNull } from 'src/app/core/extensions/helpers';
import { ResetPasswordModel } from '../models/reset-password.model';
import { UserModel } from '../../user/models/user.model';
import { LogoutModel } from '../models/logout.model';


@Injectable({
  providedIn: 'root',
})
export class AuthService extends BaseService {

  authenticationData: UserModel;

  constructor(private router: Router) {
    super();
  }

  login(loginModel: LoginModel): Observable<ResponseResult<string>> {
    return this.post<ResponseResult<string>>('security/authenticate', loginModel);
  }

  logout(logoutModel: LogoutModel): Observable<ResponseResult<LoginModel>> {
    return this.post<ResponseResult<LoginModel>>('security/logout', logoutModel);
  }

  forgotPassword(resetPasswordModel: ResetPasswordModel) {
    return this.post<ResponseResult<string>>('security/forgot-password', resetPasswordModel);
  }

  resetPassword(resetPasswordModel: ResetPasswordModel) {
    return this.post<ResponseResult<string>>('security/reset-password', resetPasswordModel);
  }

  hasPermissionAuthorization(rolePermission: Array<string>) {
    const permissionClaims = JSON.parse(localStorage.getItem('claims')!);

    if (permissionClaims == null) {
      this.router.navigate(['login']);
      return false;
    }

    if (rolePermission != null || rolePermission != undefined) {
      if (rolePermission && rolePermission.find(role => {
        const isAvailable = permissionClaims.includes(role);
        return isAvailable;
      })) {
        return true;
      }
    }
    return false;
  }

  public get token(): string | null {
    let data = localStorage.getItem(appConstant.jwtTokenName);
    let token = '';

    if (!isNull(data))
      token = JSON.parse(data!).bearerToken;
    return token;
  }

  public get userId(): string | null {
    let data = localStorage.getItem(appConstant.jwtTokenName);
    let userId = '';

    if (!isNull(data))
      userId = JSON.parse(data!).userId;
    return userId;
  }

  public get authData(): UserModel | null {
    let data = localStorage.getItem(appConstant.jwtTokenName);

    if (!isNull(data))
      this.authenticationData = JSON.parse(data!);
    return this.authenticationData;
  }

  public get isAuthenticated(): boolean {
    return this.token !== null;
  }
}
