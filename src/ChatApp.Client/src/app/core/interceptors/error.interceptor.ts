import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastr = inject(ToastrService);
  
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
            break;

          case 404:
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
