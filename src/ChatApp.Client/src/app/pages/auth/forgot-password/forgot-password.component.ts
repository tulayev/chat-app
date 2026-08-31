import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { PasswordResetService } from '@core/services';
import { ForgotPasswordForm } from '../auth.models';
import { LucideMail, LucideCircleCheck, LucideCircleAlert } from '@lucide/angular';
import { TextFieldComponent } from '@shared/components';

@Component({
  selector: 'app-forgot-password',
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, TextFieldComponent,
    LucideMail, LucideCircleCheck, LucideCircleAlert
  ],
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent {
  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email])
  });
  message = '';
  messageType = signal<'success' | 'error'>('success');

  private readonly passwordResetService = inject(PasswordResetService);
  private readonly router = inject(Router);

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.message = '';

    this.passwordResetService.forgotPassword(this.form.value as ForgotPasswordForm).subscribe({
      next: () => this.router.navigate(['/reset-password'], { queryParams: { email: this.form.value.email } }),
      error: (err) => {
        this.messageType.set('error');
        this.message = err.error?.errorMessage || 'Error sending reset code';
      },
    });
  }
}
