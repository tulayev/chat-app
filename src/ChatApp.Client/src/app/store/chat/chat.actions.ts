import { UserChat } from '@app/models';
import { createAction, props } from '@ngrx/store';

export const setCurrentChat = createAction(
  '[Chat] Set Current Chat',
  props<{ chat: UserChat }>()
);

export const clearCurrentChat = createAction('[Chat] Claer Current Chat');
