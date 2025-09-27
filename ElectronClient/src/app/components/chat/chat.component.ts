import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-chat',
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.component.html'
})
export class ChatComponent {
  private connection: signalR.HubConnection;
  messages: {user: string, text: string}[] = [];
  msg = '';
  user = 'Umarbek'; // temp hardcoded user

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/chat")
      .build();

    this.connection.on("ReceiveMessage", (user, message) => {
      this.messages.push({ user, text: message });
    });

    this.connection.start();
  }

  send() {
    if (this.msg.trim()) {
      this.connection.invoke("SendMessage", this.user, this.msg);
      this.msg = '';
    }
  }
}
