import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TeamMemberViewComponent } from './team-member-view/team-member-view.component';
import { TeamMemberCreateComponent } from './team-member-create/team-member-create.component';
import { SuperAdminPermissions, TeamMemberRegistrationPermissions } from 'src/app/core/extensions/permission-constants';
import { authGuard } from 'src/app/core/guards/auth.guard';
import { TeamMemberProfileComponent } from './team-member-profile/team-member-profile.component';
import { TeamMemberPassGenerationComponent } from './team-member-profile/team-member-pass-generation/team-member-pass-generation.component';

const routes: Routes = [
  {
    path: '',
    component: TeamMemberViewComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, TeamMemberRegistrationPermissions.cancelTeamMemberRegistration, TeamMemberRegistrationPermissions.generateTeamMemberPass, TeamMemberRegistrationPermissions.register, TeamMemberRegistrationPermissions.updateTeamMember, TeamMemberRegistrationPermissions.uploadAttachments, TeamMemberRegistrationPermissions.viewAttachments
      ]
    }
  },
  {
    path: 'create',
    component: TeamMemberCreateComponent,
  },
  {
    path: ':id',
    component: TeamMemberProfileComponent,
  },
  {
    path: 'pass-generation/print/:teamMemberId',
    component: TeamMemberPassGenerationComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeamMemberRoutingModule { }
