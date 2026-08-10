import { AfterViewInit, Component, OnInit, ViewChild, inject } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { SuperAdminPermissions, TeamMemberRegistrationPermissions } from 'src/app/core/extensions/permission-constants';
import { EventTeamMemberService } from '../services/event-team-member.service';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { TeamMemberEventModel } from '../models/team-member-event.model';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { KeyValue } from 'src/app/core/models/key-value.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { TeamMemberFilterModel } from '../models/team-member-filter.model';
import { SharedService } from 'src/app/core/services/shared.service';
import { TeamMemberService } from '../services/team-member.service';
import { TeamMemberModel } from '../models/team-member.model';

@Component({
  selector: 'facets-team-member-view',
  templateUrl: './team-member-view.component.html',
  styleUrls: ['./team-member-view.component.scss']
})
export class TeamMemberViewComponent implements OnInit, AfterViewInit {

  isBlocked = false;
  isAllSelected = false;
  showFilter = false;

  pageSizeOptions: number[] = [10, 25, 50, 100];
  searchModel = new SearchRequestModel(10, 1);
  pageIndex = 0;

  selectedStatus: string | undefined;

  superAdminPermissions = SuperAdminPermissions;
  teamMemberRegistrationPermissions = TeamMemberRegistrationPermissions;

  teamMemberFilterModel = new TeamMemberFilterModel();
  teamMemberStatuses: string[] = [
    "Cancelled", "Active"
  ]
  teamMemberEventsModel = new Array<TeamMemberEventModel>();
  selectedPassCategories = new Array<string>();
  passCategories: Array<KeyValue<string, string>>;

  teamMember = new TeamMemberModel();

  lookupsService = inject(LookupsService);
  sharedService = inject(SharedService);
  toasterService = inject(ToasterService);
  eventTeamMemberService = inject(EventTeamMemberService);
  teamMemberService = inject(TeamMemberService);


  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
  }

  public get eventId() {
    return this.sharedService.getEventId();;
  }

  ngAfterViewInit(): void {
    this.paginator.pageIndex = this.pageIndex;
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

    this.teamMemberFilterModel.eventId = this.eventId;
    if (this.eventId == null) {
      this.toasterService.warning('You have not selected an event', 'No event selected');
      return;
    }
    this.getPassCategories();
    this.getTeamMembersByEvent();
    
   


    
  }

  getTeamMembersByEvent() {
    this.isBlocked = true;
    this.eventTeamMemberService.viewTeamMembersByEvent(this.searchModel, this.teamMemberFilterModel).subscribe({
      next: (res: ResponseResult<TeamMemberEventModel[]>) => {
        this.teamMemberEventsModel = res.data;
        this.teamMemberFilterModel.eventId = this.eventId;
        this.searchModel.totalRecords = res.totalRecordCount;
        this.isBlocked = false;
      },
    })
  }

  getPassCategories() {
    this.isBlocked = true;
    this.lookupsService.getPassCategories(this.searchModel, this.eventId!, '&passCategoryType=TeamMember').subscribe({
      next: (res: ResponseResult<KeyValue<string, string>[]>) => {
        this.passCategories = res.data;
        this.isBlocked = false;
      },
    })
  }

  search() {
    this.searchModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getTeamMembersByEvent();
  }

  clearSearchTerm() {
    this.searchModel.searchTerm = '';
    this.search();
  }

  navigateToCreate() {
    this.router.navigate(['/admin/team-member/create'])
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;

    this.router.navigate([], { queryParams: { pageNumber: this.searchModel.pageNumber } });

    this.getTeamMembersByEvent();
  }

  toggleAllSelection(event: Event) {
    const isAllChecked = (event.target as HTMLInputElement).checked;
    if (isAllChecked) {
      this.selectedPassCategories = [];
      this.selectedPassCategories.push(...this.passCategories.map(item => item.key));
      this.isAllSelected = true;
    } else {
      this.selectedPassCategories = [];
      this.isAllSelected = false;
    }
    this.getTeamMembersByEvent();
  }

  navigateToProfile(teamMemberId: string) {
    this.router.navigate([`admin/team-member/${teamMemberId}`], { queryParams: { pageNumber: this.searchModel.pageNumber } });
  }

  cancelFilter() {
    this.showFilter = !this.showFilter;
    this.teamMemberFilterModel = new TeamMemberFilterModel();
    this.teamMemberFilterModel.eventId = this.eventId;
    this.getTeamMembersByEvent();
  }


  getTeamMember(teamMemberId: string): void {
    this.teamMemberService.getTeamMemberById(teamMemberId).subscribe({
      next: (response: ResponseResult<TeamMemberModel>) => {
        if (response.success) {
          this.teamMember = response.data; 
        } else {
          // Handle the error response if necessary
          // console.error(response.message);
        }
      },
      error: (error) => {
        // Handle any errors that might occur during the API call
        console.error('Error fetching team member:', error);
      }
    });
  }
}
