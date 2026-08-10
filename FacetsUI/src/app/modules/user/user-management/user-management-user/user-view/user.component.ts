import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { UserModel } from '../../../models/user.model';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { UserService } from '../../../services/user.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { isEmpty } from 'src/app/core/extensions/helpers';
import { ConfirmationModalComponent } from 'src/app/shared/modals/confirmation-modal/confirmation-modal.component';
import { ModalService } from 'src/app/core/services/modal.service';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { KeyValue } from 'src/app/core/models/key-value.model';

@Component({
  selector: 'facets-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {

  isDisplay = false;
  isBlocked = false;
  isEdit = false;

  userId: string = '';

  events: KeyValue<string, string>[];

  pageSizeOptions: number[] = [10, 25, 50, 100];

  userModels: UserModel[];
  searchModel = new SearchRequestModel(10, 1);

  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(private userService: UserService, private lookUpService: LookupsService, private toasterService: ToasterService, private modalService: ModalService) { }

  ngOnInit(): void {
    this.getEvents();
    this.getUsers();
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    this.getUsers();
  }

  viewCreateUser() {
    if (this.events.length == 0) {
      this.toasterService.warning('There are no events created', 'No events created');
      return;
    }
    this.isDisplay = true;
  }

  viewEditUser(userId: string) {
    this.isEdit = true;
    this.userId = userId;
  }

  setDisplay() {
    this.isDisplay = false;
    this.userId = "";
    this.isBlocked = false;
  }

  clearSearchTerm() {
    this.searchModel.searchTerm = '';
    this.search();
  }

  getEvents() {
    this.lookUpService.getAllEvents(this.searchModel).subscribe({
      next: (res: ResponseResult<KeyValue<string, string>[]>) => {
        this.events = res.data;
      },
    })
  }

  getUsers() {
    this.isBlocked = true;
    this.userService.getAll(this.searchModel).subscribe({
      next: (users: ResponseResult<UserModel[]>) => {
        this.userModels = users.data;
        this.searchModel.totalRecords = users.totalRecordCount;
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  search() {
    this.searchModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    if (!isEmpty(this.searchModel.searchTerm) && this.searchModel.searchTerm.length > 2)
      this.getUsers();
    else if (isEmpty(this.searchModel.searchTerm)) {
      this.getUsers();
    }
  }

  deleteUser(userId: string) {
    let options = {
      title: 'Delete User',
      message: 'Are you sure you want to delete the user ?',
    };
    this.modalService.displayDialog(ConfirmationModalComponent, options)
    this.modalService.confirmed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isBlocked = true;
        this.userService.deleteUser(userId)
          .subscribe({
            next: (res: any) => {
              this.isBlocked = false;
              this.toasterService.success("User has been successfully deleted");
              this.getUsers();
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
