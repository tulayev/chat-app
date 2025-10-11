import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse, AuthUser } from '@app/models';
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

  register({username, email, password, avatar}: RegisterForm): Observable<ApiResponse<AuthUser>> {
    const formData = new FormData();
    
    formData.append('username', username);
    formData.append('email', email);
    formData.append('password', password);
    
    if (avatar) {
      formData.append('avatar', avatar);
    }

    return this.http.post<ApiResponse<AuthUser>>(`${this.apiUrl}/register`, formData)
      .pipe(
        tap(({ data }) => this.setSession(data))
      );
  }

  login({ usernameOrEmail, password }: LoginForm): Observable<ApiResponse<AuthUser>> {
    return this.http.post<ApiResponse<AuthUser>>(`${this.apiUrl}/login`, { usernameOrEmail, password })
      .pipe(
        tap(({ data }) => this.setSession(data))
      );
  }

  logout(): void {
    localStorage.removeItem('user');
  }

  private setSession(user: AuthUser): void {
    localStorage.setItem('user', JSON.stringify(user));
  }
}
