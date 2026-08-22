import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LucideHouse, LucideMessageCircle } from '@lucide/angular';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideHouse, LucideMessageCircle],
  templateUrl: './not-found.component.html'
})
export class NotFoundComponent {

}
