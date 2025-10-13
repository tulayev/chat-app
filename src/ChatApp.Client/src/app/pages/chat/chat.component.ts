import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatMessage, UserChat } from '@app/models';
import { ChatService } from '@core/services';
import { map, Observable } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './chat.component.html'
})
export class ChatComponent implements OnInit {
  userChats$!: Observable<UserChat[]>;
  chatMessages$!: Observable<ChatMessage[]>;
  currentChat!: UserChat;
  newMessage = '';

  constructor(private readonly chatService: ChatService) { }

  async ngOnInit(): Promise<void> {
    await this.loadData();
  }

  onUserChatClick(chat: UserChat): void {
    if (this.currentChat?.chatId) {
      this.chatService.leaveChat(this.currentChat.chatId);
    }

    this.currentChat = chat;
    this.chatService.joinChat(this.currentChat.chatId);

    this.chatService.loadChatMessages(this.currentChat.chatId).subscribe();
    this.chatMessages$ = this.chatService.messages$;
  }

  onSendClick(): void {
    if (!this.newMessage.trim()) {
      return;
    }

    this.chatService.sendPrivateMessage(this.currentChat.chatId, this.newMessage)
      .subscribe(() => {
        this.newMessage = '';
      });
  }

  private async loadData(): Promise<void> {
    // contacts
    this.userChats$ = this.chatService.loadUserChats().pipe(
      map(({ data }) => data)
    );
    // start signalR for real-time chat
    await this.chatService.start();
  }
}
