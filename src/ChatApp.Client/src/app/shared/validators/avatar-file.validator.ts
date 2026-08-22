import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const MAX_AVATAR_SIZE_BYTES = 5 * 1024 * 1024;

export function avatarFileValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const file = control.value as File | null;
    if (!file) {
      return null;
    }

    const errors: ValidationErrors = {};
    if (!file.type.startsWith('image/')) {
      errors['invalidType'] = true;
    }
    if (file.size > MAX_AVATAR_SIZE_BYTES) {
      errors['tooLarge'] = true;
    }

    return Object.keys(errors).length ? errors : null;
  };
}
