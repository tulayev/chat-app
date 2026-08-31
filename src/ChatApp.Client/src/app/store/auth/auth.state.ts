import { User } from '@app/models';

export interface AuthState {
  user: User | null;
  token: string | null;
  loading: boolean;
  error: string | null;
}

function readPersistedAuth(): Pick<AuthState, 'user' | 'token'> {
  const token = localStorage.getItem('token');
  let user: User | null = null;

  try {
    const raw = localStorage.getItem('user');
    user = raw ? (JSON.parse(raw) as User) : null;
  } catch {
    user = null;
  }

  return { user, token };
}

export const initialAuthState: AuthState = {
  ...readPersistedAuth(),
  loading: false,
  error: null,
};
