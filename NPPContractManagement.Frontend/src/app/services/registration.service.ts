import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConfigService } from '../config/app.config.service';

export interface RegistrationInitiatedResponse {
  message: string;
  codeExpiresAt?: string;
}

export interface VerifyCodeResponse {
  registrationToken: string;
  tokenExpiresAt: string;
  email: string;
  firstName: string;
  lastName: string;
}

export interface CheckUserIdResponse {
  isAvailable: boolean;
  message?: string;
}

@Injectable({ providedIn: 'root' })
export class RegistrationService {
  private readonly base: string;

  constructor(private http: HttpClient, private api: AppConfigService) {
    this.base = `${this.api.apiUrl}/registration`;
  }

  initiateRegistration(email: string): Observable<RegistrationInitiatedResponse> {
    return this.http.post<RegistrationInitiatedResponse>(`${this.base}/initiate`, { email });
  }

  verifyCode(email: string, code: string): Observable<VerifyCodeResponse> {
    return this.http.post<VerifyCodeResponse>(`${this.base}/verify-code`, { email, code });
  }

  checkUserId(userId: string): Observable<CheckUserIdResponse> {
    return this.http.post<CheckUserIdResponse>(`${this.base}/check-userid`, { userId });
  }

  completeRegistration(registrationToken: string, userId: string, password: string): Observable<any> {
    return this.http.post(`${this.base}/complete`, { registrationToken, userId, password });
  }
}
