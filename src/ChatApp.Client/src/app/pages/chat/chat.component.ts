import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Message, UserChat } from '@app/models';
import { selectCurrentChat } from '@app/store';
import { Store } from '@ngrx/store';
import * as ChatActions from '@app/store';
import { AuthService, ChatService } from '@core/services';
import { Destroy } from '@core/utils';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, map, Observable, take, takeUntil, tap } from 'rxjs';
import {
  LucideMessageCircle, LucidePanelLeftClose, LucidePanelLeftOpen, LucideArrowLeft, LucideSend, LucideLogOut
} from '@lucide/angular';
import { AvatarComponent } from '@shared/components';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    FormsModule, CommonModule, AvatarComponent,
    LucideMessageCircle, LucidePanelLeftClose, LucidePanelLeftOpen, LucideArrowLeft, LucideSend, LucideLogOut
  ],
  templateUrl: './chat.component.html',
  providers: [Destroy]
})
export class ChatComponent implements OnInit, OnDestroy {
  messages$!: Observable<Message[]>;
  newMessage = '';
  sidebarCollapsed = signal(false);
  receiverId = signal(0);

  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly store = inject(Store);
  private readonly chatService = inject(ChatService);
  private readonly destroy$ = inject(Destroy);
  readonly currentUser = this.authService.user;

  async ngOnInit(): Promise<void> {
    this.activatedRoute.paramMap.subscribe(params => {
      const userId = params.get('userId');
      if (!userId) {
        this.router.navigateByUrl('/not-found');
        return;
      }

      this.loadMessages(+userId);
    });

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
    this.currentChat$.pipe(
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
  loadMessages(receiverId: number) {
    this.chatService.loadMessagesWith(receiverId)
  }

  ngOnDestroy(): void {
    this.currentChat$
      .pipe(take(1))
      .subscribe(current => {
        if (current?.chatId) {
          this.chatService.leaveChat(current.chatId);
        }
      });

    this.chatService.stop();
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

  onBack(): void {
    this.currentChat$
      .pipe(take(1))
      .subscribe(current => {
        if (current?.chatId) {
          this.chatService.leaveChat(current.chatId);
        }
      });

    this.store.dispatch(ChatActions.clearCurrentChat());
  }

  onLogout(): void {
    this.currentChat$
      .pipe(take(1))
      .subscribe(current => {
        if (current?.chatId) {
          this.chatService.leaveChat(current.chatId);
        }
      });

    this.chatService.stop();
    this.store.dispatch(ChatActions.clearCurrentChat());
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
