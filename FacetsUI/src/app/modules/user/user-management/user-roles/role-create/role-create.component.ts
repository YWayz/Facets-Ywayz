import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild, ViewChildren } from '@angular/core';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { Observable, fromEvent, merge } from 'rxjs';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { UserPermissionViewComponent } from '../../user-permission-view/user-permission-view.component';
import { UserRoleModel } from '../../../models/user-role.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { UserRoleService } from '../../../services/user-role.service';
import { UserPermissionService } from '../../../services/user-permission.service';
import { UpdateUserRoleModel } from '../../../models/update-user-role.model';
import { RolePermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';

@Component({
  selector: 'facets-role-create',
  templateUrl: './role-create.component.html',
  styleUrls: ['./role-create.component.scss']
})
export class RoleCreateComponent implements OnInit, OnChanges, AfterViewInit {

  userRoleForm: FormGroup;
  userRoleModel: UserRoleModel;

  isFormSubmitted = false;
  isBlocked = false;
  displayStyle = "none";

  superAdminPermissions = SuperAdminPermissions;
  rolePermissions = RolePermissions;
    
  @Input() isEdit = false;
  @Input() roleId: string;
  @Output() rolesUpdatedList = new EventEmitter<boolean>() 
  validationModel: ValidationModel = new ValidationModel();
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];
  @ViewChild('userPermissionComponent') userPermissionComponent: UserPermissionViewComponent;

  constructor(private formBuilder: FormBuilder, 
    private userRoleService: UserRoleService, 
    private userPermissionService: UserPermissionService, 
    private toasterService: ToasterService) { 
    this.validationModel.validationMessages = {
      roleName: {
        required: 'Name is required'
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.isEdit) {
      this.getUserRole(this.roleId)
      this.openEditPopup()
    }
  }

  ngOnInit(): void {
    this.userRoleCreateForm();
  }
  
  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.userRoleForm.valueChanges, ...controlBlurs).subscribe(value => {
      this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.userRoleForm, this.isFormSubmitted);
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.userRoleForm, this.isFormSubmitted);
  }

  userRoleCreateForm() {
    this.userRoleForm = this.formBuilder.group({
      roleName: ['', Validators.required]
    });
  }
  
  openPopup() {
    this.isFormSubmitted = false;
    this.validate();
    this.userRoleCreateForm();
    this.displayStyle = "block";
    this.userPermissionComponent.makeAllPermissionFalse();
    this.userPermissionComponent.displayStyle = "block";
    this.userPermissionComponent.isView = false
  }
  
  
  getUserRole(roleId: string) {
    this.isBlocked = true;
    this.userRoleService.getById(roleId).subscribe({
      next: (res: ResponseResult<UserRoleModel>) => {
        this.userRoleModel = res.data;
        this.userPermissionComponent.getUserRolePermissions(this.userRoleModel.roleId)
        this.userPermissionComponent.isView = true
        this.isBlocked = false;

        this.userRoleForm.patchValue(this.userRoleModel)
      },
      error: (error: ErrorResponse) => {
        console.error(error);
      }
    })
  }

  closePopup() {
    this.displayStyle = "none";
    this.rolesUpdatedList.emit(true);
  }

  openEditPopup() {
    this.displayStyle = "block";
  }

  createUserRole() {
    this.isFormSubmitted = true;
    this.validate();

    if(this.userRoleForm.invalid) return;
    if(this.userPermissionComponent.permissionNames.length == 0) return this.toasterService.warning("Permissions cannot be empty", "User role creation");

    this.isBlocked = true;
    this.userRoleModel = Object.assign({}, this.userRoleModel, this.userRoleForm.value);
    this.userRoleService.create(this.userRoleModel).subscribe({
      next: (userRoleModel: ResponseResult<UserRoleModel>) => {
        this.isBlocked = true;
        this.updateUserPermission(userRoleModel.data.roleId)
        this.isFormSubmitted = false;
        this.isBlocked = false;
        this.rolesUpdatedList.emit(true);
        this.closePopup();
        this.userRoleForm.reset();
        this.toasterService.successfullyCreated("User role");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    })
  }

  updateUserRole() {
    this.isFormSubmitted = true;
    this.validate();

    if(this.userRoleForm.invalid) return;
    if(this.userPermissionComponent.permissionNames.length == 0) return this.toasterService.warning("Permissions cannot be empty", "User role creation");

    this.isBlocked = true;
    this.userRoleModel = Object.assign({}, this.userRoleModel, this.userRoleForm.value);
    
    this.userPermissionComponent.updatePermissionModel.permissions = this.userPermissionComponent.permissionNames;
    this.userPermissionComponent.updatePermissionModel.roleName =this.userRoleModel.roleName;

    this.userRoleService.updateRole(this.roleId, this.userPermissionComponent.updatePermissionModel).subscribe({
      next: () => {
        this.isBlocked = true;
        this.isFormSubmitted = false;
        this.isBlocked = false;
        this.userPermissionComponent.permissionNames = [];
        this.rolesUpdatedList.emit(true);
        this.closePopup();
        this.userRoleForm.reset();
        this.toasterService.successfullyUpdated("User role");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    })
  }

  updateUserPermission(roleId: string) {
    this.userPermissionComponent.updatePermissionModel.permissions = this.userPermissionComponent.permissionNames;
    this.userPermissionComponent.updatePermissionModel.roleName =this.userRoleModel.roleName;
    this.userPermissionService.update(roleId, this.userPermissionComponent.updatePermissionModel).subscribe({
      next: () => {
        this.userPermissionComponent.permissionNames = [];
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    });
  }
}
