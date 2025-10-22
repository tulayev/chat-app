import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { ChatEffects, chatReducer } from '@app/store';
import { errorInterceptor, jwtInterceptor } from '@core/interceptors';
import { provideEffects } from '@ngrx/effects';
import { provideStore } from '@ngrx/store';
import { ToastrModule } from 'ngx-toastr';

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
      withInterceptors([
        jwtInterceptor,
        errorInterceptor 
      ])
    ),
    provideAnimationsAsync(),
    // Toastr
    importProvidersFrom(
      ToastrModule.forRoot({
        timeOut: 3000,
        positionClass: 'toast-bottom-right',
        preventDuplicates: true
      })
    )
  ]
};
