import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VisitorPassVerificationComponent } from './visitor-pass-verification/visitor-pass-verification.component';
import { VisitorManageComponent } from './visitor-manage/visitor-manage.component';
import { VisitorPassComponent } from './visitor-pass/visitor-pass.component';
import { VisitorRegistrationOverviewComponent } from './visitor-registration/visitor-registration-overview/visitor-registration-overview.component';
import { authGuard } from 'src/app/core/guards/auth.guard';
import { PassGenerationPermissions, PavilionVisitorPermissions, SuperAdminPermissions, VisitorRegistrationPermissions } from 'src/app/core/extensions/permission-constants';
import { VisitorPassProfileComponent } from './visitor-pass/visitor-pass-profile/visitor-pass-profile.component';
import { VisitorViewComponent } from './visitor-manage/visitor-view/visitor-view.component';
import { VisitorPassGenerationComponent } from './visitor-pass/visitor-pass-generation/visitor-pass-generation.component';
import { VisitorPassVerificationPavilionComponent } from './visitor-pass-verification-pavilion/visitor-pass-verification-pavilion.component';
import { PavilionVisitorComponent } from './pavilion-visitor/pavilion-visitor.component';
import { VisitorPayLaterComponent } from './visitor-pass/visitor-pay-later/visitor-pay-later.component';

const routes: Routes = [
  {
    path: 'register',
    component: VisitorRegistrationOverviewComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, VisitorRegistrationPermissions.onSiteRegister, VisitorRegistrationPermissions.onSiteUpdate
      ]
    }
  },
  {
    path: 'pass-verification/event',
    component: VisitorPassVerificationComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, PassGenerationPermissions.verification
      ]
    }
  },
  {
    path: 'pass-verification/pavilion',
    component: VisitorPassVerificationPavilionComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, PassGenerationPermissions.verification
      ]
    }
  },
  {
    path: 'pass-generation',
    component: VisitorPassComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, PassGenerationPermissions.view
      ]
    }
  },
  {
    path: 'pass-generation/:visitorId/profile/:attendanceScheduleId/registrations/:registrationId',
    component: VisitorPassProfileComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, PassGenerationPermissions.view
      ]
    }
  },
  {
    path: 'visitor-management',
    component: VisitorManageComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, VisitorRegistrationPermissions.view
      ]
    }
  },
  {
    path: 'pavilion-visitor',
    component: PavilionVisitorComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, PavilionVisitorPermissions.view
      ]
    }
  },
  {
    path: 'visitor-management/:visitorId/registrations/:registrationId',
    component: VisitorViewComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, VisitorRegistrationPermissions.view
      ]
    }
  },
  {
    path: 'pass-generation/print/:visitorId/:eventDateId/:attendanceScheduleId',
    component: VisitorPassGenerationComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, PassGenerationPermissions.view
      ]
    }
  },
  {
    path: 'pay-later/:visitorRegistrationId',
    component: VisitorPayLaterComponent,
    canActivate: [authGuard],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VisitorRoutingModule { }
