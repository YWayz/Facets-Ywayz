import { Component, inject } from '@angular/core';
import { RolePermissions, SuperAdminPermissions, UserPermissions } from 'src/app/core/extensions/permission-constants';
import { AuthService } from '../../auth/services/auth.service';

@Component({
  selector: 'facets-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent {
  
  superAdminPermissions = SuperAdminPermissions;
  rolePermissions = RolePermissions;
  userPermissions = UserPermissions;

  isUserHidePermission = false;
  isUserRoleHidePermission = false;

  authService = inject(AuthService);

  ngOnInit(): void {
    if(!this.authService.hasPermissionAuthorization(['all', 'user.view'])) {
      this.isUserHidePermission = true;
    }

    if(!this.authService.hasPermissionAuthorization(['all', 'role.view'])) {
      this.isUserRoleHidePermission = true;
    }
  }
}
