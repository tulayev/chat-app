import { Routes } from '@angular/router';
import { authGuard } from '@core/guards';

export const routes: Routes = [
  { path: 'users', loadChildren: () => import('@pages/users').then(m => m.USERS_ROUTES), canActivate: [authGuard] },
  { path: 'chat', loadChildren: () => import('@pages/chat').then(m => m.CHAT_ROUTES), canActivate: [authGuard] },
  { path: '', loadChildren: () => import('@pages/auth').then(m => m.AUTH_ROUTES) },
  { path: '**', loadChildren: () => import('@pages/not-found').then(m => m.NOT_FOUND_ROUTES) }
];
