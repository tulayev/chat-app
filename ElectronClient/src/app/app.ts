import { Component, signal } from '@angular/core';
import { ChatComponent } from "./features/chat/pages/chat/chat.component";

@Component({
  selector: 'app-root',
  imports: [ChatComponent],
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('ElectronClient');
}
