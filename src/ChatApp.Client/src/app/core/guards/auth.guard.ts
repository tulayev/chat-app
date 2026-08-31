import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { map, take } from 'rxjs';
import { selectUser } from '@store/auth';

export const authGuard: CanActivateFn = () => {
  const store = inject(Store);
  const router = inject(Router);

  return store.select(selectUser).pipe(
    take(1),
    map((user) => {
      if (user) {
        if (!user.emailConfirmed) {
          router.navigate(['/verify-email']);
          return false;
        }

        return true;
      }

      router.navigate(['/login']);
      return false;
    }),
  );
};
