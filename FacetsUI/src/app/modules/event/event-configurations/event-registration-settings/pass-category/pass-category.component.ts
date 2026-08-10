import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PassCategoryCreateComponent } from './pass-category-create/pass-category-create.component';
import { PassCategoryService } from '../../../services/pass-category.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { PassCategoryModel } from '../../../models/pass-category.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { ModalService } from 'src/app/core/services/modal.service';
import { ActivatedRoute, Params } from '@angular/router';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { PassCategoryPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { UpdateVisitorPassCategoryTypeModel } from '../../../models/update-visitor-pass-catery-type.model';

@Component({
  selector: 'facets-pass-category',
  templateUrl: './pass-category.component.html',
  styleUrls: ['./pass-category.component.scss']
})
export class PassCategoryComponent implements OnInit {

  isBlocked = false;
  eventId = '';

  superAdminPermissions = SuperAdminPermissions;
  passCategoryPermissions = PassCategoryPermissions;

  passCategoryModels: PassCategoryModel[];

  dialog = inject(MatDialog);
  passCategoryService = inject(PassCategoryService);
  toasterService = inject(ToasterService);
  modalService = inject(ModalService);
  authService = inject(AuthService);
  activatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((param: Params) => {
      this.eventId = param['eventId'];
      if (this.eventId != null || this.eventId != undefined || this.eventId != '') {
        this.getPassCategories();
      }
    });
  }

  getPassCategories() {
    this.isBlocked = true;
    this.passCategoryService.getAll(this.eventId).subscribe({
      next: (result: ResponseResult<PassCategoryModel[]>) => {
        this.isBlocked = false;
        this.passCategoryModels = result.data;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  openCreatePopup(id?: string) {
    this.modalService.displayDialog(PassCategoryCreateComponent, {
      data: {
        id: id,
        eventId: this.eventId,
        isEdit: id == undefined ? false : true
      }
    });
    this.modalService.confirmed().subscribe(() => this.getPassCategories());
  }

  deleteCategory(id: string) {
    let options = {
      title: 'Delete Pass Category',
      message: 'Are you sure you want to delete this pass category?',
    };
    this.modalService.displayDialog(ConfirmationModalComponent, options);
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.passCategoryService.deleteCategory(this.eventId, id).subscribe({
          next: () => {
            this.isBlocked = false;
            this.getPassCategories();
            this.toasterService.successfullyDeleted("Pass category");
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.toasterService.error(err);
          }
        });
      }
    });
  }

  passCategoryTypeStatusUpdate(event: any, passCategoryId: string, eventId: string) {
    event.preventDefault();
    this.isBlocked = true;
    let visitorPassCategoryType = event.target?.checked == true ? 'CommonPass' : 'PerDayPass';

    const updateVisitorPassCategoryTypeModel = new UpdateVisitorPassCategoryTypeModel();
    updateVisitorPassCategoryTypeModel.visitorPassCategoryType = visitorPassCategoryType

    this.passCategoryService.updatePassCategoryType(eventId, passCategoryId, updateVisitorPassCategoryTypeModel).subscribe({
      next: () => {
        this.isBlocked = false;
        this.getPassCategories();
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }
}
