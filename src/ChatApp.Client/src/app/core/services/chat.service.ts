import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse, UserChat, ChatMessage } from '@app/models';
import { AuthService } from '@core/services/auth.service';
import { environment } from '@environments/environment';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private messagesSource = new BehaviorSubject<ChatMessage[]>([]);
  messages$ = this.messagesSource.asObservable();
  private readonly chatHubUrl = `${environment.baseUrl}/hubs/chat`;
  private readonly apiUrl = `${environment.apiUrl}`;
  private hubConnection!: signalR.HubConnection;

  constructor(
    private readonly auth: AuthService,
    private readonly http: HttpClient) { }

  async start(): Promise<void> {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.chatHubUrl, {
        accessTokenFactory: () => this.auth.user?.token ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      const current = this.messagesSource.value;
      this.messagesSource.next([...current, message]);
    });

    await this.hubConnection.start();
  }

  loadUserChats(): Observable<ApiResponse<UserChat[]>> {
    return this.http.get<ApiResponse<UserChat[]>>(`${this.apiUrl}/chat/userChats`);
  }

  loadChatMessages(chatId: number): Observable<ApiResponse<ChatMessage[]>> {
    return this.http.get<ApiResponse<ChatMessage[]>>(`${this.apiUrl}/chat/${chatId}/messages`)
      .pipe(
        tap(({ data }) => this.messagesSource.next(data))
      );
  }

  joinChat(chatId: number): void {
    this.hubConnection.invoke('JoinChat', chatId);
  }

  leaveChat(chatId: number): void {
    this.hubConnection.invoke('LeaveChat', chatId);
  }

  sendPrivateMessage(chatId: number, content: string): Observable<ApiResponse<ChatMessage>> {
    return this.http.post<ApiResponse<ChatMessage>>(`${this.apiUrl}/chat/sendMessage`, {
      chatId,
      content
    });
  }
}
