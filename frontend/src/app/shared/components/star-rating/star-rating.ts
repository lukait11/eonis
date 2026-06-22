import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-star-rating',
  template: `
    <div class="stars" [class.interactive]="interactive">
      @for (star of [1,2,3,4,5]; track star) {
        <button
          type="button"
          class="star"
          [class.filled]="star <= (hovered ?? value)"
          (mouseenter)="interactive && (hovered = star)"
          (mouseleave)="interactive && (hovered = null)"
          (click)="interactive && onSelect(star)"
          [disabled]="!interactive"
        >★</button>
      }
    </div>
  `,
  styles: [`
    .stars { display: flex; gap: 2px; }
    .star {
      font-size: 1.25rem;
      color: var(--border);
      background: none;
      border: none;
      cursor: default;
      padding: 0;
      line-height: 1;
      transition: color var(--transition);
    }
    .interactive .star { cursor: pointer; }
    .star.filled { color: var(--accent); }
    .star:disabled { cursor: default; }
  `],
})
export class StarRating {
  @Input() value = 0;
  @Input() interactive = false;
  @Output() rated = new EventEmitter<number>();

  hovered: number | null = null;

  onSelect(star: number): void {
    this.value = star;
    this.rated.emit(star);
  }
}
