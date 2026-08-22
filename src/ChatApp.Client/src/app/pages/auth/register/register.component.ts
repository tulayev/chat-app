import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterForm } from '../auth.models';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@core/services';
import { LucideUser, LucideMail, LucideLock, LucideEye, LucideEyeOff, LucideCamera, LucideMessageCircle, LucideCircleAlert } from '@lucide/angular';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule,
    LucideUser, LucideMail, LucideLock, LucideEye, LucideEyeOff, LucideCamera, LucideMessageCircle, LucideCircleAlert
  ],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    avatar: new FormControl()
  });
  error = '';
  showPassword = signal(false);
  avatarPreviewUrl = signal<string | null>(null);

  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.form.value.avatar = input.files[0];
      this.avatarPreviewUrl.set(URL.createObjectURL(input.files[0]));
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.register(this.form.value as RegisterForm).subscribe({
      next: () => this.router.navigate(['/verify-email'], { queryParams: { email: this.form.value.email } }),
      error: err => this.error = err.error?.message || 'Registration failed'
    });
  }
}
