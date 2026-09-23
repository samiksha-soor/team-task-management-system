import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../core/services/dashboard.service';
import { DashboardSummary } from '../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  summary: DashboardSummary | null = null;
  loading = true;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getSummary().subscribe({
      next: (data) => { this.summary = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
