import { Routes } from '@angular/router';
import { authGuard } from '@core/guards';

export const routes: Routes = [
  { path: 'chat', loadChildren: () => import('@pages/chat').then(m => m.CHAT_ROUTES), canActivate: [authGuard] },
  { path: '', loadChildren: () => import('@pages/auth').then(m => m.AUTH_ROUTES) },
  { path: '**', redirectTo: 'login' }
];
