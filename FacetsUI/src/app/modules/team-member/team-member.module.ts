import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { TeamMemberCreateComponent } from './team-member-create/team-member-create.component';
import { TeamMemberViewComponent } from './team-member-view/team-member-view.component';
import { TeamMemberSuccessfulRegistrationComponent } from './team-member-successful-registration/team-member-successful-registration.component';
import { TeamMemberActivityComponent } from './team-member-profile/team-member-activity/team-member-activity.component';
import { TeamMemberAttachmentComponent } from './team-member-profile/team-member-attachment/team-member-attachment.component';
import { TeamMemberProfileComponent } from './team-member-profile/team-member-profile.component';
import { TeamMemberRoutingModule } from './team-member-routing.module';
import { VerifyNicPassportComponent } from 'src/app/shared/components/registration/verify-nic-passport/verify-nic-passport.component';
import { AttachmentNicPassportComponent } from 'src/app/shared/components/attachment/attachment-nic-passport/attachment-nic-passport.component';
import { AttachmentOtherComponent } from 'src/app/shared/components/attachment/attachment-other/attachment-other.component';
import { TeamMemberPassGenerationComponent } from './team-member-profile/team-member-pass-generation/team-member-pass-generation.component';
import { QRCodeModule } from 'angularx-qrcode';

@NgModule({
  declarations: [TeamMemberViewComponent, TeamMemberCreateComponent, TeamMemberSuccessfulRegistrationComponent,
    TeamMemberProfileComponent, TeamMemberActivityComponent, TeamMemberAttachmentComponent],
  imports: [
    CommonModule,
    TeamMemberRoutingModule,
    VerifyNicPassportComponent,
    AttachmentNicPassportComponent,
    AttachmentOtherComponent,
    SharedModule
  ]
})
export class TeamMemberModule { }
