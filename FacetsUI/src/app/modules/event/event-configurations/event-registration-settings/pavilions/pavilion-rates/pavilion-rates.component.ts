import { Component, Inject, OnInit, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { PassCategoryPermissions, PavilionPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { ModalService } from 'src/app/core/services/modal.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { PassCategoryRatesModel } from 'src/app/modules/event/models/pass-category-rates.model';
import { PavilionModel } from 'src/app/modules/event/models/pavilion.model';
import { UpdatePassCategoryPavilionRateModel } from 'src/app/modules/event/models/update-pass-category-pavilion-rate';
import { UpdatePassCategoryPavilionRateItemModel } from 'src/app/modules/event/models/update-pass-category-pavilion-rate-item.model';
import { PassCategoriesPavilionSettingsService } from 'src/app/modules/event/services/pass-categories-pavilion-settings.service';
import { PassCategorySettingsService } from 'src/app/modules/event/services/pass-category-settings.service';
import { PavilionService } from 'src/app/modules/event/services/pavilion.service';

@Component({
  selector: 'facets-pavilion-rates',
  templateUrl: './pavilion-rates.component.html',
  styleUrls: ['./pavilion-rates.component.scss']
})
export class PavilionRatesComponent implements OnInit {
  isBlocked = false;

  superAdminPermissions = SuperAdminPermissions;
  pavilionPermissions = PavilionPermissions;

  pavilionModel: PavilionModel;
  searchModel = new SearchRequestModel(10000, 1);

  updatePassCategoryPavilionRateItemModels = new Array<UpdatePassCategoryPavilionRateItemModel>();
  updatePassCategoryPavilionRateModel = new UpdatePassCategoryPavilionRateModel();

  dialog = inject(MatDialog);
  passCategoriesPavilionSettingsService = inject(PassCategoriesPavilionSettingsService);
  pavilionService = inject(PavilionService);
  toasterService = inject(ToasterService);
  modalService = inject(ModalService);
  authService = inject(AuthService);
  activatedRoute = inject(ActivatedRoute);

  dialogRef = inject(MatDialogRef<PavilionRatesComponent>);

  constructor(@Inject(MAT_DIALOG_DATA) public dialogData: { data: { id: string, isEdit: boolean, eventId: string } }) {
  }

  ngOnInit(): void {
    if (this.dialogData.data.eventId != '' || this.dialogData.data.eventId != undefined) {
      if (this.authService.hasPermissionAuthorization([this.superAdminPermissions.all, this.pavilionPermissions.rateView])) {
        this.getPavilions();
      }
    }
  }

  getPavilions() {
    this.isBlocked = true;
    this.pavilionService.getPavilionById(this.dialogData.data.eventId, this.dialogData.data.id).subscribe({
      next: (result: ResponseResult<PavilionModel>) => {
        this.isBlocked = false;
        this.pavilionModel = result.data;
        this.pavilionModel.passCategoryPavilionSettings = this.pavilionModel.passCategoryPavilionSettings.filter(t=>t.passType == 'Visitor')
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  update() {
    this.isBlocked = true;
    this.pavilionModel.passCategoryPavilionSettings.forEach(passCategory => {
      this.updatePassCategoryPavilionRateItemModels.push({ passCategoryId: passCategory.passCategoryId, passCategoryPavilionSettingsId: passCategory.id, pavilionRate: passCategory.pavilionRate })
    });
    
    this.updatePassCategoryPavilionRateModel.pavilionRates = this.updatePassCategoryPavilionRateItemModels;
    this.passCategoriesPavilionSettingsService.update(this.dialogData.data.eventId, this.dialogData.data.id, this.updatePassCategoryPavilionRateModel).subscribe({
      next: () => { 
        this.toasterService.successfullyUpdated("Pavilion Rates");
        this.updatePassCategoryPavilionRateModel = new UpdatePassCategoryPavilionRateModel();
        this.updatePassCategoryPavilionRateItemModels = [];
        this.dialogRef.close();
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
        this.updatePassCategoryPavilionRateModel = new UpdatePassCategoryPavilionRateModel();
        this.updatePassCategoryPavilionRateItemModels = new Array<UpdatePassCategoryPavilionRateItemModel>();
        this.isBlocked = false;
      },
    })
  }

  discard() {
    this.dialogRef.close();
  }
}
