import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '@core/services';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  if (auth.user) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${auth.user.token}`
      }
    });
  }

  return next(req);
};
