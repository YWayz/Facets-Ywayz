import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { UserClaimModel, UserRoleModel } from '../../models/user-role.model';
import { PermissionListModel, PermissionModel } from '../../models/user-permission.model';
import { UpdateUserRoleClaimModel } from '../../models/update-user-role-claims.model';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { UserPermissionService } from '../../services/user-permission.service';
import { MaterialModule } from 'src/app/shared/material.module';

@Component({
  selector: 'facets-user-permission-view',
  templateUrl: './user-permission-view.component.html',
  styleUrls: ['./user-permission-view.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule, MaterialModule]
})
export class UserPermissionViewComponent implements OnInit, OnChanges {

  userRole: UserRoleModel;
  displayStyle = 'none';
  isAllPermissionChecked = false;
  permissionModels: PermissionModel[];
  permissionNames = new Array<string>();
  updatePermissionModel = new UpdateUserRoleClaimModel();

  @Input() roleId: string = '';
  @Input() isPermissionSelection: boolean;
  @Input() isView: boolean;
  @Input() isEdit: boolean;
  @Input() isPermissionEdit: boolean;
  @Input() selectedPermission = new Array<string>();
  @Input() retrievedPermission = new Array<UserClaimModel>();
  @Output() selectedPermissionNames = new EventEmitter<string[]>();
  @Output() isPopupClosed = new EventEmitter<boolean>();

  constructor(private toasterService: ToasterService, private userPermissionService: UserPermissionService) {

  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((this.roleId != '' && this.roleId != undefined ) && !this.isPermissionEdit) {
      this.getUserRolePermissions(this.roleId);
    } else if (this.roleId != '' && this.isPermissionEdit) {
      this.getUserRoleByUser();
    }

    if (this.isPermissionSelection) {
      this.getSelectedTemplates();
    }

    if (this.retrievedPermission.length != 0) {
      this.getUserRoleByUser();
    }
  }

  ngOnInit(): void {
    this.getUserPermissions();
  }

  makeAllPermissionFalse() {
    this.permissionModels.forEach((f) => {
      f.isAllPermission = false;
    });

    this.permissionNames = [];
    this.selectedPermissionNames.emit(this.permissionNames);
  }

  getUserPermissions() {
    this.userPermissionService.getAll().subscribe({
      next: (permissions: ResponseResult<PermissionModel[]>) => {
        this.permissionModels = permissions.data;
        if (this.userRole != undefined)
          this.getUserRolePermissions(this.userRole.roleId);
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      },
    });
  }

  getUserRoleByUser() {
    this.permissionNames = [];
    this.selectedPermission = [];
    this.retrievedPermission.forEach((userClaim) =>
      this.permissionNames.push(userClaim.claimValue)
    );
    this.selectedPermission.push(...this.permissionNames);
    this.selectedPermissionNames.emit(this.permissionNames);
    this.getSelectedTemplates();
  }

  getSelectedTemplates() {
    this.permissionModels.forEach((permissionModel) => {
      permissionModel.value.forEach((f) => (f.isSelected = false));
      this.selectedPermission.forEach((permissionName) => {
        let selectedPermiss = permissionModel.value.find(
          (f) => f.key == permissionName
        );
        if (selectedPermiss != undefined) {
          permissionModel.value.find(
            (f) => f.key == permissionName
          )!.isSelected = true;
        }
      });
    });
  }

  getUserRolePermissions(roleId: string) {
    this.retrievedPermission = [];
    this.userPermissionService.getPermissionsByRole(roleId).subscribe({
      next: (userPermissions: ResponseResult<UserRoleModel>) => {
        this.permissionNames = [];
        this.permissionModels.forEach((f) => {
          userPermissions.data.claims.forEach((e) => {
            let selectedPermission = f.value.find(
              (sp) => sp.key == e.claimValue
            );
            if (selectedPermission != undefined) {
              f.value.find((sp) => sp.key == e.claimValue)!.isSelected = true;
              this.permissionNames.push(selectedPermission.key);
              this.retrievedPermission.push(e);
            }
          });
        });
        this.selectedPermission.push(...this.permissionNames);
        this.selectedPermissionNames.emit(this.permissionNames);
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      },
    });
  }

  selectedPermissions(
    event: any,
    permissionListModel: PermissionListModel,
    permissionKey: string
  ) {

    let checked = event.target.checked;

    if (checked) {
      this.permissionNames.push(permissionListModel.key);
      var permission = this.permissionModels.filter(s => s.key == permissionKey);

      permission.map((permissionModel: PermissionModel) => {

        let permissionContainCount = 0;
        this.permissionNames.forEach(element => {
          if (element.split('.')[0].toLowerCase().startsWith(permissionKey.toLowerCase().replace(' ', ''))) {
            permissionContainCount += 1
          }
          else {
            permissionModel.isAllPermission = false;
          }
        });

        if (permissionContainCount == permissionModel.value.length) {
          permissionModel.isAllPermission = true;
        }
        else {
          permissionModel.isAllPermission = false;
        }
      })
      this.selectedPermissionNames.emit(this.permissionNames);

    } else {
      const index = this.permissionNames.indexOf(permissionListModel.key);
      this.permissionNames.splice(index, 1);

      var permission = this.permissionModels.filter(s => s.key == permissionKey);

      permission.map((permissionModel: PermissionModel) => {
        let permissionContainCount = 0;

        this.permissionNames.forEach(element => {
          if (element.split('.')[0].toLowerCase().startsWith(permissionKey.toLowerCase().replace(' ', ''))) {
            permissionContainCount += 1
          }
          else {
            permissionModel.isAllPermission = false;
          }
        });

        if (permissionContainCount == permissionModel.value.length) {
          permissionModel.isAllPermission = true;
        }
        else {
          permissionModel.isAllPermission = false;
        }
      })

      this.selectedPermissionNames.emit(this.permissionNames);
    }
  }

  changePermissionList(event: any, permissionModelName: string) {
    let checked = event.target.checked;

    const permissionModelList = this.permissionModels.find(
      (f) => f.key == permissionModelName
    );

    permissionModelList?.value.forEach((f) => {
      if (checked) {
        this.permissionNames.push(f.key);
        this.selectedPermissionNames.emit(this.permissionNames);
      } else {
        const index = this.permissionNames.indexOf(f.key);
        this.permissionNames.splice(index, 1);
        this.selectedPermissionNames.emit(this.permissionNames);
      }
    });

    permissionModelList!.isAllPermission = checked;
  }
}
