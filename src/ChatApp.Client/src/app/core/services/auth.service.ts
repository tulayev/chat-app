import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponse, User } from '@app/models';
import { environment } from 'environments/environment';
import { LoginForm, RegisterForm } from '@pages/auth';
import { Observable, switchMap, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private readonly http = inject(HttpClient);

  get token(): string | null {
    const token = localStorage.getItem('token');
    return token;
  }

  get user(): User | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  register({username, email, password, avatar}: RegisterForm): Observable<ApiResponse<User>> {
    const formData = new FormData();
    
    formData.append('username', username);
    formData.append('email', email);
    formData.append('password', password);
    
    if (avatar) {
      formData.append('avatar', avatar);
    }

    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/register`, formData)
      .pipe(
        tap(({ data }) => this.setToken(data)),
        switchMap(() => this.http.get<ApiResponse<User>>(`${this.apiUrl}/me`)),
        tap(({ data }) => this.setUser(data))
      );
  }

  login({ usernameOrEmail, password }: LoginForm): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/login`, { usernameOrEmail, password })
      .pipe(
        tap(({ data }) => this.setToken(data)),
        switchMap(() => this.http.get<ApiResponse<User>>(`${this.apiUrl}/me`)),
        tap(({ data }) => this.setUser(data))
      );
  }

  refreshUser(): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/me`)
      .pipe(tap(({ data }) => this.setUser(data)));
  }

  logout(): void {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  }

  private setToken(token: string): void {
    localStorage.setItem('token', token);
  }
  
  private setUser(user: User): void {
    localStorage.setItem('user', JSON.stringify(user));
  }
}
