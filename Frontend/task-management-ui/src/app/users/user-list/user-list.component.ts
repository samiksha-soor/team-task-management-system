import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { AppUser, UserRole } from '../../core/models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit {
  users: AppUser[] = [];
  loading = true;
  errorMessage = '';

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.userService.getUsers().subscribe({
      next: (data) => { this.users = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  onRoleChange(user: AppUser, role: UserRole): void {
    this.errorMessage = '';
    this.userService.updateRole(user.id, role).subscribe({
      next: () => this.loadUsers(),
      error: (err) => { this.errorMessage = err.error?.message || 'Failed to update role.'; }
    });
  }
}
