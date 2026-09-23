import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DashboardService } from '../core/services/dashboard.service';
import { DashboardSummary } from '../core/models/dashboard.model';
import { TaskFilter } from '../core/models/task.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  summary: DashboardSummary | null = null;
  loading = true;
  filter: TaskFilter = {};

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadSummary();
  }

  loadSummary(): void {
    this.loading = true;
    this.dashboardService.getSummary(this.filter).subscribe({
      next: (data) => { this.summary = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  onFilterChange(): void {
    this.loadSummary();
  }
}
