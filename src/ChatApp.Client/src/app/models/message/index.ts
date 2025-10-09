import { User } from '../user';

export interface ChatContact {
  id: number;
  username: string;
  lastMessage: string;
  lastMessageDate: Date;
}

export interface ChatHistory {
  id: number;
  content: string;
  sentAt: Date;
  from: User;
  to: User;
}

export interface SendMessage {
  id: number;
  senderId: number;
  receiverId: number | null;
  content: string;
  sentAt: Date;
}
