import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TaskComment } from '../models/comment.model';

@Injectable({ providedIn: 'root' })
export class CommentService {
  constructor(private http: HttpClient) {}

  getComments(taskId: number): Observable<TaskComment[]> {
    return this.http.get<TaskComment[]>(`${environment.apiUrl}/tasks/${taskId}/comments`);
  }

  addComment(taskId: number, content: string): Observable<TaskComment> {
    return this.http.post<TaskComment>(`${environment.apiUrl}/tasks/${taskId}/comments`, { content });
  }
}
