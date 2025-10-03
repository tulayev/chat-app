import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: 'chat', loadChildren: () => import('@pages/chat').then(m => m.CHAT_ROUTES) }
];
