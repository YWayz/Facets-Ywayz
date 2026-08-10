import { Component, OnInit, inject } from '@angular/core';
import { PassCategoryPermissions, PavilionPermissions, PaymentSettingsPermissions, RegistrationCounterPermissions, SuperAdminPermissions } from 'src/app/core/extensions/permission-constants';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Component({
  selector: 'facets-registration-settings',
  templateUrl: './registration-settings.component.html',
  styleUrls: ['./registration-settings.component.scss']
})
export class RegistrationSettingsComponent implements OnInit {

  isPassCategoryHidePermission = false;
  isRegistrationCounterHidePermission = false;
  isPassRateHidePermission = false;
  isPavilionHidePermission = false;

  registrationCounterPermissions = RegistrationCounterPermissions;
  superAdminPermissions = SuperAdminPermissions;
  passCategoryPermissions = PassCategoryPermissions;
  pavilionPermissions = PavilionPermissions;
  paymentSettingsPermissions = PaymentSettingsPermissions;

  authService = inject(AuthService);

  ngOnInit(): void {
    if(!this.authService.hasPermissionAuthorization([SuperAdminPermissions.all, PassCategoryPermissions.view])) {
      this.isPassCategoryHidePermission = true;
    }
    
    if(!this.authService.hasPermissionAuthorization([SuperAdminPermissions.all, RegistrationCounterPermissions.view])) {
      this.isRegistrationCounterHidePermission = true;
    }
    
    if(!this.authService.hasPermissionAuthorization([SuperAdminPermissions.all, PassCategoryPermissions.viewPassRate])) {
      this.isPassRateHidePermission = true;
    }
    
    if(!this.authService.hasPermissionAuthorization([SuperAdminPermissions.all, PavilionPermissions.view])) {
      this.isPavilionHidePermission = true;
    }
  }
  
}
