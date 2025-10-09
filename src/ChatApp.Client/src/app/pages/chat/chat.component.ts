import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatHistory } from '@app/models';
import { ChatService } from '@core/services';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './chat.component.html'
})
export class ChatComponent implements OnInit {
  messages$!: Observable<ChatHistory[]>;
  newMessage = '';
  recepientId = 2; // TODO: dynamically choose recepient

  constructor(private readonly chatService: ChatService) { }

  async ngOnInit(): Promise<void> {
    this.messages$ = this.chatService.messages$;
    this.chatService.loadChatHistory(this.recepientId);
    // start signalR for real-time chat
    await this.chatService.start();
  }

  send(): void {
    if (!this.newMessage.trim()) {
      return;
    }

    this.chatService.sendPrivateMessage(this.recepientId, this.newMessage);
    this.newMessage = '';
  }
}
