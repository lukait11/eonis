import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, TokenClaims } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'eonis_token';
  private readonly api = `${environment.apiUrl}/auth`;

  private _token = signal<string | null>(localStorage.getItem(this.TOKEN_KEY));
  private _claims = computed<TokenClaims | null>(() => {
    const t = this._token();
    return t ? this.parseJwt(t) : null;
  });

  readonly isLoggedIn = computed(() => {
    const c = this._claims();
    return c !== null && c.exp * 1000 > Date.now();
  });

  readonly currentUserId = computed(() => this._claims()?.sub ?? null);
  readonly currentUserEmail = computed(() => this._claims()?.email ?? null);
  readonly currentUserRole = computed(() => this._claims()?.role ?? null);

  constructor(private http: HttpClient) {}

  login(req: LoginRequest): Observable<string> {
    return this.http
      .post(`${this.api}/login`, req, { withCredentials: true, responseType: 'text' })
      .pipe(tap(token => this.store(token)));
  }

  register(req: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${this.api}/register`, req);
  }

  refresh(): Observable<string> {
    return this.http
      .post(`${this.api}/refresh`, {}, { withCredentials: true, responseType: 'text' })
      .pipe(tap(token => this.store(token)));
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(`${this.api}/logout`, {}, { withCredentials: true })
      .pipe(tap(() => this.clear()));
  }

  getToken(): string | null {
    return this._token();
  }

  private store(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    this._token.set(token);
  }

  private clear(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this._token.set(null);
  }

  private parseJwt(token: string): TokenClaims | null {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));
    } catch {
      return null;
    }
  }
}
