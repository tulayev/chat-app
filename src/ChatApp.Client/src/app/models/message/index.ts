import { User } from '../user';

export interface ChatMessage {
  id: number;
  chatId: number;
  sender: User;
  content: string;
  sentAt: Date;
}

export interface SendMessage {
  id: number;
  senderId: number;
  content: string;
  sentAt: Date;
}
