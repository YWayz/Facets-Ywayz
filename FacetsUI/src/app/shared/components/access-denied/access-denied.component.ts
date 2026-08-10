import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { LogoutModel } from 'src/app/modules/auth/models/logout.model';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Component({
  selector: 'facets-access-denied',
  templateUrl: './access-denied.component.html',
  styleUrls: ['./access-denied.component.scss']
})
export class AccessDeniedComponent {

  router = inject(Router);
  authService = inject(AuthService);
  toasterService = inject(ToasterService);

  navigateToLogin() {
    this.authService.logout(new LogoutModel(this.authService.userId!)).subscribe({
      next: () => {
        localStorage.clear();
        this.router.navigate(['login']);
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    })
  }
}
