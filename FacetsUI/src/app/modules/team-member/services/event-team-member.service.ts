import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/core/services/base.service';
import { AssignTeamMemberToEventModel } from '../models/assign-team-member-to-events.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { TeamMemberEventModel } from '../models/team-member-event.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { Observable } from 'rxjs';
import { TeamMemberFilterModel } from '../models/team-member-filter.model';

@Injectable({
  providedIn: 'root'
})
export class EventTeamMemberService extends BaseService {

  constructor() {
    super();
  }

  assignEventAndPassCategory(createTeamMemberModel: AssignTeamMemberToEventModel) {
    return this.post('event-team-members', createTeamMemberModel);
  }

  viewTeamMembersByEvent(searchModel: SearchRequestModel, filter: TeamMemberFilterModel): Observable<ResponseResult<TeamMemberEventModel[]>> {
    let url = `event-team-members?PageSize=${searchModel.pageSize}&PageNumber=${searchModel.pageNumber}&EventId=${filter.eventId}&SearchQuery=${searchModel.searchTerm}`

    if (filter.teamMemberStatus != undefined && filter.teamMemberStatus != null) {
      url = `${url}&teamMemberStatus=${filter.teamMemberStatus}`;
    }
    if (filter.passCategoryIds != undefined && filter.passCategoryIds != null) {
      filter.passCategoryIds!.forEach(p => {
        url = `${url}&passCategoryIds=${p}`
      });
    }
    return this.get<ResponseResult<TeamMemberEventModel[]>>(`${url}`);
  }

  cancelTeamMemberEvent(eventId: string, teamMemberId: string) {
    return this.delete(`event-team-members/${eventId}/team-members/${teamMemberId}`);
  }
}
