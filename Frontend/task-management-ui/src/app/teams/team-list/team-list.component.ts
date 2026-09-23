import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TeamService } from '../../core/services/team.service';
import { UserService } from '../../core/services/user.service';
import { ConfirmDialogService } from '../../core/services/confirm-dialog.service';
import { AuthService } from '../../core/services/auth.service';
import { Team, TeamMember } from '../../core/models/team.model';
import { AppUser } from '../../core/models/user.model';

@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './team-list.component.html',
  styleUrl: './team-list.component.css'
})
export class TeamListComponent implements OnInit {
  teams: Team[] = [];
  allUsers: AppUser[] = [];
  loading = true;
  selectedNewMember: Record<number, number | null> = {};

  constructor(
    private teamService: TeamService,
    private userService: UserService,
    private confirmDialog: ConfirmDialogService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadTeams();
    this.userService.getUsers().subscribe(users => this.allUsers = users);
  }

  loadTeams(): void {
    this.loading = true;
    this.teamService.getTeams().subscribe({
      next: (data) => { this.teams = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  canManageTeam(team: Team): boolean {
    if (this.authService.hasRole('Admin')) return true;
    return this.authService.hasRole('Manager') && this.authService.currentUser()?.userId === team.managerId;
  }

  availableUsers(team: Team): AppUser[] {
    const memberIds = new Set(team.members.map(m => m.id));
    return this.allUsers.filter(u => !memberIds.has(u.id) && u.id !== team.managerId);
  }

  addMember(team: Team): void {
    const userId = this.selectedNewMember[team.id];
    if (!userId) return;

    this.teamService.addMember(team.id, userId).subscribe({
      next: () => {
        this.selectedNewMember[team.id] = null;
        this.loadTeams();
      }
    });
  }

  async removeMember(team: Team, member: TeamMember): Promise<void> {
    const confirmed = await this.confirmDialog.confirm(`Remove ${member.fullName} from ${team.name}?`, {
      title: 'Remove Member',
      confirmText: 'Remove',
      danger: true
    });
    if (!confirmed) return;

    this.teamService.removeMember(team.id, member.id).subscribe({
      next: () => this.loadTeams()
    });
  }
}
