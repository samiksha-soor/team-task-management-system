import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { AuthService } from '../../core/services/auth.service';
import { ConfirmDialogService } from '../../core/services/confirm-dialog.service';
import { TaskItem, TaskFilter, TaskStatusValue } from '../../core/models/task.model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent implements OnInit {
  tasks: TaskItem[] = [];
  loading = true;

  filter: TaskFilter = {};
  searchTerm = '';

  constructor(
    private taskService: TaskService,
    public authService: AuthService,
    private confirmDialog: ConfirmDialogService
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.loading = true;
    this.taskService.getTasks(this.filter).subscribe({
      next: (data) => { this.tasks = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  onFilterChange(): void {
    this.loadTasks();
  }

  get filteredTasks(): TaskItem[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) return this.tasks;
    return this.tasks.filter(t => t.title.toLowerCase().includes(term));
  }

  updateStatus(task: TaskItem, status: TaskStatusValue): void {
    this.taskService.updateStatus(task.id, status).subscribe({
      next: () => this.loadTasks()
    });
  }

  async deleteTask(task: TaskItem): Promise<void> {
    const confirmed = await this.confirmDialog.confirm(`Delete task "${task.title}"?`, {
      title: 'Delete Task',
      confirmText: 'Delete',
      danger: true
    });
    if (!confirmed) return;

    this.taskService.deleteTask(task.id).subscribe({
      next: () => this.loadTasks()
    });
  }

  canManage(): boolean {
    return this.authService.hasRole('Admin', 'Manager');
  }
}
