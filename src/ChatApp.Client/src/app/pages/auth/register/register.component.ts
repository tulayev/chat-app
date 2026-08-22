import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterForm } from '../auth.models';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@core/services';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    avatar: new FormControl()
  });
  error = '';

  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.form.value.avatar = input.files[0];
    }
  }

  onSubmit(): void {
    console.log(this.form.value)
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.register(this.form.value as RegisterForm).subscribe({
      next: () => this.router.navigate(['/verify-email'], { queryParams: { emaiil: this.form.value.email } }),
      error: err => this.error = err.error?.message || 'Registration failed'
    });
  }
}
