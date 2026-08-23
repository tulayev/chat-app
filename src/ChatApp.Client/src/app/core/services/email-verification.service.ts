import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponse } from '@app/models';
import { SendCodeForm, VerifyCodeForm } from '@app/pages/auth';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmailVerificationService {
  private readonly apiUrl = `${environment.apiUrl}/emailverification`;
  private readonly http = inject(HttpClient);

  sendVerificationCode({ email }: SendCodeForm): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/send`, { email });
  }

  verifyEmail({ email, code }: VerifyCodeForm): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/verify`, { email, code });
  }
}
