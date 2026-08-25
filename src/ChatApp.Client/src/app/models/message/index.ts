import { User } from '../user';

export interface Message {
  id: number;
  content: string;
  sentAt: Date;
  sender: User;
}
