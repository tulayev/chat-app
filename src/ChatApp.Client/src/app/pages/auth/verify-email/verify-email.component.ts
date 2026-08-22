import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EmailVerificationService } from '@app/core/services';
import { SendCodeForm, VerifyCodeForm } from '../auth.models';
import { LucideMail, LucideKeyRound, LucideCircleCheck, LucideCircleAlert } from '@lucide/angular';
import { TextFieldComponent } from '@shared/components';
import { Destroy } from '@core/utils';
import { takeUntil } from 'rxjs';

@Component({
  selector: 'app-verify-email',
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, TextFieldComponent,
    LucideMail, LucideKeyRound, LucideCircleCheck, LucideCircleAlert
  ],
  templateUrl: './verify-email.component.html',
  providers: [Destroy]
})
export class VerifyEmail implements OnDestroy {
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

  private redirectTimeout?: ReturnType<typeof setTimeout>;
  private readonly emailVerificationService = inject(EmailVerificationService);
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly destroy$ = inject(Destroy);

  constructor() {
    this.activatedRoute.queryParams.pipe(takeUntil(this.destroy$))
      .subscribe(p => {
        if (p['email']) {
          this.sendCodeForm.patchValue({ email: p['email'] });
        }
      });
  }

  ngOnDestroy(): void {
    if (this.redirectTimeout) {
      clearTimeout(this.redirectTimeout);
    }
  }

  onSendCode() {
    if (this.sendCodeForm.invalid) {
      this.sendCodeForm.markAllAsTouched();
      return;
    }
    this.message = '';

    this.emailVerificationService.sendVerificationCode(this.sendCodeForm.value as SendCodeForm).subscribe({
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

    this.emailVerificationService.verifyEmail(this.verifyCodeForm.value as VerifyCodeForm).subscribe({
      next: () => {
        this.messageType.set('success');
        this.message = 'Email successfully verified!';
        this.redirectTimeout = setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err) => {
        this.messageType.set('error');
        this.message = err.error || 'Incorrect code';
      },
    });
  }
}
