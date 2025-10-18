import { UserChat } from '@app/models';

export interface ChatState {
  currentChat: UserChat | null;
}

export const initialChatState: ChatState = {
  currentChat: null
}
