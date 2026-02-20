import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, map, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AppConfigService } from '../config/app.config.service';
import { ListStateService } from '../shared/services/list-state.service';

export interface User {
  id: number;
  userId?: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  phone?: string; // Alias for phoneNumber for compatibility
  department?: string;
  title?: string; // alias for jobTitle
  jobTitle?: string;
  company?: string;
  address?: string;
  city?: string;
  state?: string;
  postCode?: string;
  notes?: string;
  status?: number;
  accountStatus?: number;
  accountStatusName?: string;
  failedAuthAttempts?: number;
  isActive: boolean;
  isHeadless?: boolean;
  emailConfirmed: boolean;
  lastLoginDate?: Date;
  roles: Role[];
}

export interface Role {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface LoginRequest {
  userId: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  expiresAt: Date;
  user: User;
}

export interface ForgotPasswordRequest {
  userId: string;
}

export interface ResetPasswordRequest {
  token: string;
  email: string;
  password: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface SetPasswordRequest {
  token: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl: string;
  private readonly tokenKey = 'npp_token';
  private readonly refreshTokenKey = 'npp_refresh_token';
  private readonly userKey = 'npp_user';

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  private manufacturerIdsSubject = new BehaviorSubject<number[]>([]);
  public manufacturerIds$ = this.manufacturerIdsSubject.asObservable();


  constructor(
    private http: HttpClient,
    private router: Router,
    private configService: AppConfigService,
    private listStateService: ListStateService
  ) {
    this.apiUrl = this.configService.apiUrl;
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = this.getToken();
    const user = this.getStoredUser();

    if (token && user && !this.isTokenExpired(token)) {
      this.currentUserSubject.next(user);
      this.isAuthenticatedSubject.next(true);
      this.manufacturerIdsSubject.next(this.parseManufacturerIdsFromToken(token));
    } else {
      this.clearAuth();
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {
          this.setAuth(response);
        }),
        catchError(error => {
          console.error('Login error:', error);
          return throwError(() => error);
        })
      );
  }

  logout(): void {
    this.http.post(`${this.apiUrl}/auth/logout`, {}).subscribe();
    this.listStateService.clearAllStates();
    this.clearAuth();
    this.router.navigate(['/login']);
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/forgot-password`, request);
  }

  resetPassword(request: ResetPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/reset-password`, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/change-password`, request);
  }

  setPassword(request: SetPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/users/set-password`, request);
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = this.getRefreshToken();
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/refresh-token`, { refreshToken })
      .pipe(
        tap(response => {
          this.setAuth(response);
        })
      );
  }

  getCurrentUser(): Observable<any> {
    return this.http.get(`${this.apiUrl}/auth/me`);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  private getStoredUser(): User | null {
    const userJson = localStorage.getItem(this.userKey);
    return userJson ? JSON.parse(userJson) : null;
  }

  private setAuth(response: LoginResponse): void {
    // Store token and refresh token
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.refreshTokenKey, response.refreshToken);
    localStorage.setItem(this.userKey, JSON.stringify(response.user));

    // Update subjects
    this.currentUserSubject.next(response.user);
    this.isAuthenticatedSubject.next(true);
    this.manufacturerIdsSubject.next(this.parseManufacturerIdsFromToken(response.token));
  }

  private clearAuth(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.userKey);

    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000; // Convert to milliseconds
      return Date.now() >= exp;
    } catch (error) {
      console.error('❌ Error checking token expiration:', error);
      return true;
    }
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return token !== null && !this.isTokenExpired(token);
  }

  hasRole(roleName: string): boolean {
    const user = this.currentUserSubject.value;
    return user?.roles?.some(role => role.name === roleName) || false;
  }

  hasAnyRole(roleNames: string[]): boolean {
    return roleNames.some(roleName => this.hasRole(roleName));
  }

  get currentUser(): User | null {
    return this.currentUserSubject.value;
  }

  // Helper methods for components
  isLoggedIn(): boolean {
    return this.isAuthenticated();
  }

  getUserInfo(): User | null {
    return this.currentUserSubject.value;
  }

  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  isAdminOrManager(): boolean {
    return this.hasAnyRole(['System Administrator', 'Contract Manager']);
  }

  isContractViewer(): boolean {
    return this.hasRole('Contract Viewer');
  }

  isManufacturer(): boolean {
    return this.hasRole('Manufacturer');
  }

  isHeadlessOnly(): boolean {
    const user = this.currentUserSubject.value;
    if (!user?.roles?.length) return false;
    return user.roles.every(r => r.name === 'Headless');
  }

  private parseManufacturerIdsFromToken(token: string | null): number[] {
    try {
      if (!token) return [];
      const payload = JSON.parse(atob(token.split('.')[1]));

      // Support both plural and singular claim names
      let claim: any = payload['manufacturer_ids'] ?? payload['manufacturer_id'];
      if (claim == null) return [];

      // Already an array
      if (Array.isArray(claim)) {
        return claim.map((x: any) => Number(x)).filter((n: any) => !isNaN(n));
      }

      // Numeric value
      if (typeof claim === 'number') {
        const n = Number(claim);
        return isNaN(n) ? [] : [n];
      }

      if (typeof claim === 'string') {
        const trimmed = claim.trim();
        // JSON array string: "[3,5]"
        if (trimmed.startsWith('[') && trimmed.endsWith(']')) {
          const parsed = JSON.parse(trimmed);
          if (Array.isArray(parsed)) {
            return parsed.map((x: any) => Number(x)).filter((n: any) => !isNaN(n));
          }
        }
        // Comma separated: "3,5"
        if (trimmed.includes(',')) {
          return trimmed.split(',').map(s => Number(s.trim())).filter(n => !isNaN(n));
        }
        // Single numeric string: "3"
        const single = Number(trimmed);
        return isNaN(single) ? [] : [single];
      }

      return [];
    } catch (e) {
      console.warn('Failed to parse manufacturer IDs from token:', e);
      return [];
    }
  }

  get manufacturerIds(): number[] {
    return this.manufacturerIdsSubject.value;
  }
}
