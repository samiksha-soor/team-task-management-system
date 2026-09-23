import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AppUser } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  getUsers(): Observable<AppUser[]> {
    return this.http.get<AppUser[]>(this.baseUrl);
  }

  updateRole(userId: number, role: string): Observable<AppUser> {
    return this.http.patch<AppUser>(`${this.baseUrl}/${userId}/role`, JSON.stringify(role), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
