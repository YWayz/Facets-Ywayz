import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, ViewChildren, inject } from '@angular/core';
import { VisitorRegistrationService } from '../services/visitor-registration.service';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { VisitorRegisterSummaryModel } from '../models/visitor-register-summary.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { isEmpty } from 'src/app/core/extensions/helpers';
import { KeyValue } from '@angular/common';
import { Observable, fromEvent, map, merge, startWith } from 'rxjs';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { FormBuilder, FormControlName, FormGroup } from '@angular/forms';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';
import { SuperAdminPermissions, VisitorRegistrationPermissions } from 'src/app/core/extensions/permission-constants';

@Component({
  selector: 'facets-visitor-manage',
  templateUrl: './visitor-manage.component.html',
  styleUrls: ['./visitor-manage.component.scss']
})
export class VisitorManageComponent implements OnInit, AfterViewInit {

  isBlocked = false;
  isFormSubmitted = false;
  isFilterShow = false;
  searchModel = new SearchRequestModel(10, 1);
  pageSizeOptions: number[] = [10, 25, 50, 100];
  filteredOptions: Observable<KeyValue<string, string>[]>;
  countries = new Array<KeyValue<string, string>>();
  superAdminPermissions = SuperAdminPermissions;
  visitorRegistrationPermissions = VisitorRegistrationPermissions;
  pageIndex = 0;

  visitorForm: FormGroup;
  registeredVisitors: VisitorRegisterSummaryModel[];
  validationModel: ValidationModel = new ValidationModel();

  visitorRegistrationService = inject(VisitorRegistrationService);
  toasterService = inject(ToasterService);
  lookupService = inject(LookupsService);
  formBuilder = inject(FormBuilder);
  router = inject(Router);
  authService = inject(AuthService)

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor(private activatedRoute: ActivatedRoute) {
    this.validationModel.validationMessages = {
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe({
      next: (params: Params) => {
        const pageNumber = params['pageNumber'];
        if (pageNumber != undefined || pageNumber != null) {
          this.searchModel.pageNumber = pageNumber;
          this.pageIndex = pageNumber - 1;
        }
      }
    });

    this.createVisitorForm();
    if (this.authService.hasPermissionAuthorization([this.superAdminPermissions.all, this.visitorRegistrationPermissions.view])) {
      this.getRegistrations();
    }
    this.getCountries();
  }

  ngAfterViewInit(): void {
    this.paginator.pageIndex = this.pageIndex;

    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.visitorForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.visitorForm, this.isFormSubmitted);
  }

  createVisitorForm() {
    this.visitorForm = this.formBuilder.group({
      countryId: [''],
      visitorStatus: ['']
    });

  }

  getRegistrations() {
    this.isFormSubmitted = true;
    this.validate();

    this.isBlocked = true;
    const filterValue = this.visitorForm.value;
    const countryId = this.countries.find(f => f.value == filterValue.countryId)?.key;
    this.visitorRegistrationService.getRegistrations(this.searchModel, '', countryId ?? '', filterValue.visitorStatus).subscribe({
      next: (result: ResponseResult<VisitorRegisterSummaryModel[]>) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.searchModel.totalRecords = result.totalRecordCount;
        this.registeredVisitors = result.data;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }

  getCountries() {
    this.lookupService.getCountries().subscribe({
      next: (result: ResponseResult<KeyValue<string, string>[]>) => {
        this.countries = result.data;
        this.filteredOptions = this.visitorForm.get('countryId')!.valueChanges.pipe(
          startWith(''),
          map(value => this._filter(value || '')),
        );
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    });
  }

  private _filter(value: string): KeyValue<string, string>[] {
    const filterValue = value.toLowerCase();
    return this.countries.filter(option => option.value.toLowerCase().includes(filterValue));
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;

    this.router.navigate([], { queryParams: { pageNumber: this.searchModel.pageNumber } });
    this.getRegistrations();
  }

  search() {
    this.searchModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    if (!isEmpty(this.searchModel.searchTerm) && this.searchModel.searchTerm.length > 0)
      this.getRegistrations();
    else if (isEmpty(this.searchModel.searchTerm)) {
      this.getRegistrations();
    }
  }

  showFilterSection() {
    this.visitorForm.reset();
    this.isFilterShow = !this.isFilterShow;
  }

  clearSearchTerm() {
    this.searchModel.searchTerm = '';
    this.search();
  }

  navigateToVisitorProfile(visitorId: string, registrationId: string) {
    this.router.navigate([`/admin/visitor/visitor-management/${visitorId}/registrations/${registrationId}`], { queryParams: { pageNumber: this.searchModel.pageNumber } })
  }

  reset() {
    this.visitorForm.reset();
    this.searchModel.searchTerm = '';
    this.search();
  }

  getVisitorStatusClass(status: string) {
    if (status == 'Active')
      return 's-active';
    if (status == 'BlackListed')
      return 's-blacklisted';
    if (status == 'Cancelled')
      return 's-cancel';
    return '';
  }
}
