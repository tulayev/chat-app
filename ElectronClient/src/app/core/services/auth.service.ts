import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { Observable, tap } from 'rxjs';

export interface AuthResponse {
  token: string;
  email: string;
  userName: string;
  avatarUrl?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) { }

  get isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  get user(): AuthResponse | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  register(email: string, userName: string, password: string, avatar?: File): Observable<AuthResponse> {
    const formData = new FormData();
    
    formData.append('email', email);
    formData.append('userName', userName);
    formData.append('password', password);
    
    if (avatar) {
      formData.append('avatar', avatar);
    }

    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, formData)
      .pipe(
        tap(res => this.setSession(res))
      );
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, password })
      .pipe(
        tap(res => this.setSession(res))
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  private setSession(auth: AuthResponse) {
    localStorage.setItem('token', auth.token);
    localStorage.setItem('user', JSON.stringify(auth));
  }
}
