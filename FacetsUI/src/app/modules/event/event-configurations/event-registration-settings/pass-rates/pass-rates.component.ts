import { Component, OnInit, inject } from '@angular/core';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { MatDialog } from '@angular/material/dialog';
import { PassRatesUpdateComponent } from './pass-rates-update/pass-rates-update.component';
import { PassCategorySettingsService } from '../../../services/pass-category-settings.service';
import { ModalService } from 'src/app/core/services/modal.service';
import { PassCategoryRatesModel } from '../../../models/pass-category-rates.model';
import { ActivatedRoute, Params } from '@angular/router';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { PassCategoryPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';

@Component({
  selector: 'facets-pass-rates',
  templateUrl: './pass-rates.component.html',
  styleUrls: ['./pass-rates.component.scss']
})
export class PassRatesComponent implements OnInit {

  isBlocked = false;
  eventId = '';

  superAdminPermissions = SuperAdminPermissions;
  passCategoryPermissions = PassCategoryPermissions;

  passCategoryRatesModel: PassCategoryRatesModel[];

  dialog = inject(MatDialog);
  passCategorySettingsService = inject(PassCategorySettingsService);
  toasterService = inject(ToasterService);
  modalService = inject(ModalService);
  authService = inject(AuthService);
  activatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((param: Params) => {
      this.eventId = param['eventId'];
      if (this.eventId != null || this.eventId != undefined || this.eventId != '') {
        this.getPassRates();
      }
    });
  }

  getPassRates() {
    this.isBlocked = true;
    this.passCategorySettingsService.getAll(this.eventId).subscribe({
      next: (result: ResponseResult<PassCategoryRatesModel[]>) => {
        this.isBlocked = false;
        this.passCategoryRatesModel = result.data;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  openUpdatePopup(passCategoryId?: string, id?: string, isChargable?: boolean, passCategoryName?: string) {
    this.modalService.displayDialog(PassRatesUpdateComponent, {
      data: {
        id: id,
        eventId: this.eventId,
        passCategoryId: passCategoryId,
        passCategoryName: passCategoryName,
        isChargable: isChargable,
        isEdit: id == undefined ? false : true
      }
    })
    this.modalService.confirmed().subscribe(() => {
      this.getPassRates();
    });
  }

  chargableChanged(event: Event, passCategoryId: string, id: string, passCategoryName: string) {
    event.preventDefault();
    const isChecked = (event.target as HTMLInputElement).checked;

    if (isChecked) this.openUpdatePopup(passCategoryId, id, isChecked, passCategoryName);
    else this.updateStatus(isChecked, passCategoryId, id);

  }

  edit(isChargeable: boolean, passCategoryId: string, id: string, passCategoryName: string) {
    this.openUpdatePopup(passCategoryId, id, isChargeable, passCategoryName);
  }

  updateStatus(isChargeable: boolean, passCategoryId: string, id: string) {
    this.isBlocked = true;
    this.passCategorySettingsService.updateIsChargeableStatus(this.eventId, passCategoryId, id, isChargeable).subscribe({
      next: () => {
        this.isBlocked = false;
        this.getPassRates();
        this.toasterService.successfullyUpdated("Pass category rate status");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }
}
