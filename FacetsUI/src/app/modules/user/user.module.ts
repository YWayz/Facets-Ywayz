import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserManagementComponent } from './user-management/user-management.component';
import { UserComponent } from './user-management/user-management-user/user-view/user.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { UserRoutingModule } from './user-routing.module';
import { UserPermissionViewComponent } from "./user-management/user-permission-view/user-permission-view.component";
import { RoleCreateComponent } from './user-management/user-roles/role-create/role-create.component';
import { RoleViewComponent } from './user-management/user-roles/role-view/role-view.component';
import { UserCreateComponent } from './user-management/user-management-user/user-create/user-create.component';
import { UserProfileComponent } from './user-management/user-profile/user-profile.component';

@NgModule({
    declarations: [
        UserManagementComponent,
        UserComponent,
        RoleCreateComponent,
        RoleViewComponent,
        UserCreateComponent,
        UserProfileComponent,
    ],
    imports: [
        CommonModule,
        UserRoutingModule,
        SharedModule,
        UserPermissionViewComponent
    ]
})
export class UserModule { }
