import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponse } from '@app/models';
import { ForgotPasswordForm, ResetPasswordForm } from '@pages/auth';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PasswordResetService {
  private readonly apiUrl = `${environment.apiUrl}/passwordreset`;
  private readonly http = inject(HttpClient);

  forgotPassword({ email }: ForgotPasswordForm): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/forgot`, { email });
  }

  resetPassword({ email, code, newPassword }: ResetPasswordForm): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/reset`, { email, code, newPassword });
  }
}
