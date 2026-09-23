import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTaskRequest, TaskFilter, TaskItem, UpdateTaskRequest, TaskStatusValue } from '../models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private baseUrl = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getTasks(filter?: TaskFilter): Observable<TaskItem[]> {
    let params = new HttpParams();
    if (filter?.status) params = params.set('status', filter.status);
    if (filter?.priority) params = params.set('priority', filter.priority);
    if (filter?.deadlineFrom) params = params.set('deadlineFrom', filter.deadlineFrom);
    if (filter?.deadlineTo) params = params.set('deadlineTo', filter.deadlineTo);
    return this.http.get<TaskItem[]>(this.baseUrl, { params });
  }

  getTask(id: number): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.baseUrl}/${id}`);
  }

  createTask(payload: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.baseUrl, payload);
  }

  updateTask(id: number, payload: UpdateTaskRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  updateStatus(id: number, status: TaskStatusValue): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/status`, { status });
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
