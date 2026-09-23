import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { CommentService } from '../../core/services/comment.service';
import { TaskItem } from '../../core/models/task.model';
import { TaskComment } from '../../core/models/comment.model';

@Component({
  selector: 'app-task-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-detail.component.html',
  styleUrl: './task-detail.component.css'
})
export class TaskDetailComponent implements OnInit {
  task: TaskItem | null = null;
  comments: TaskComment[] = [];
  newComment = '';
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private taskService: TaskService,
    private commentService: CommentService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.taskService.getTask(id).subscribe(t => { this.task = t; this.loading = false; });
    this.loadComments(id);
  }

  loadComments(taskId: number): void {
    this.commentService.getComments(taskId).subscribe(c => this.comments = c);
  }

  addComment(): void {
    if (!this.task || !this.newComment.trim()) return;
    this.commentService.addComment(this.task.id, this.newComment).subscribe(() => {
      this.newComment = '';
      this.loadComments(this.task!.id);
    });
  }
}
