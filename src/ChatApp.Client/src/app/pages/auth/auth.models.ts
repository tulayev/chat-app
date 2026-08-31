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

export interface SendCodeForm {
  email: string;
}

export interface VerifyCodeForm extends SendCodeForm {
  code: string;
}

export interface ForgotPasswordForm {
  email: string;
}

export interface ResetPasswordForm {
  email: string;
  code: string;
  newPassword: string;
}
