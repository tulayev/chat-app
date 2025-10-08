import { Injectable } from '@angular/core';
import { ChatMessage } from '@app/models';
import { AuthService } from '@core/services/auth.service';
import { environment } from '@environments/environment';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private messagesSource = new BehaviorSubject<ChatMessage[]>([]);
  messages$ = this.messagesSource.asObservable();
  private readonly url = `${environment.baseUrl}/hubs/chat`;
  private readonly hubConnection: signalR.HubConnection;

  constructor(private readonly auth: AuthService) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.url, {
        accessTokenFactory: () => this.auth.user?.token ?? ''
      })
      .withAutomaticReconnect()
      .build();
  }

  async start() {
    await this.hubConnection.start();
    console.log("Connected to SignalR");

    this.hubConnection.on('ReceiveMessage', (fromUserId, message, sentAt) => {
      const current = this.messagesSource.value;
      this.messagesSource.next([...current, { fromUserId, message, sentAt }]);
    });
  }

  sendPrivateMessage(toUserId: number, message: string) {
    this.hubConnection.invoke('SendPrivateMessage', toUserId, message);
  }
}
