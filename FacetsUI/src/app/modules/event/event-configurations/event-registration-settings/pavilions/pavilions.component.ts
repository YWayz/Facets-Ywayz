import { Component, OnInit, inject } from '@angular/core';
import { PavilionPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { PavilionModel } from '../../../models/pavilion.model';
import { ModalService } from 'src/app/core/services/modal.service';
import { PavilionCreateComponent } from './pavilion-create/pavilion-create.component';
import { ActivatedRoute, Params } from '@angular/router';
import { PavilionService } from '../../../services/pavilion.service';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { PageEvent } from '@angular/material/paginator';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { PavilionRatesComponent } from './pavilion-rates/pavilion-rates.component';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Component({
  selector: 'facets-pavilions',
  templateUrl: './pavilions.component.html',
  styleUrls: ['./pavilions.component.scss']
})
export class PavilionsComponent implements OnInit {
  isBlocked = false;
  eventId: '';

  searchModel = new SearchRequestModel(10, 1);
  pageSizeOptions: number[] = [10, 25, 50, 100];

  pavilionModels = new Array<PavilionModel>();

  superAdminPermissions = SuperAdminPermissions;
  pavilionPermissions = PavilionPermissions;

  modalService = inject(ModalService);
  toasterService = inject(ToasterService);
  pavilionService = inject(PavilionService);
  activatedRoute = inject(ActivatedRoute);
  authService = inject(AuthService);

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((param: Params) => {
      this.eventId = param['eventId'];
    })
    this.getPavilions();
  }

  getPavilions() {
    this.isBlocked = true;
    this.pavilionService.getAllPavilions(this.eventId, this.searchModel).subscribe({
      next: (res: ResponseResult<PavilionModel[]>) => {
        this.pavilionModels = res.data;
        this.searchModel.totalRecords = res.totalRecordCount;
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      },
    })
  }

  openCreatePopup(id?: string) {
    this.modalService.displayDialog(PavilionCreateComponent, {
      data: {
        id: id,
        eventId: this.eventId,
        isEdit: id == undefined ? false : true
      }
    });

    this.modalService.confirmed().subscribe(() => this.getPavilions());
  }


  openPavilionRatePopup(pavilionId?: string) {
    this.modalService.displayDialog(PavilionRatesComponent, {
      data: {
        id: pavilionId,
        eventId: this.eventId,
        isEdit: pavilionId == undefined ? false : true
      }
    });

    this.modalService.confirmed().subscribe(() => this.getPavilions());
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    this.getPavilions();
  }

  updatePavilion(event: any, model: PavilionModel) {
    let status = event.target?.checked;
    event.preventDefault();

    let options = {
      title: 'Update Pavilion Status',
      message: 'Are you sure you want to update the status of the pavilion?'
    }

    this.modalService.displayDialog(ConfirmationModalComponent, options);
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        let body = { pavilionStatus: status ? 'Active' : 'Inactive' }
        this.isBlocked = true;
        this.pavilionService.updatePavilionStatus(this.eventId, model.id, body).subscribe({
          next: () => {
            model.status = status ? 'Active' : 'Inactive';
            this.isBlocked = false;
            this.toasterService.successfullyUpdated('Pavilion Status');
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.toasterService.error(err);
          },
        })
      }
    })
  }

  deletePavilion(pavilionId: string) {
    let options = {
      title: 'Delete Pavilion',
      message: 'Are you sure you want to delete the pavilion?'
    }

    this.modalService.displayDialog(ConfirmationModalComponent, options);
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.pavilionService.deletePavilion(this.eventId, pavilionId).subscribe({
          next: () => {
            this.isBlocked = false;
            this.getPavilions();
            this.toasterService.successfullyDeleted('Pavilion');
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.toasterService.error(err);
          },
        })
      }
    })
  }
}
