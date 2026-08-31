import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponse, User } from '@app/models';
import { environment } from 'environments/environment';
import { LoginForm, RegisterForm } from '@pages/auth';
import { map, Observable, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private readonly http = inject(HttpClient);

  register({username, email, password, avatar}: RegisterForm): Observable<{ user: User; token: string }> {
    const formData = new FormData();

    formData.append('username', username);
    formData.append('email', email);
    formData.append('password', password);

    if (avatar) {
      formData.append('avatar', avatar);
    }

    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/register`, formData)
      .pipe(
        switchMap(({ data: token }) => this.http.get<ApiResponse<User>>(`${this.apiUrl}/me`, {
          headers: { Authorization: `Bearer ${token}` }
        }).pipe(
          map(({ data: user }) => ({ user, token }))
        ))
      );
  }

  login({ usernameOrEmail, password }: LoginForm): Observable<{ user: User; token: string }> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/login`, { usernameOrEmail, password })
      .pipe(
        switchMap(({ data: token }) => this.http.get<ApiResponse<User>>(`${this.apiUrl}/me`, {
          headers: { Authorization: `Bearer ${token}` }
        }).pipe(
          map(({ data: user }) => ({ user, token }))
        ))
      );
  }

  refreshUser(): Observable<User> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/me`)
      .pipe(map(({ data }) => data));
  }
}
