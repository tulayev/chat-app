import { CommonModule } from '@angular/common';
import { Component, OnDestroy, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterForm } from '../auth.models';
import { RouterModule } from '@angular/router';
import { Store } from '@ngrx/store';
import { AuthActions } from '@store/auth';
import { LucideUser, LucideMail, LucideLock, LucideCamera, LucideMessageCircle } from '@lucide/angular';
import { TextFieldComponent, FieldErrorComponent } from '@shared/components';
import { avatarFileValidator, passwordStrengthValidator } from '@shared/validators';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, TextFieldComponent, FieldErrorComponent,
    LucideUser, LucideMail, LucideLock, LucideCamera, LucideMessageCircle
  ],
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnDestroy {
  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    username: new FormControl('', [Validators.required, Validators.minLength(3)]),
    password: new FormControl('', [Validators.required, passwordStrengthValidator()]),
    avatar: new FormControl<File | null>(null, [avatarFileValidator()])
  });
  avatarPreviewUrl = signal<string | null>(null);

  private readonly store = inject(Store);

  ngOnDestroy(): void {
    this.revokeAvatarPreviewUrl();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.form.controls.avatar.setValue(file);
      this.revokeAvatarPreviewUrl();
      this.avatarPreviewUrl.set(URL.createObjectURL(file));
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.store.dispatch(AuthActions.register({ credentials: this.form.value as RegisterForm }));
  }

  private revokeAvatarPreviewUrl(): void {
    const previous = this.avatarPreviewUrl();

    if (previous) {
      URL.revokeObjectURL(previous);
    }
  }
}
