import { Component, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { UserRoleModel } from '../../../models/user-role.model';
import { UserPermissionViewComponent } from '../../user-permission-view/user-permission-view.component';
import { RoleCreateComponent } from '../role-create/role-create.component';
import { SubSink } from 'subsink';
import { UserRoleService } from '../../../services/user-role.service';
import { isEmpty } from 'src/app/core/extensions/helpers';
import { UserPermissionService } from '../../../services/user-permission.service';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { ModalService } from 'src/app/core/services/modal.service';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { RolePermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';

@Component({
  selector: 'facets-role-view',
  templateUrl: './role-view.component.html',
  styleUrls: ['./role-view.component.scss']
})
export class RoleViewComponent implements OnInit {

  userRoles: UserRoleModel[];
  isBlocked = false;
  isEdit = false;
  roleId: string;
  displayStyle = "none";

  superAdminPermissions = SuperAdminPermissions;
  rolePermissions = RolePermissions;

  private subs = new SubSink();
  searchModel = new SearchRequestModel(10, 1);

  resultsEmpty$ = new BehaviorSubject<boolean>(false);

  @ViewChild('userPermissionComponent') userPermissionComponent: UserPermissionViewComponent;
  @ViewChild('userRoleCreateComponent') userRoleCreateComponent: RoleCreateComponent;

  constructor(
    private userRoleService: UserRoleService,
    private userPermissionService: UserPermissionService,
    private toasterService: ToasterService, private modalService: ModalService) { }

  ngOnInit(): void {
    this.getUserRoles();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  openPopup() {
    this.userPermissionComponent.getUserPermissions();
    this.displayStyle = "block";
  }

  closePopup() {
    this.displayStyle = "none";
  }

  editRole(roleId: string) {
    this.isEdit = true;
    this.roleId = roleId;
  }

  deleteRole(roleId: string) {
    let options = {
      title: 'Delete User Role',
      message: 'Are you sure you want to delete the user role ?',
    };
    this.modalService.displayDialog(ConfirmationModalComponent, options)
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.userRoleService.deleteRole(roleId)
          .subscribe({
            next: (res: any) => {
              this.isBlocked = false;
              this.toasterService.success("User role has been successfully deleted");
              this.getUserRoles();
            },
            error: (err: ErrorResponse) => {
              this.isBlocked = false;
              this.toasterService.error(err);
            }
          })
      } 
    });
  }

  getUserRoles() {
    this.isBlocked = true;
    this.subs.sink = this.userRoleService.getAll(this.searchModel).subscribe({
      next: (roles: ResponseResult<UserRoleModel[]>) => {
        this.userRoles = roles.data;
        if (!roles.success || !roles.totalRecordCount) {
          this.resultsEmpty$.next(true)
          this.isBlocked = false;
          return
        } else {
          this.resultsEmpty$.next(false)
        }
        this.isBlocked = false;
        this.isEdit = false
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  search() {
    if (!isEmpty(this.searchModel.searchTerm) && this.searchModel.searchTerm.length > 2)
      this.getUserRoles();
    else if (isEmpty(this.searchModel.searchTerm)) {
      this.getUserRoles();
    }
  }

  clearSearchTerm() {
    this.searchModel.searchTerm = '';
    this.search();
  }

  viewPermission(userRole: UserRoleModel) {
    this.userPermissionComponent.isView = true;
    this.userPermissionComponent.isPermissionEdit = false;
    this.userPermissionComponent.userRole = userRole;
    this.openPopup();
  }

  viewCreateRole() {
    this.userRoleCreateComponent.openPopup();
  }

  viewEditRole() {
    this.userRoleCreateComponent.openPopup();
  }
}
