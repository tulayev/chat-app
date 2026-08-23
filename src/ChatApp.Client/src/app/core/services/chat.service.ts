import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse, ChatMessages, Message, UserChat } from '@app/models';
import { AuthService } from '@core/services/auth.service';
import { environment } from 'environments/environment';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private messagesSource = new BehaviorSubject<Message[]>([]);
  messages$ = this.messagesSource.asObservable();
  private readonly chatHubUrl = `${environment.baseUrl}/hubs/chat`;
  private readonly apiUrl = `${environment.apiUrl}`;
  private hubConnection!: signalR.HubConnection;
  private rejoinChatId: number | null = null;

  constructor(
    private readonly auth: AuthService,
    private readonly http: HttpClient) { }

  async start(): Promise<void> {
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.chatHubUrl, {
        accessTokenFactory: () => this.auth.user?.token ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      const current = this.messagesSource.value;
      this.messagesSource.next([...current, message]);
    });

    this.hubConnection.onreconnected(() => {
      const chatId = this.rejoinChatId;
      if (chatId != null) {
        this.joinChat(chatId);
      }
    });

    await this.hubConnection.start();
  }

  async stop(): Promise<void> {
    if (!this.hubConnection) {
      return;
    }

    this.rejoinChatId = null;
    await this.hubConnection.stop();
  }

  loadUserChats(): Observable<ApiResponse<UserChat[]>> {
    return this.http.get<ApiResponse<UserChat[]>>(`${this.apiUrl}/chat/userchats`);
  }

  loadChatMessages(userId: number): Observable<ApiResponse<ChatMessages>> {
    return this.http.get<ApiResponse<ChatMessages>>(`${this.apiUrl}/chat/messages/with/${userId}`)
      .pipe(
        tap(({ data }) => this.messagesSource.next(data.messages))
      );
  }

  joinChat(chatId: number): void {
    this.rejoinChatId = chatId;
    this.hubConnection.invoke('JoinChat', chatId).catch(err => console.error('JoinChat failed', err));
  }

  leaveChat(chatId: number): void {
    if (this.rejoinChatId === chatId) {
      this.rejoinChatId = null;
    }
    this.hubConnection.invoke('LeaveChat', chatId).catch(err => console.error('LeaveChat failed', err));
  }

  sendPrivateMessage(chatId: number, content: string): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/chat/sendmessage`, {
      chatId,
      content
    });
  }
}
