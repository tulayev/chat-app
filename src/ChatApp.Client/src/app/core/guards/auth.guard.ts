import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@core/services';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const user = authService.user;

  if (user) {
    if (!user.emailConfirmed) {
      router.navigate(['/verify-email']);
      return false;
    }

    return true;
  }

  router.navigate(['/login']);
  return false;
};
