import { Component, ElementRef, OnInit, ViewChild, ViewChildren, inject } from '@angular/core';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { RegistrationCounterModel } from '../../../models/registration-counter.model';
import { MatDialog } from '@angular/material/dialog';
import { RegistrationCounterService } from '../../../services/registration-counter.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ModalService } from 'src/app/core/services/modal.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { RegistrationCountersCreateComponent } from './registration-counters-create/registration-counters-create.component';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { RegistrationCounterPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { FormBuilder, FormControlName, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'facets-registration-counters',
  templateUrl: './registration-counters.component.html',
  styleUrls: ['./registration-counters.component.scss']
})
export class RegistrationCountersComponent implements OnInit {

  isBlocked = false;
  isFilterShow = false;
  eventId = '';
  status: boolean | undefined;
  registrationCounterPermissions = RegistrationCounterPermissions;
  superAdminPermissions = SuperAdminPermissions;

  searchModel = new SearchRequestModel(10, 1);
  pageSizeOptions: number[] = [10, 25, 50, 100];
  registrationCounterModels: RegistrationCounterModel[];
  counterForm: FormGroup;

  dialog = inject(MatDialog);
  registrationCounterService = inject(RegistrationCounterService);
  toasterService = inject(ToasterService);
  authService = inject(AuthService);
  router = inject(Router);
  modalService = inject(ModalService);
  activatedRoute = inject(ActivatedRoute);
  formBuilder = inject(FormBuilder);

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((param: Params) => {
      this.eventId = param['eventId'];
      if (this.eventId != null || this.eventId != undefined || this.eventId != '') {
        this.createCounterForm();
        this.getRegistrationCounter();
      }
    });
  }

  getRegistrationCounter() {
    this.isBlocked = true;
    this.registrationCounterService.getAll(this.eventId, this.searchModel, this.status!).subscribe({
      next: (result: ResponseResult<RegistrationCounterModel[]>) => {
        this.isBlocked = false;
        this.registrationCounterModels = result.data;
        this.searchModel.totalRecords = result.totalRecordCount;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  createCounterForm() {
    this.counterForm = this.formBuilder.group({
      counterStatus: ['']
    });

  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    this.getRegistrationCounter();
  }

  openCreatePopup(id?: string) {
    this.modalService.displayDialog(RegistrationCountersCreateComponent, {
      data: {
        id: id,
        eventId: this.eventId,
        isEdit: id == undefined ? false : true
      }
    });
    this.modalService.confirmed().subscribe(() => this.getRegistrationCounter());
  }

  deleteRegistrationCounter(id: string) {
    let options = {
      title: 'Confirmation Message',
      message: 'Are you sure you want to delete this registration counter?',
    };
    this.modalService.displayDialog(ConfirmationModalComponent, options);
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.registrationCounterService.deleteRegistrationCounter(this.eventId, id).subscribe({
          next: () => {
            this.isBlocked = false;
            this.toasterService.successfullyDeleted("Registration counter");
            this.getRegistrationCounter();
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.toasterService.error(err);
          }
        });
      }
    });
  }

  counterStatusUpdate(event: any, userName: string | null, counterId: string) {
    let status = event.target?.checked;
    event.preventDefault();

    if (status == false) {
      let options = {
        title: 'Unlock Counter',
        message: `The counter is in use by '${userName}'. Are you sure, do you want to unlock this registration counter?`,
      };
      this.modalService.displayDialog(ConfirmationModalComponent, options);
      this.modalService.confirmed().subscribe((confirmed: boolean) => {
        if (confirmed) {
          this.isBlocked = true;
          this.registrationCounterService.unLockCounter(this.eventId, counterId)
            .subscribe({
              next: (res: any) => {
                this.isBlocked = false;
                this.toasterService.success("You have successfully un-locked the counter.");
                this.getRegistrationCounter();
              },
              error: (err: ErrorResponse) => {
                this.isBlocked = false;
                this.toasterService.error(err);
              }
            })
        }
      });
    }

  }

  showFilterSection() {
    this.counterForm.reset();
    this.status = undefined;
    this.isFilterShow = !this.isFilterShow;
    this.getRegistrationCounter();
  }

  search() {
    this.searchModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    const filterValue = this.counterForm.value;
    if (filterValue.counterStatus == 'locked')
      this.status = true;
    else if (filterValue.counterStatus == 'unLocked')
      this.status = false;

    this.getRegistrationCounter();
  }
}
