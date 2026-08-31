import { createFeature, createReducer, createSelector, on } from '@ngrx/store';
import { AuthActions } from './auth.actions';
import { initialAuthState } from './auth.state';

export const authFeature = createFeature({
  name: 'auth',
  reducer: createReducer(
    initialAuthState,
    on(AuthActions.login, AuthActions.register, AuthActions.refreshUser, (state) => ({
      ...state,
      loading: true,
      error: null,
    })),
    on(AuthActions.loginSuccess, AuthActions.registerSuccess, (state, { user, token }) => ({
      ...state,
      user,
      token,
      loading: false,
      error: null,
    })),
    on(AuthActions.refreshUserSuccess, (state, { user }) => ({
      ...state,
      user,
      loading: false,
      error: null,
    })),
    on(AuthActions.loginFailure, AuthActions.registerFailure, AuthActions.refreshUserFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })),
    on(AuthActions.logout, (state) => ({
      ...state,
      user: null,
      token: null,
      loading: false,
      error: null,
    })),
  ),
  extraSelectors: ({ selectUser, selectToken }) => ({
    selectIsAuthenticated: createSelector(selectUser, selectToken, (user, token) => !!user && !!token),
    selectEmailConfirmed: createSelector(selectUser, (user) => user?.emailConfirmed ?? false),
  }),
});

export const {
  name: authFeatureKey,
  reducer: authReducer,
  selectUser,
  selectToken,
  selectLoading,
  selectError,
  selectIsAuthenticated,
  selectEmailConfirmed,
} = authFeature;
