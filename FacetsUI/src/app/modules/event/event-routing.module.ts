import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EventViewComponent } from './event-view/event-view.component';
import { EventCreateComponent } from './event-create/event-create.component';
import { EventProfileComponent } from './event-profile/event-profile.component';
import { RegistrationSettingsComponent } from './event-configurations/event-registration-settings/registration-settings/registration-settings.component';
import { authGuard } from 'src/app/core/guards/auth.guard';
import { PassCategoryPermissions, PavilionPermissions, RegistrationCounterPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { PassTemplateComponent } from './event-configurations/event-pass/pass-template/pass-template.component';

const routes: Routes = [
  {
    path: '',
    component: EventViewComponent
  },
  {
    path: 'create',
    component: EventCreateComponent
  },
  {
    path: 'edit/:id',
    component: EventCreateComponent
  },
  {
    path: 'profile/:id',
    component: EventProfileComponent
  },
  {
    path: ':eventId/configuration/reg-settings',
    component: RegistrationSettingsComponent,
    canActivate: [authGuard],
    data: {
      roleClaimType: [
        SuperAdminPermissions.all, RegistrationCounterPermissions.view, PassCategoryPermissions.viewPassRate, PassCategoryPermissions.view, PavilionPermissions.view
      ]
    }
  },
  {
    path: ':eventId/configuration/event-pass',
    component: PassTemplateComponent,
    canActivate: [authGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EventRoutingModule { }
