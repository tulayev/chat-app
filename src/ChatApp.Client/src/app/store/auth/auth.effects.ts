import { HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, tap } from 'rxjs';
import { AuthService } from '@core/services';
import { AuthActions } from './auth.actions';

const login$ = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService)) => actions$.pipe(
    ofType(AuthActions.login),
    exhaustMap(({ credentials }) => authService.login(credentials).pipe(
      map(({ user, token }) => AuthActions.loginSuccess({ user, token })),
      catchError((err: HttpErrorResponse) => of(AuthActions.loginFailure({
        error: err.error?.errorMessage ?? 'Login failed',
      }))),
    )),
  ),
  { functional: true },
);

const register$ = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService)) => actions$.pipe(
    ofType(AuthActions.register),
    exhaustMap(({ credentials }) => authService.register(credentials).pipe(
      map(({ user, token }) => AuthActions.registerSuccess({ user, token })),
      catchError((err: HttpErrorResponse) => of(AuthActions.registerFailure({
        error: err.error?.errorMessage ?? 'Registration failed',
      }))),
    )),
  ),
  { functional: true },
);

const refreshUser$ = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService)) => actions$.pipe(
    ofType(AuthActions.refreshUser),
    switchMap(() => authService.refreshUser().pipe(
      map((user) => AuthActions.refreshUserSuccess({ user })),
      catchError((err: HttpErrorResponse) => of(AuthActions.refreshUserFailure({
        error: err.error?.errorMessage ?? 'Failed to refresh user',
      }))),
    )),
  ),
  { functional: true },
);

const persistOnAuthSuccess$ = createEffect(
  (actions$ = inject(Actions)) => actions$.pipe(
    ofType(AuthActions.loginSuccess, AuthActions.registerSuccess),
    tap(({ user, token }) => {
      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));
    }),
  ),
  { functional: true, dispatch: false },
);

const persistOnRefreshUserSuccess$ = createEffect(
  (actions$ = inject(Actions)) => actions$.pipe(
    ofType(AuthActions.refreshUserSuccess),
    tap(({ user }) => localStorage.setItem('user', JSON.stringify(user))),
  ),
  { functional: true, dispatch: false },
);

const clearPersistedAuthOnLogout$ = createEffect(
  (actions$ = inject(Actions)) => actions$.pipe(
    ofType(AuthActions.logout),
    tap(() => {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }),
  ),
  { functional: true, dispatch: false },
);

const navigateAfterLogin$ = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) => actions$.pipe(
    ofType(AuthActions.loginSuccess),
    tap(() => router.navigateByUrl('/users')),
  ),
  { functional: true, dispatch: false },
);

const navigateAfterRegister$ = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) => actions$.pipe(
    ofType(AuthActions.registerSuccess),
    tap(({ user }) => router.navigate(['/verify-email'], { queryParams: { email: user.email } })),
  ),
  { functional: true, dispatch: false },
);

const navigateAfterLogout$ = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) => actions$.pipe(
    ofType(AuthActions.logout),
    tap(() => router.navigateByUrl('/login')),
  ),
  { functional: true, dispatch: false },
);

export const authEffects = {
  login$,
  register$,
  refreshUser$,
  persistOnAuthSuccess$,
  persistOnRefreshUserSuccess$,
  clearPersistedAuthOnLogout$,
  navigateAfterLogin$,
  navigateAfterRegister$,
  navigateAfterLogout$,
};
