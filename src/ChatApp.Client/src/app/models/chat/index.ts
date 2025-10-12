import { User } from '../user';

export interface UserChat {
  chatId: number;
  contact: User;
  lastMessage: string | null;
  lastMessageTime: Date | null;
}
