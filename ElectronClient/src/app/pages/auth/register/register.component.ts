import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RegisterForm } from '../auth.models';
import { Router } from '@angular/router';
import { AuthService } from '@core/services';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  form: RegisterForm = {
    email: '',
    username: '',
    password: '',
    avatar: undefined
  };
  error = '';

  constructor(
    private readonly router: Router,
    private readonly auth: AuthService) { }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.form.avatar = input.files[0];
    }
  }

  onSubmit() {
    this.auth.register(this.form).subscribe({
      next: () => this.router.navigate(['/chat']),
      error: err => this.error = err.error?.message || 'Registration failed'
    });
  }
}
