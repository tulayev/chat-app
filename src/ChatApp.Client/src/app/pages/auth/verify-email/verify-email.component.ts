import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EmailVerificationService } from '@app/core/services';
import { SendCodeForm, VerifyCodeForm } from '../auth.models';
import { LucideMail, LucideKeyRound, LucideCircleCheck, LucideCircleAlert } from '@lucide/angular';

@Component({
  selector: 'app-verify-email',
  imports: [CommonModule, FormsModule, RouterModule, LucideMail, LucideKeyRound, LucideCircleCheck, LucideCircleAlert],
  templateUrl: './verify-email.component.html'
})
export class VerifyEmail {
  sendCodeForm: SendCodeForm = {
    email: ''
  };
  verifyCodeForm: VerifyCodeForm = {
    email: '',
    code: ''
  };
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
        this.sendCodeForm.email = p['email'];
      }
    });
  }

  onSendCode() {
    if (!this.sendCodeForm.email) {
      return;
    }
    this.message = '';

    this.auth.sendVerificationCode(this.sendCodeForm).subscribe({
      next: () => {
        this.sent = true;
        this.verifyCodeForm.email = this.sendCodeForm.email;
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
    if (!this.verifyCodeForm.code) {
      return;
    }
    this.message = '';

    this.auth.verifyEmail(this.verifyCodeForm).subscribe({
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
