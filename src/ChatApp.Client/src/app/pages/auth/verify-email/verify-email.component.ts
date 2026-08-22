import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EmailVerificationService } from '@app/core/services';
import { SendCodeForm, VerifyCodeForm } from '../auth.models';
import { LucideMail, LucideKeyRound, LucideCircleCheck, LucideCircleAlert } from '@lucide/angular';
import { TextFieldComponent } from '@shared/components';

@Component({
  selector: 'app-verify-email',
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, TextFieldComponent,
    LucideMail, LucideKeyRound, LucideCircleCheck, LucideCircleAlert
  ],
  templateUrl: './verify-email.component.html'
})
export class VerifyEmail {
  sendCodeForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email])
  });
  verifyCodeForm = new FormGroup({
    email: new FormControl(''),
    code: new FormControl('', [Validators.required, Validators.pattern(/^\d{6}$/)])
  });
  message = '';
  messageType = signal<'success' | 'error'>('success');
  sent = false;

  constructor(
    private readonly auth: EmailVerificationService,
    private readonly router: Router,
    private readonly route: ActivatedRoute
  ) {
    this.route.queryParams.subscribe(p => {
      if (p['email']) {
        this.sendCodeForm.patchValue({ email: p['email'] });
      }
    });
  }

  onSendCode() {
    if (this.sendCodeForm.invalid) {
      this.sendCodeForm.markAllAsTouched();
      return;
    }
    this.message = '';

    this.auth.sendVerificationCode(this.sendCodeForm.value as SendCodeForm).subscribe({
      next: () => {
        this.sent = true;
        this.verifyCodeForm.patchValue({ email: this.sendCodeForm.value.email });
        this.messageType.set('success');
        this.message = 'Code sent to you email';
      },
      error: (err) => {
        this.messageType.set('error');
        this.message = err.error || 'Error sending code';
      },
    });
  }

  onVerify() {
    if (this.verifyCodeForm.invalid) {
      this.verifyCodeForm.markAllAsTouched();
      return;
    }
    this.message = '';

    this.auth.verifyEmail(this.verifyCodeForm.value as VerifyCodeForm).subscribe({
      next: () => {
        this.messageType.set('success');
        this.message = 'Email successfully verified!';
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err) => {
        this.messageType.set('error');
        this.message = err.error || 'Incorrect code';
      },
    });
  }
}
