import { Directive } from '@angular/core';

@Directive({
  selector: '[elementHighlight]',
  host: {
    '[class.bg-yellow-200]': 'isHighlighted',
    '(mouseenter)': 'onMouseEnter()',
    '(mouseleave)': 'onMouseLeave()',
  }
})
export class ElementHighlightDirective {
  isHighlighted = false;

  onMouseEnter(): void {
    this.isHighlighted = true;
  }

  onMouseLeave(): void {
    this.isHighlighted = false;
  }
}
