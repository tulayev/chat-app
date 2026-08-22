import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { tap } from 'rxjs/operators';
import * as ChatActions from './chat.actions';

export class ChatEffects {
  private readonly actions$ = inject(Actions);

  saveCurrentChat$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(ChatActions.setCurrentChat),
        tap(({ chat }) => {
          try {
            localStorage.setItem('currentChat', JSON.stringify(chat));
          } catch (err) {
            console.error('Failed to persist current chat', err);
          }
        })
      ),
    { dispatch: false }
  );

  clearCurrentChat$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(ChatActions.clearCurrentChat),
        tap(() => {
          try {
            localStorage.removeItem('currentChat');
          } catch (err) {
            console.error('Failed to clear current chat', err);
          }
        })
      ),
    { dispatch: false }
  );
}
