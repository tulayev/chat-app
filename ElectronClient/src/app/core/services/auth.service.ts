import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '@app/models';
import { environment } from '@environments/environment';
import { LoginForm, RegisterForm } from '@pages/auth';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) { }

  get isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  get user(): User | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  register({username, email, password, avatar}: RegisterForm): Observable<User> {
    const formData = new FormData();
    
    formData.append('username', username);
    formData.append('email', email);
    formData.append('password', password);
    
    if (avatar) {
      formData.append('avatar', avatar);
    }

    return this.http.post<User>(`${this.apiUrl}/register`, formData)
      .pipe(
        tap(res => this.setSession(res))
      );
  }

  login({ usernameOrEmail, password }: LoginForm): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, { usernameOrEmail, password })
      .pipe(
        tap(res => this.setSession(res))
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  private setSession(auth: User) {
    localStorage.setItem('token', auth.token);
    localStorage.setItem('user', JSON.stringify(auth));
  }
}
