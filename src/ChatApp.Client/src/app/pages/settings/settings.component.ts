import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ProfileService } from '@core/services';
import { Store } from '@ngrx/store';
import { AuthActions, selectUser } from '@store/auth';
import { ChangePasswordForm, UpdateProfileForm } from './settings.models';
import { LucideArrowLeft, LucideCamera, LucideLock, LucideUser } from '@lucide/angular';
import { AvatarComponent, TextFieldComponent } from '@shared/components';
import { avatarFileValidator, passwordStrengthValidator } from '@shared/validators';

@Component({
  selector: 'app-settings',
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, AvatarComponent, TextFieldComponent,
    LucideArrowLeft, LucideCamera, LucideLock, LucideUser
  ],
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnDestroy {
  private readonly store = inject(Store);
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);

  readonly currentUser = this.store.selectSignal(selectUser);

  profileForm = new FormGroup({
    username: new FormControl(this.currentUser()?.username ?? '', [Validators.required, Validators.minLength(3)]),
    avatar: new FormControl<File | null>(null, [avatarFileValidator()])
  });
  passwordForm = new FormGroup({
    currentPassword: new FormControl('', [Validators.required]),
    newPassword: new FormControl('', [Validators.required, passwordStrengthValidator()]),
    confirmPassword: new FormControl('', [Validators.required])
  });

  avatarPreviewUrl = signal<string | null>(null);
  profileMessage = '';
  profileMessageType = signal<'success' | 'error'>('success');
  passwordMessage = '';
  passwordMessageType = signal<'success' | 'error'>('success');

  ngOnDestroy(): void {
    this.revokeAvatarPreviewUrl();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.profileForm.controls.avatar.setValue(file);
      this.revokeAvatarPreviewUrl();
      this.avatarPreviewUrl.set(URL.createObjectURL(file));
    }
  }

  onSaveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }
    this.profileMessage = '';

    this.profileService.updateProfile(this.profileForm.value as UpdateProfileForm).subscribe({
      next: () => {
        this.store.dispatch(AuthActions.refreshUser());
        this.profileMessageType.set('success');
        this.profileMessage = 'Profile updated successfully';
      },
      error: (err) => {
        this.profileMessageType.set('error');
        this.profileMessage = err.error?.errorMessage || 'Error updating profile';
      },
    });
  }

  onChangePassword(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    if (this.passwordForm.value.newPassword !== this.passwordForm.value.confirmPassword) {
      this.passwordMessageType.set('error');
      this.passwordMessage = 'Passwords do not match';
      return;
    }

    this.passwordMessage = '';

    this.profileService.changePassword(this.passwordForm.value as ChangePasswordForm).subscribe({
      next: () => {
        this.passwordMessageType.set('success');
        this.passwordMessage = 'Password changed successfully';
        this.passwordForm.reset();
      },
      error: (err) => {
        this.passwordMessageType.set('error');
        this.passwordMessage = err.error?.errorMessage || 'Error changing password';
      },
    });
  }

  onBack(): void {
    this.router.navigate(['/users']);
  }

  private revokeAvatarPreviewUrl(): void {
    const previous = this.avatarPreviewUrl();

    if (previous) {
      URL.revokeObjectURL(previous);
    }
  }
}
