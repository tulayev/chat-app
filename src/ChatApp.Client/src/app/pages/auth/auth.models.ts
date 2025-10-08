export interface RegisterForm {
  username: string;
  email: string;
  password: string;
  avatar?: File;
}

export interface LoginForm {
  usernameOrEmail: string;
  password: string;
}
