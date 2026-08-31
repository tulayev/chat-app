import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PasswordResetService } from '@core/services';
import { ResetPasswordForm } from '../auth.models';
import { LucideKeyRound, LucideLock, LucideCircleCheck, LucideCircleAlert } from '@lucide/angular';
import { TextFieldComponent } from '@shared/components';
import { passwordStrengthValidator } from '@shared/validators';

@Component({
  selector: 'app-reset-password',
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, TextFieldComponent,
    LucideKeyRound, LucideLock, LucideCircleCheck, LucideCircleAlert
  ],
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordComponent {
  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    code: new FormControl('', [Validators.required, Validators.pattern(/^\d{6}$/)]),
    newPassword: new FormControl('', [Validators.required, passwordStrengthValidator()]),
    confirmPassword: new FormControl('', [Validators.required])
  });
  message = '';
  messageType = signal<'success' | 'error'>('success');

  private readonly passwordResetService = inject(PasswordResetService);
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);

  constructor() {
    this.activatedRoute.queryParams.pipe(takeUntilDestroyed())
      .subscribe(queryParams => {
        if (queryParams['email']) {
          this.form.patchValue({ email: queryParams['email'] });
        }
      });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.form.value.newPassword !== this.form.value.confirmPassword) {
      this.messageType.set('error');
      this.message = 'Passwords do not match';
      return;
    }

    this.message = '';

    this.passwordResetService.resetPassword(this.form.value as ResetPasswordForm).subscribe({
      next: () => {
        this.messageType.set('success');
        this.message = 'Password reset successful. Redirecting to login…';
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err) => {
        this.messageType.set('error');
        this.message = err.error?.errorMessage || 'Error resetting password';
      },
    });
  }
}
