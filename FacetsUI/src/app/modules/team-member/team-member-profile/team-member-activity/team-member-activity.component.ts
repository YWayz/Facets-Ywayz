import { Component, OnInit, inject } from '@angular/core';
import { TeamMemberService } from '../../services/team-member.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { ActivatedRoute, Params } from '@angular/router';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { TeamMemberActivityModel } from '../../models/team-member-activity.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'facets-team-member-activity',
  templateUrl: './team-member-activity.component.html',
  styleUrls: ['./team-member-activity.component.scss']
})
export class TeamMemberActivityComponent implements OnInit{

  isBlocked = false;
  id = '';
  eventId: string | null;

  teamMemberActivitiesModel = new Array<TeamMemberActivityModel>();

  pageSizeOptions: number[] = [10, 25, 50, 100];
  searchModel = new SearchRequestModel(10, 1);

  authService = inject(AuthService);
  activatedRoute = inject(ActivatedRoute);
  teamMemberService = inject(TeamMemberService);
  toasterService = inject(ToasterService);

  ngOnInit(): void {

    this.eventId = localStorage.getItem(appConstant.selectedEventId);
    this.activatedRoute.params.subscribe((params: Params) => {
      this.id = params['id'];
      if (this.id != "" && this.id != undefined) {
        this.getTeamMemberActivitiesById(this.id);
      }
    });
  }

  getTeamMemberActivitiesById(teamMemberId: string) {
    this.isBlocked = true;
    this.teamMemberService.getTeamMemberActivities(this.searchModel, teamMemberId, this.eventId!)
      .subscribe({
        next: (res: ResponseResult<TeamMemberActivityModel[]>) => {
          Object.assign(this.teamMemberActivitiesModel, res.data);
          this.searchModel.totalRecords = res.totalRecordCount;
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    this.getTeamMemberActivitiesById(this.id);
  }

}
