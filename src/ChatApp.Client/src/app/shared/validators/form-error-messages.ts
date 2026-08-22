import { ValidationErrors } from '@angular/forms';

export const DEFAULT_ERROR_MESSAGES: Record<string, (err: any) => string> = {
  required: () => 'This field is required.',
  email: () => 'Enter a valid email address.',
  minlength: (err) => typeof err === 'object' && err?.requiredLength
    ? `Must be at least ${err.requiredLength} characters.` : 'Must be at least 6 characters.',
  pattern: () => 'Invalid format.',
  uppercase: () => 'Must contain an uppercase letter.',
  digit: () => 'Must contain a digit.',
  specialChar: () => 'Must contain a special character.',
  invalidType: () => 'File must be an image.',
  tooLarge: () => 'Image must be smaller than 5MB.',
};

export function getErrorMessage(errors: ValidationErrors, overrides?: Record<string, string>): string {
  const key = Object.keys(errors)[0];
  if (overrides?.[key]) {
    return overrides[key];
  }
  return DEFAULT_ERROR_MESSAGES[key]?.(errors[key]) ?? 'Invalid value.';
}
