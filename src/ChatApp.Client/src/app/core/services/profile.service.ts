import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponse, User } from '@app/models';
import { ChangePasswordForm, UpdateProfileForm } from '@pages/settings';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly apiUrl = environment.apiUrl;
  private readonly http = inject(HttpClient);

  updateProfile({ username, avatar }: UpdateProfileForm): Observable<ApiResponse<User>> {
    const formData = new FormData();

    formData.append('username', username);

    if (avatar) {
      formData.append('avatar', avatar);
    }

    return this.http.put<ApiResponse<User>>(`${this.apiUrl}/profile`, formData);
  }

  changePassword({ currentPassword, newPassword }: ChangePasswordForm): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/auth/password`, { currentPassword, newPassword });
  }
}
