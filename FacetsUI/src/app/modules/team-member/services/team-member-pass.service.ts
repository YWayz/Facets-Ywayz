import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { BaseService } from 'src/app/core/services/base.service';
import { TeamMemberPassTemplateModel } from '../models/team-member-pass-template.model';
import { QRVerifiedTeamMemberModel } from '../models/qr-verified-team-member.model';
import { PassVerificationModel } from '../../visitor/models/pass-verification.model';
import { TeamMemberPassVerificationModel } from '../models/team-member-pass-verification.model';

@Injectable({
  providedIn: 'root'
})
export class TeamMemberPassService extends BaseService {

  constructor() { 
    super();
  }

  getTemplate(teamMemberId: string): Observable<ResponseResult<TeamMemberPassTemplateModel>> {
    return this.get<ResponseResult<TeamMemberPassTemplateModel>>(`team-member-passes/template/${teamMemberId}`);
  }

  update(teamMemberPassVerificationModel: TeamMemberPassVerificationModel): Observable<ResponseResult<QRVerifiedTeamMemberModel>> {
    return this.putWithReturn<ResponseResult<QRVerifiedTeamMemberModel>>(`team-member-passes/verify-qr`, teamMemberPassVerificationModel);
  }
}
