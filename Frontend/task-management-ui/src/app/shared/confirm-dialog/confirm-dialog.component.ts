import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmDialogService } from '../../core/services/confirm-dialog.service';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.css'
})
export class ConfirmDialogComponent {
  constructor(public dialogService: ConfirmDialogService) {}

  onCancel(): void {
    this.dialogService.resolve(false);
  }

  onConfirm(): void {
    this.dialogService.resolve(true);
  }
}
