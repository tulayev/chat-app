import { Message } from '../message';
import { User } from '../user';

export interface UserChat {
  chatId: number;
  contact: User;
  lastMessage: string | null;
  lastMessageTime: Date | null;
}

export interface ChatMessage {
  chatId: number;
  contact: User;
  messages: Message[];
}
