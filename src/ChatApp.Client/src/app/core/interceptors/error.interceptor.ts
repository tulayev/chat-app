import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '@core/services';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastr = inject(ToastrService);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error) {
        switch (error.status) {
          case 400:
            if (error.error?.errorMessage) {
              toastr.error(error.error.errorMessage, error.status.toString());
            }
            break;

          case 401:
            toastr.error('Unauthorized', error.status.toString());
            authService.logout();
            router.navigateByUrl('/login');
            break;

          case 404:
            toastr.error('Not found', error.status.toString());
            router.navigateByUrl('/not-found');
            break;

          case 500:
            toastr.error('Internal Server Error!');
            break;

          default:
            toastr.error('Something went wrong');
            break;
        }
      }
      return throwError(() => error);
    })
  );
};
