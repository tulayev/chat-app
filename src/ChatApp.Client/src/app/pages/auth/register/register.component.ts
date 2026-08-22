import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterForm } from '../auth.models';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@core/services';
import { LucideUser, LucideMail, LucideLock, LucideCamera, LucideMessageCircle } from '@lucide/angular';
import { TextFieldComponent, FieldErrorComponent } from '@shared/components';
import { avatarFileValidator, passwordStrengthValidator } from '@shared/validators';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, TextFieldComponent, FieldErrorComponent,
    LucideUser, LucideMail, LucideLock, LucideCamera, LucideMessageCircle
  ],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    username: new FormControl('', [Validators.required, Validators.minLength(3)]),
    password: new FormControl('', [Validators.required, passwordStrengthValidator()]),
    avatar: new FormControl<File | null>(null, [avatarFileValidator()])
  });
  avatarPreviewUrl = signal<string | null>(null);

  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.form.controls.avatar.setValue(file);
      this.avatarPreviewUrl.set(URL.createObjectURL(file));
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.register(this.form.value as RegisterForm).subscribe({
      next: () => this.router.navigate(['/verify-email'], { queryParams: { email: this.form.value.email } })
    });
  }
}
