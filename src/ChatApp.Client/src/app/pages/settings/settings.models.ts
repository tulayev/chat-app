export interface UpdateProfileForm {
  username: string;
  avatar?: File | null;
}

export interface ChangePasswordForm {
  currentPassword: string;
  newPassword: string;
}
