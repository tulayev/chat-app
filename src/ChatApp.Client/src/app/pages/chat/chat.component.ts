import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatMessage, UserChat } from '@app/models';
import { selectCurrentChat } from '@app/store';
import { Store } from '@ngrx/store';
import * as ChatActions from '@app/store';
import { ChatService } from '@core/services';
import { Destroy } from '@core/utils';
import { filter, map, Observable, take, takeUntil, tap } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './chat.component.html',
  providers: [Destroy]
})
export class ChatComponent implements OnInit {
  userChats$!: Observable<UserChat[]>;
  chatMessages$!: Observable<ChatMessage[]>;
  currentChat$!: Observable<UserChat | null>;
  newMessage = '';

  constructor(
    private readonly store: Store,
    private readonly chatService: ChatService,
    private readonly destroy$: Destroy) { 
      this.currentChat$ = this.store.select(selectCurrentChat);
    }

  async ngOnInit(): Promise<void> {
    // Start SignalR
    await this.chatService.start();

    // Load Chat Users
    this.userChats$ = this.chatService.loadUserChats().pipe(map(({ data }) => data));

    // Rehydrate saved chat from localStorage
    const savedChat = localStorage.getItem('currentChat');
    if (savedChat) {
      const chat = JSON.parse(savedChat) as UserChat;
      this.store.dispatch(ChatActions.setCurrentChat({ chat }));
    }

    // Subscribe to current chat and handle joining/loading
    this.currentChat$
      .pipe(
        takeUntil(this.destroy$),
        filter((chat): chat is UserChat => !!chat),
        tap(chat => {
          this.chatService.joinChat(chat.chatId);
          this.chatService.loadChatMessages(chat.chatId).subscribe();
          this.chatMessages$ = this.chatService.messages$;
        })
      )
      .subscribe();
  }

  onUserChatClick(chat: UserChat): void {
    // Leave previous chat before switching
    this.currentChat$
      .pipe(take(1))
      .subscribe(current => {
        if (current?.chatId) {
          this.chatService.leaveChat(current.chatId);
        }
      });

    // Select new chat
    this.store.dispatch(ChatActions.setCurrentChat({ chat }));
  }

  onSendClick(): void {
    this.currentChat$
      .pipe(take(1))
      .subscribe(chat => {
        if (!chat || !this.newMessage.trim()) {
          return;
        }

        this.chatService
          .sendPrivateMessage(chat.chatId, this.newMessage)
          .subscribe(() => (this.newMessage = ''));
      });
  }
}
