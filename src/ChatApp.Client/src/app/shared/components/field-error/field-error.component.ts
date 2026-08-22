import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { getErrorMessage } from '@shared/validators';

@Component({
  selector: 'app-field-error',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './field-error.component.html'
})
export class FieldErrorComponent {
  @Input({ required: true }) control!: AbstractControl | null;
  @Input() messages?: Record<string, string>;

  get shouldShow(): boolean {
    return !!this.control && this.control.invalid && (this.control.touched || this.control.dirty);
  }

  get message(): string {
    return this.control?.errors ? getErrorMessage(this.control.errors, this.messages) : '';
  }
}
