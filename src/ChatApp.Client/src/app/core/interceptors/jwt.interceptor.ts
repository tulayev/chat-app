import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '@core/services';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  if (authService.user) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${authService.user.token}`
      }
    });
  }

  return next(req);
};
