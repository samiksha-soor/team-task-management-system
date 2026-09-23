import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTeamRequest, Team } from '../models/team.model';

@Injectable({ providedIn: 'root' })
export class TeamService {
  private baseUrl = `${environment.apiUrl}/teams`;

  constructor(private http: HttpClient) {}

  getTeams(): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl);
  }

  getTeam(id: number): Observable<Team> {
    return this.http.get<Team>(`${this.baseUrl}/${id}`);
  }

  createTeam(payload: CreateTeamRequest): Observable<{ id: number; name: string }> {
    return this.http.post<{ id: number; name: string }>(this.baseUrl, payload);
  }

  addMember(teamId: number, userId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${teamId}/members`, { userId });
  }

  removeMember(teamId: number, userId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${teamId}/members/${userId}`);
  }
}
