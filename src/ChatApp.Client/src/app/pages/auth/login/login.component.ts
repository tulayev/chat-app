import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@core/services';
import { LoginForm } from '../auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  form = new FormGroup({
    usernameOrEmai: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });
  error = '';

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
