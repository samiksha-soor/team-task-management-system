import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AppNotification } from '../models/notification.model';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private baseUrl = `${environment.apiUrl}/notifications`;

  constructor(private http: HttpClient) {}

  getMyNotifications(): Observable<AppNotification[]> {
    return this.http.get<AppNotification[]>(this.baseUrl);
  }

  markAsRead(id: number): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/read`, {});
  }
}
