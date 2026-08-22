import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-avatar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './avatar.component.html'
})
export class AvatarComponent {
  @Input({ required: true }) name!: string;
  @Input() src?: string | null;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';

  get sizeClasses(): string {
    switch (this.size) {
      case 'sm': return 'w-8 h-8 text-xs';
      case 'lg': return 'w-16 h-16 text-xl';
      default: return 'w-10 h-10 text-sm';
    }
  }

  get initial(): string {
    return this.name?.trim()?.charAt(0)?.toUpperCase() || '?';
  }
}
