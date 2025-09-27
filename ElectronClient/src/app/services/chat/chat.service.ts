import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  public readonly messages: { user: string, message: string }[] = [];
  private readonly connection: signalR.HubConnection;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/chat')
      .build();

    this.connection.on('ReceiveMessage', (user, message) => {
      this.messages.push({ user, message });
    });

    this.connection.start();
  }

  sendMessage(user: string, message: string) {
    return this.connection.invoke('SendMessage', user, message);
  }
}
