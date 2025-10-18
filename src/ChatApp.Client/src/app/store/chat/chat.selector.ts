import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ChatState } from './chat.state';

export const selectChatState = createFeatureSelector<ChatState>('chat');

export const selectCurrentChat = createSelector(
  selectChatState,
  (state) => state.currentChat
);
