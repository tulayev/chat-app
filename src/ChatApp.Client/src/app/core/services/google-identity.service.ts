import { inject, Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { AuthActions } from '@store/auth';
import { environment } from 'environments/environment';

declare const google: any;

@Injectable({
  providedIn: 'root'
})
export class GoogleIdentityService {
  private readonly store = inject(Store);
  private scriptLoadPromise: Promise<void> | null = null;
  private initializePromise: Promise<void> | null = null;

  initialize(): Promise<void> {
    if (!this.initializePromise) {
      this.initializePromise = this.loadScript().then(() => {
        google.accounts.id.initialize({
          client_id: environment.googleClientId,
          callback: (response: { credential: string }) => {
            this.store.dispatch(AuthActions.googleLogin({ idToken: response.credential }));
          }
        });
      });
    }

    return this.initializePromise;
  }

  async renderButton(host: HTMLElement, width: number): Promise<void> {
    await this.initialize();

    google.accounts.id.renderButton(host, {
      type: 'standard',
      theme: 'outline',
      size: 'large',
      shape: 'rectangular',
      text: 'continue_with',
      logo_alignment: 'center',
      width
    });
  }

  private loadScript(): Promise<void> {
    if (!this.scriptLoadPromise) {
      this.scriptLoadPromise = new Promise<void>((resolve, reject) => {
        if (typeof google !== 'undefined' && google?.accounts?.id) {
          resolve();
          return;
        }

        const src = 'https://accounts.google.com/gsi/client';
        const existing = document.querySelector<HTMLScriptElement>(`script[src="${src}"]`);
        const script = existing ?? document.createElement('script');

        script.addEventListener('load', () => resolve(), { once: true });
        script.addEventListener('error', () => reject(new Error('Failed to load Google Identity Services script')), { once: true });

        if (!existing) {
          script.src = src;
          script.async = true;
          script.defer = true;
          document.head.appendChild(script);
        }
      });
    }

    return this.scriptLoadPromise;
  }
}
