import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, UserRole } from '../models/user.model';

const TOKEN_KEY = 'tms_token';
const USER_KEY = 'tms_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  currentUser = signal<AuthResponse | null>(this.loadUser());

  constructor(private http: HttpClient) {}

  private loadUser(): AuthResponse | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, payload).pipe(
      tap(res => this.setSession(res))
    );
  }

  register(payload: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, payload).pipe(
      tap(res => this.setSession(res))
    );
  }

  private setSession(res: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, res.token);
    localStorage.setItem(USER_KEY, JSON.stringify(res));
    this.currentUser.set(res);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUser.set(null);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getRole(): UserRole | null {
    return this.currentUser()?.role ?? null;
  }

  hasRole(...roles: UserRole[]): boolean {
    const role = this.getRole();
    return role !== null && roles.includes(role);
  }
}
