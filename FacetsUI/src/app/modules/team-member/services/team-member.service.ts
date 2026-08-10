import { Injectable } from '@angular/core';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { CreateTeamMemberModel } from '../models/create-team-member.model';
import { TeamMemberModel } from '../models/team-member.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpHeaders } from '@angular/common/http';
import { TeamMemberSearchModel } from '../models/team-member-search.model';
import { TeamMemberActivityModel } from '../models/team-member-activity.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { UpdateTeamMemberModel } from '../models/update-team-member.model';
import { FileModel } from 'src/app/shared/models/file.model';

@Injectable({
  providedIn: 'root'
})
export class TeamMemberService extends BaseService {

  constructor() {
    super();
  }

  register(createTeamMemberModel: CreateTeamMemberModel): Observable<ResponseResult<TeamMemberModel>> {
    return this.post<ResponseResult<TeamMemberModel>>('team-members', createTeamMemberModel);
  }
  
  uploadImage(teamMemberId: string, files: File[]) {
    return this.filePost(`team-members/${teamMemberId}/profile-image`, files)
  }
  
  getTeamMemberDocuments(teamMemberId: string): Observable<ResponseResult<FileModel[]>> {
    return this.get<ResponseResult<FileModel[]>>(`team-members/${teamMemberId}/documents`);
  }
 
  deleteTeamMemberDocument(teamMemberId: string, documentId: string) {
    return this.delete(`team-members/${teamMemberId}/documents/${documentId}`);
  }
  
  getTeamMemberById(teamMemberId: string): Observable<ResponseResult<TeamMemberModel>> {
    return this.get<ResponseResult<TeamMemberModel>>(`team-members/${teamMemberId}`);
  }

  uploadDocument(teamMemberId: string, formData: FormData) {
    return this.http.post(`${environment.baseEndPoint}/team-members/${teamMemberId}/documents`, formData, {
      headers: new HttpHeaders({
        'enctype': 'multipart/form-data'
      })
    });
  }
  searchTeamMember(searchValue: string): Observable<ResponseResult<TeamMemberSearchModel>> {
    return this.get<ResponseResult<TeamMemberSearchModel>>(`team-members/search?searchValue=${searchValue}`);
  }
  
  getTeamMemberActivities(searchModel: SearchRequestModel, teamMemberId: string, eventId:string): Observable<ResponseResult<TeamMemberActivityModel[]>> {
    return this.get<ResponseResult<TeamMemberActivityModel[]>>(`team-members/${teamMemberId}/event/${eventId}/activities?PageSize=${searchModel.pageSize}&PageNumber=${searchModel.pageNumber}`);
  }

  updateTeamMemberWithEvent(teamMemberId: string, updateTeamMemberModel: UpdateTeamMemberModel) {
    return this.put(`team-members/${teamMemberId}/event`, updateTeamMemberModel);
  }
  
  updateTeamMember(teamMemberId: string, updateTeamMemberModel: UpdateTeamMemberModel) {
    return this.put(`team-members/${teamMemberId}`, updateTeamMemberModel);
  }

  blaklistTeamMember(teamMemberId: string){
    return this.put(`team-members/${teamMemberId}/mark-as-blacklisted`);
  }

  RemoveblaklistTeamMember(teamMemberId: string){
    return this.put(`team-members/${teamMemberId}/remove-blacklisted`);
  }
}
