export interface AuthUser {
  id: number;
  token: string;
  username: string;
  email: string;
  avatarUrl: string;
}

export interface User {
  id: number;
  username: string;
  email: string;
  avatarUrl: string;
}
