import { createReducer, on } from '@ngrx/store';
import { initialChatState } from './chat.state';
import * as ChatActions from './chat.actions';

export const chatReducer = createReducer(
  initialChatState,

  on(ChatActions.setCurrentChat, (state, { chat }) => ({
    ...state,
    currentChat: chat
  })),

  on(ChatActions.clearCurrentChat, state => ({
    ...state,
    currentChat: null
  }))
);
