import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordStrengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as string;
    if (!value) {
      return null;
    }

    const errors: ValidationErrors = {};
    if (value.length < 6) {
      errors['minlength'] = true;
    }
    if (!/[A-Z]/.test(value)) {
      errors['uppercase'] = true;
    }
    if (!/\d/.test(value)) {
      errors['digit'] = true;
    }
    if (!/[!@#$%^&*(),.?":{}|<>]/.test(value)) {
      errors['specialChar'] = true;
    }

    return Object.keys(errors).length ? errors : null;
  };
}
