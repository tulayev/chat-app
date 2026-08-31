import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { User } from '@app/models';
import { LoginForm, RegisterForm } from '@pages/auth';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    'Login': props<{ credentials: LoginForm }>(),
    'Login Success': props<{ user: User; token: string }>(),
    'Login Failure': props<{ error: string }>(),

    'Register': props<{ credentials: RegisterForm }>(),
    'Register Success': props<{ user: User; token: string }>(),
    'Register Failure': props<{ error: string }>(),

    'Refresh User': emptyProps(),
    'Refresh User Success': props<{ user: User }>(),
    'Refresh User Failure': props<{ error: string }>(),

    'Logout': emptyProps(),
  },
});
