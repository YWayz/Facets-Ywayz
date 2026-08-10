import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, ViewChildren, inject } from '@angular/core';
import { FormBuilder, FormControlName, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Observable, fromEvent, map, merge } from 'rxjs';
import { convertOnlyDate, isEmpty } from 'src/app/core/extensions/helpers';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { AuthService } from '../../auth/services/auth.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { PavilionVisitorPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { VisitorRegistrationService } from '../services/visitor-registration.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { PavilionSessionVisitorModel } from '../models/pavilion-sessionvisitor.model';

@Component({
  selector: 'facets-pavilion-visitor',
  templateUrl: './pavilion-visitor.component.html',
  styleUrls: ['./pavilion-visitor.component.scss']
})
export class PavilionVisitorComponent implements OnInit, AfterViewInit {

  isBlocked = false;
  panelOpenState = false;
  isFormSubmitted = false;
  isFilterShow = false;
  searchModel = new SearchRequestModel(10, 1);
  pageSizeOptions: number[] = [10, 25, 50, 100];

  superAdminPermissions = SuperAdminPermissions;
  pavilionVisitorPermissions = PavilionVisitorPermissions;

  visitorPavilionForm: FormGroup;
  validationModel: ValidationModel = new ValidationModel();

  pavilionSessionVisitorsModel = new Array<PavilionSessionVisitorModel>();
  filteredPavilionSessionVisitorsModel = new Array<PavilionSessionVisitorModel>();

  formBuilder = inject(FormBuilder);
  toasterService = inject(ToasterService);
  visitorRegistrationService = inject(VisitorRegistrationService);
  authService = inject(AuthService)

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor() {
    this.validationModel.validationMessages = { };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createVisitorPavilionForm();
    if (this.authService.hasPermissionAuthorization([this.superAdminPermissions.all, this.pavilionVisitorPermissions.view])) {
      this.getPavilionVisitors();
    }
  }

  createVisitorPavilionForm() {
    this.visitorPavilionForm = this.formBuilder.group({
      eventDate: [''],
      pavilionStatus: []
    });
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.visitorPavilionForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.visitorPavilionForm, this.isFormSubmitted);
  }

  getPavilionVisitors() {
    this.isFormSubmitted = true;
    this.validate();

    this.isBlocked = true;
    const filterValue = this.visitorPavilionForm.value;
    this.visitorRegistrationService.getPavilionVisitors(this.searchModel, convertOnlyDate(filterValue.eventDate), '' ?? '', filterValue.pavilionStatus).subscribe({
      next: (result: ResponseResult<PavilionSessionVisitorModel[]>) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.searchModel.totalRecords = result.totalRecordCount;
        this.pavilionSessionVisitorsModel = result.data;
        this.pavilionSessionVisitorsModel.map(m => m.isExpanded = false);
        this.filteredPavilionSessionVisitorsModel = this.pavilionSessionVisitorsModel;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }

  expandTableData(visitorFullName: string) {
    let pavilionSessionVisitorModel = this.pavilionSessionVisitorsModel.find(f => f.visitorFullName == visitorFullName)!;
    pavilionSessionVisitorModel.isExpanded = !pavilionSessionVisitorModel.isExpanded;
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    this.getPavilionVisitors();
  }

  search() {
    this.searchModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    if (!isEmpty(this.searchModel.searchTerm) && this.searchModel.searchTerm.length > 0)
      this.getPavilionVisitors();
    else if (isEmpty(this.searchModel.searchTerm)) {
      this.getPavilionVisitors();
    }
  }

  showFilterSection() {
    this.visitorPavilionForm.reset();
    this.isFilterShow = !this.isFilterShow;
  }

  clearSearchTerm() {
    this.searchModel.searchTerm = '';
    this.search();
  }

  reset() {
    this.visitorPavilionForm.reset();
    this.searchModel.searchTerm = '';
    this.search();
  }

  getPavilionVisitorStatusClass(status: boolean) {
    if (status == false)
      return 's-active';
    if (status == true)
      return 's-cancel';
    return '';
  }
}
