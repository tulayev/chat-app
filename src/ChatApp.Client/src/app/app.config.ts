import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { jwtInterceptor } from '@core/interceptors';
import { provideStore } from '@ngrx/store';
import { ChatEffects, chatReducer } from './store';
import { provideEffects } from '@ngrx/effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    // Router
    provideRouter(routes),
    // Store
    provideStore({
      chat: chatReducer
    }),
    // Effects
    provideEffects([ChatEffects]),
    // Http Client
    provideHttpClient(
      withInterceptors([jwtInterceptor])
    )
  ]
};
