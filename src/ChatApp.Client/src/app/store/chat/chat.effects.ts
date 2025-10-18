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
          localStorage.setItem('currentChat', JSON.stringify(chat));
        })
      ),
    { dispatch: false }
  );

  clearCurrentChat$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(ChatActions.clearCurrentChat),
        tap(() => localStorage.removeItem('currentChat'))
      ),
    { dispatch: false }
  );
}
