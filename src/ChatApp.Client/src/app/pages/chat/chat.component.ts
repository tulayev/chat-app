import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthUser, ChatContact, ChatHistory } from '@app/models';
import { AuthService, ChatService } from '@core/services';
import { map, Observable } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './chat.component.html'
})
export class ChatComponent implements OnInit {
  chatContacts$!: Observable<ChatContact[]>;
  messages$!: Observable<ChatHistory[]>;
  user!: AuthUser | null;
  chatContactId!: number;
  newMessage = '';

  constructor(
    private readonly chatService: ChatService,
    private readonly authService: AuthService
  ) { }

  async ngOnInit(): Promise<void> {
    await this.loadData();
  }

  send(): void {
    if (!this.newMessage.trim()) {
      return;
    }

    this.chatService.sendPrivateMessage(this.chatContactId, this.newMessage);
    this.newMessage = '';
  }

  onChatContactClick(userId: number): void {
    this.chatContactId = userId;
    this.chatService.loadChatHistory(this.chatContactId);
    this.messages$ = this.chatService.messages$;
  }

  private async loadData(): Promise<void> {
    this.user = this.authService.user;
    // contacts
    this.chatContacts$ = this.chatService.loadChatContacts().pipe(
      map(({ data }) => data)
    );
    // start signalR for real-time chat
    await this.chatService.start();
  }
}
