import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { User, UserChat } from '@app/models';
import { AuthService, ChatService } from '@core/services';
import { ActivatedRoute, Router } from '@angular/router';
import { EMPTY, map, Observable, switchMap } from 'rxjs';
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
  templateUrl: './chat.component.html'
})
export class ChatComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly chatService = inject(ChatService);
  private readonly destroyRef = inject(DestroyRef);

  readonly currentUser = this.authService.user;
  readonly chatMessages$ = this.chatService.messages$;

  userChats$!: Observable<UserChat[]>;
  contact = signal<User | null>(null);
  chatId = signal<number | null>(null);
  mobileView = signal<'list' | 'chat'>('chat');
  sidebarCollapsed = signal(false);
  newMessage = '';

  ngOnInit(): void {
    this.userChats$ = this.chatService.getUserChats().pipe(
      map(response => response.data)
    );

    const hubReady = this.chatService.start();

    this.activatedRoute.paramMap
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map(params => params.get('userId')),
        switchMap(userId => {
          if (!userId || isNaN(Number(userId))) {
            this.router.navigateByUrl('/not-found');
            return EMPTY;
          }
          
          return this.chatService.getChatMessages(+userId);
        })
      )
      .subscribe(({ data }) => {
        this.leaveCurrentChat();
        this.chatId.set(data.chatId);
        this.contact.set(data.contact);
        this.mobileView.set('chat');
        hubReady
          .then(() => this.chatService.joinChat(data.chatId))
          .catch(err => console.error('Failed to join chat', err));
      });
  }

  ngOnDestroy(): void {
    this.leaveCurrentChat();
    this.chatService.stop();
  }

  onUserChatClick(chat: UserChat): void {
    this.mobileView.set('chat');
    this.router.navigate(['/chat', chat.contact.id]);
  }

  onSendClick(): void {
    const chatId = this.chatId();
    if (!chatId || !this.newMessage.trim()) {
      return;
    }

    this.chatService
      .sendPrivateMessage(chatId, this.newMessage)
      .subscribe(() => (this.newMessage = ''));
  }

  onBack(): void {
    this.mobileView.set('list');
  }

  async onLogout(): Promise<void> {
    this.leaveCurrentChat();
    await this.chatService.stop();
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  private leaveCurrentChat(): void {
    const chatId = this.chatId();

    if (chatId) {
      this.chatService.leaveChat(chatId);
    }
  }
}
