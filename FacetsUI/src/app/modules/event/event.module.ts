import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventViewComponent } from './event-view/event-view.component';
import { EventCreateComponent } from './event-create/event-create.component';
import { EventProfileComponent } from './event-profile/event-profile.component';
import { RegistrationSettingsComponent } from './event-configurations/event-registration-settings/registration-settings/registration-settings.component';
import { PassCategoryComponent } from './event-configurations/event-registration-settings/pass-category/pass-category.component';
import { PassRatesComponent } from './event-configurations/event-registration-settings/pass-rates/pass-rates.component';
import { RegistrationCountersComponent } from './event-configurations/event-registration-settings/registration-counters/registration-counters.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { EventRoutingModule } from './event-routing.module';
import { PassCategoryCreateComponent } from './event-configurations/event-registration-settings/pass-category/pass-category-create/pass-category-create.component';
import { PassRatesUpdateComponent } from './event-configurations/event-registration-settings/pass-rates/pass-rates-update/pass-rates-update.component';
import { RegistrationCountersCreateComponent } from './event-configurations/event-registration-settings/registration-counters/registration-counters-create/registration-counters-create.component';
import { PassTemplateComponent } from './event-configurations/event-pass/pass-template/pass-template.component';
import { MemberPassTemplateComponent } from './event-configurations/event-pass/member-pass-template/member-pass-template.component';
import { VisitorPassTemplateComponent } from './event-configurations/event-pass/visitor-pass-template/visitor-pass-template.component';
import { PassTemplateSizeComponent } from './event-configurations/event-pass/pass-template-size/pass-template-size.component';
import { PassTemplateValueComponent } from './event-configurations/event-pass/pass-template-value/pass-template-value.component';
import { PavilionsComponent } from './event-configurations/event-registration-settings/pavilions/pavilions.component';
import { PavilionCreateComponent } from './event-configurations/event-registration-settings/pavilions/pavilion-create/pavilion-create.component';
import { PavilionRatesComponent } from './event-configurations/event-registration-settings/pavilions/pavilion-rates/pavilion-rates.component';
import { PaymentSettingsComponent } from './event-configurations/event-registration-settings/payment-settings/payment-settings.component';


@NgModule({
  declarations: [
    EventViewComponent,
    EventCreateComponent,
    EventProfileComponent,
    RegistrationSettingsComponent,
    PassCategoryComponent,
    PassRatesComponent,
    RegistrationCountersComponent,
    PassCategoryCreateComponent,
    PassRatesUpdateComponent,
    RegistrationCountersCreateComponent,
    PassTemplateComponent,
    MemberPassTemplateComponent,
    VisitorPassTemplateComponent,
    PassTemplateSizeComponent,
    PassTemplateValueComponent,
    PavilionsComponent,
    PavilionCreateComponent,
    PavilionRatesComponent,
    PaymentSettingsComponent
  ],
  imports: [
    CommonModule,
    EventRoutingModule,
    SharedModule
  ]
})
export class EventModule { }
