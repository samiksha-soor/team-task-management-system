import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { TeamService } from '../../core/services/team.service';
import { Team } from '../../core/models/team.model';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css'
})
export class TaskFormComponent implements OnInit {
  teams: Team[] = [];
  errorMessage = '';
  loading = false;

  form;

  get selectedTeam(): Team | undefined {
    return this.teams.find(t => t.id === this.form.value.teamId);
  }

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private teamService: TeamService,
    private router: Router
  ) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      priority: ['Medium', Validators.required],
      deadline: [''],
      teamId: [null as number | null, Validators.required],
      assignedToId: [null as number | null, Validators.required]
    });
  }

  ngOnInit(): void {
    this.teamService.getTeams().subscribe(teams => this.teams = teams);
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.errorMessage = '';

    const raw = this.form.getRawValue();
    this.taskService.createTask({
      title: raw.title!,
      description: raw.description || undefined,
      priority: raw.priority as any,
      deadline: raw.deadline || null,
      teamId: raw.teamId!,
      assignedToId: raw.assignedToId!
    }).subscribe({
      next: () => this.router.navigate(['/tasks']),
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Failed to create task.';
      }
    });
  }
}
