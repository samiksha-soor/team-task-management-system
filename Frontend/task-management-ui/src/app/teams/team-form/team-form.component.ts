import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TeamService } from '../../core/services/team.service';
import { UserService } from '../../core/services/user.service';
import { AppUser } from '../../core/models/user.model';

@Component({
  selector: 'app-team-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './team-form.component.html',
  styleUrl: './team-form.component.css'
})
export class TeamFormComponent implements OnInit {
  managers: AppUser[] = [];
  errorMessage = '';
  loading = false;

  form;

  constructor(
    private fb: FormBuilder,
    private teamService: TeamService,
    private userService: UserService,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      managerId: [null as number | null, Validators.required]
    });
  }

  ngOnInit(): void {
    this.userService.getUsers().subscribe(users => {
      this.managers = users.filter(u => u.role === 'Manager' || u.role === 'Admin');
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.errorMessage = '';

    const raw = this.form.getRawValue();
    this.teamService.createTeam({
      name: raw.name!,
      description: raw.description || undefined,
      managerId: raw.managerId!
    }).subscribe({
      next: () => this.router.navigate(['/teams']),
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Failed to create team.';
      }
    });
  }
}
