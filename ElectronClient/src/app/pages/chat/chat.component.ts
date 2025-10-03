import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatMessage } from '@app/models';
import { ChatService } from '@core/services';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './chat.component.html'
})
export class ChatComponent {
  messages: ChatMessage[] = [];
  newMessage = '';
  recepientId = 2; // TODO: dynamically choose recepient

  constructor(private readonly chatService: ChatService) { }

  async ngOnInit() {
    await this.chatService.start();
    this.chatService.messages$.subscribe(messages => this.messages = messages);
  }

  send() {
    if (!this.newMessage.trim()) {
      return;
    }

    this.chatService.sendPrivateMessage(this.recepientId, this.newMessage);
    this.newMessage = '';
  }
}
