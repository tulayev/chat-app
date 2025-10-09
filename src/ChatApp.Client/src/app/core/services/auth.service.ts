import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthUser } from '@app/models';
import { environment } from '@environments/environment';
import { LoginForm, RegisterForm } from '@pages/auth';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  constructor(private readonly http: HttpClient) { }

  get user(): AuthUser | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  register({username, email, password, avatar}: RegisterForm): Observable<AuthUser> {
    const formData = new FormData();
    
    formData.append('username', username);
    formData.append('email', email);
    formData.append('password', password);
    
    if (avatar) {
      formData.append('avatar', avatar);
    }

    return this.http.post<AuthUser>(`${this.apiUrl}/register`, formData)
      .pipe(
        tap(response => this.setSession(response))
      );
  }

  login({ usernameOrEmail, password }: LoginForm): Observable<AuthUser> {
    return this.http.post<AuthUser>(`${this.apiUrl}/login`, { usernameOrEmail, password })
      .pipe(
        tap(response => this.setSession(response))
      );
  }

  logout() {
    localStorage.removeItem('user');
  }

  private setSession(user: AuthUser) {
    localStorage.setItem('user', JSON.stringify(user));
  }
}
