import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@core/services';
import { LoginForm } from '../auth.models';
import { LucideUser, LucideLock, LucideEye, LucideEyeOff, LucideMessageCircle, LucideCircleAlert } from '@lucide/angular';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule,
    LucideUser, LucideLock, LucideEye, LucideEyeOff, LucideMessageCircle, LucideCircleAlert
  ],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  form = new FormGroup({
    usernameOrEmail: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });
  error = '';
  showPassword = signal(false);

  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.login(this.form.value as LoginForm).subscribe({
      next: () => this.router.navigate(['/chat']),
      error: err => this.error = err.error?.message || 'Login failed'
    });
  }
}
