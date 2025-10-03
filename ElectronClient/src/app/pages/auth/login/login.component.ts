import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@core/services';
import { LoginForm } from '../auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  form: LoginForm = {
    usernameOrEmail: '',
    password: ''
  };
  error = '';

  constructor(
    private readonly router: Router,
    private readonly auth: AuthService) {}

  onSubmit() {
    this.auth.login(this.form).subscribe({
      next: () => this.router.navigate(['/chat']),
      error: err => this.error = err.error?.message || 'Login failed'
    });
  }
}
