import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, inject, viewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Store } from '@ngrx/store';
import { AuthActions } from '@store/auth';
import { GoogleIdentityService } from '@core/services';
import { LoginForm } from '../auth.models';
import { LucideUser, LucideLock, LucideMessageCircle } from '@lucide/angular';
import { TextFieldComponent } from '@shared/components';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, TextFieldComponent,
    LucideUser, LucideLock, LucideMessageCircle
  ],
  templateUrl: './login.component.html'
})
export class LoginComponent implements AfterViewInit {
  form = new FormGroup({
    usernameOrEmail: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  private readonly googleBtn = viewChild.required<ElementRef<HTMLDivElement>>('googleBtn');
  private readonly store = inject(Store);
  private readonly googleIdentityService = inject(GoogleIdentityService);

  ngAfterViewInit(): void {
    const host = this.googleBtn().nativeElement;

    this.googleIdentityService.renderButton(host, Math.min(400, Math.max(200, host.clientWidth)));
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.store.dispatch(AuthActions.login({ credentials: this.form.value as LoginForm }));
  }
}
