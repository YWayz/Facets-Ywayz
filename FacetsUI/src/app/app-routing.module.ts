import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './core/layout/layout/layout.component';
import { LoginComponent } from './modules/auth/login/login.component';
import { authGuard } from './core/guards/auth.guard';
import { AccessDeniedComponent } from './shared/components/access-denied/access-denied.component';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';
import { ResetPasswordLinkComponent } from './modules/auth/reset-password-link/reset-password-link.component';
import { ResetPasswordComponent } from './modules/auth/reset-password/reset-password.component';
import { LandingPageComponent } from './public/landing-page/landing-page.component';
import { VisitorRegistrationOverviewComponent } from './public/visitor-registration/visitor-registration-overview/visitor-registration-overview.component';
import { VisitorRegistrationSuccessComponent } from './public/visitor-registration/visitor-registration-success/visitor-registration-success.component';
import { VisitorRegistrationOnlinePaymentSuccessComponent } from './public/visitor-registration/visitor-registration-online-payment-success/visitor-registration-online-payment-success.component';

const routes: Routes = [
  {
    path: 'admin',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: "full",
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./modules/dashboard/dashboard.module').then(t => t.DashboardModule)
      },
      {
        path: 'event',
        loadChildren: () => import('./modules/event/event.module').then(t => t.EventModule)
      },
      {
        path: 'team-member',
        loadChildren: () => import('./modules/team-member/team-member.module').then(t => t.TeamMemberModule)
      },
      {
        path: 'report',
        loadChildren: () => import('./modules/report/report.module').then(t => t.ReportModule)
      },
      {
        path: 'visitor',
        loadChildren: () => import('./modules/visitor/visitor.module').then(t => t.VisitorModule)
      },
      {
        path: 'user-management',
        loadChildren: () => import('./modules/user/user.module').then(t => t.UserModule)
      },
    ]
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'resetpassword',
    component: ResetPasswordComponent,
  },
  {
    path: 'reset-password-link',
    component: ResetPasswordLinkComponent,
  },
  {
    path: '',
    component: LandingPageComponent
  },
  {
    path: 'visitor/register',
    component: VisitorRegistrationOverviewComponent
  },
  {
    path: 'visitor/payment/status',
    component: VisitorRegistrationOnlinePaymentSuccessComponent
  },
  {
    path: 'forbidden',
    component: AccessDeniedComponent,
  },
  {
    path: '**',
    component: PageNotFoundComponent,
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { bindToComponentInputs: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
