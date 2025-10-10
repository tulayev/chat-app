import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse, ChatHistory } from '@app/models';
import { AuthService } from '@core/services/auth.service';
import { environment } from '@environments/environment';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subscription } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private messagesSource = new BehaviorSubject<ChatHistory[]>([]);
  messages$ = this.messagesSource.asObservable();
  private readonly chatHubUrl = `${environment.baseUrl}/hubs/chat`;
  private readonly apiUrl = `${environment.apiUrl}`;
  private readonly hubConnection: signalR.HubConnection;

  constructor(
    private readonly auth: AuthService,
    private readonly http: HttpClient) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.chatHubUrl, {
        accessTokenFactory: () => this.auth.user?.token ?? ''
      })
      .withAutomaticReconnect()
      .build();
  }

  async start(): Promise<void> {
    await this.hubConnection.start();
    console.log("Connected to SignalR");

    this.hubConnection.on('ReceiveMessage', (senderId, content, sentAt) => {
      const realTimeMessage: ChatHistory = {
        id: 0,
        content: content,
        sentAt: sentAt,
        from: { id: senderId, username: '', email: '', avatarUrl: '' },
        to: { id: 0, username: '', email: '', avatarUrl: '' },
      };
      this.messagesSource.next([...this.messagesSource.value, realTimeMessage]);
    });
  }

  sendPrivateMessage(receiverId: number, content: string): void {
    this.hubConnection.invoke('SendPrivateMessage', receiverId, content);
  }

  loadChatHistory(withUserId: number): Subscription {
    return this.http.get<ApiResponse<ChatHistory[]>>(`${this.apiUrl}/chat/history/${withUserId}`)
      .subscribe(({ data }) => {
        this.messagesSource.next(data);
      });
  }
}
