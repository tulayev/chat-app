import { CommonModule } from '@angular/common';
import { Component, inject, Input, signal } from '@angular/core';
import { AbstractControl, ControlValueAccessor, NgControl } from '@angular/forms';
import { LucideEye, LucideEyeOff } from '@lucide/angular';
import { FieldErrorComponent } from '../field-error/field-error.component';

@Component({
  selector: 'app-text-field',
  standalone: true,
  imports: [CommonModule, FieldErrorComponent, LucideEye, LucideEyeOff],
  templateUrl: './text-field.component.html'
})
export class TextFieldComponent implements ControlValueAccessor {
  @Input({ required: true }) label!: string;
  @Input() id = '';
  @Input() type: 'text' | 'email' | 'password' = 'text';
  @Input() placeholder = '';
  @Input() autocomplete = 'off';
  @Input() icon = true;
  @Input() inputClass = '';
  @Input() errorMessages?: Record<string, string>;

  value = '';
  disabled = false;
  showPassword = signal(false);

  onChange: (value: string) => void = () => {};
  onTouched: () => void = () => {};

  private readonly ngControl = inject(NgControl, { optional: true, self: true });

  constructor() {
    if (this.ngControl) {
      this.ngControl.valueAccessor = this;
    }
  }

  get control(): AbstractControl | null {
    return this.ngControl?.control ?? null;
  }

  get inputType(): string {
    if (this.type !== 'password') {
      return this.type;
    }
    return this.showPassword() ? 'text' : 'password';
  }

  writeValue(value: string): void {
    this.value = value ?? '';
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(disabled: boolean): void {
    this.disabled = disabled;
  }

  handleInput(event: Event): void {
    this.value = (event.target as HTMLInputElement).value;
    this.onChange(this.value);
  }
}
