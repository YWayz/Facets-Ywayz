import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserManagementComponent } from './user-management/user-management.component';
import { UserProfileComponent } from './user-management/user-profile/user-profile.component';
import { authGuard } from 'src/app/core/guards/auth.guard';
import { RolePermissions, SuperAdminPermissions, UserPermissions } from 'src/app/core/extensions/permission-constants';

const routes: Routes = [
  {
    path: '',
    component: UserManagementComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, RolePermissions.create, RolePermissions.update, RolePermissions.view, RolePermissions.delete, RolePermissions.updateRoleClaim, UserPermissions.create, UserPermissions.edit, UserPermissions.view, UserPermissions.delete
      ]
    }
  },
  {
    path: 'user-profile',
    component: UserProfileComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule { }
