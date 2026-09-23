import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardSummary } from '../models/dashboard.model';
import { TaskFilter } from '../models/task.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(private http: HttpClient) {}

  getSummary(filter?: TaskFilter): Observable<DashboardSummary> {
    let params = new HttpParams();
    if (filter?.status) params = params.set('status', filter.status);
    if (filter?.priority) params = params.set('priority', filter.priority);
    if (filter?.deadlineFrom) params = params.set('deadlineFrom', filter.deadlineFrom);
    if (filter?.deadlineTo) params = params.set('deadlineTo', filter.deadlineTo);
    return this.http.get<DashboardSummary>(`${environment.apiUrl}/dashboard/summary`, { params });
  }
}
