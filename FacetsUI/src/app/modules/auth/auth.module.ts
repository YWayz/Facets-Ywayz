import { NgModule } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ResetPasswordLinkComponent } from './reset-password-link/reset-password-link.component';


@NgModule({
  declarations: [
    LoginComponent,
    ResetPasswordComponent,
    ResetPasswordLinkComponent
  ],
  imports: [
    SharedModule
  ]
})
export class AuthModule { }
