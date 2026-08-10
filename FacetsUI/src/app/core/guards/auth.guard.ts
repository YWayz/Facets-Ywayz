import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService) as AuthService;
  const router = inject(Router) as Router;

  const permission = route.data ? route.data['roleClaimType'] : null;

  if (authService.token == null || authService.token == undefined || authService.token == '') {
    router.navigateByUrl('login');
    return false;
  }

  if (!permission) {
    return true;
  }

  if (authService.hasPermissionAuthorization(permission)) {
    return true;
  }

  router.navigate(['/forbidden']);
  return false;
};
