import { TaskItem } from './task.model';

export interface DashboardSummary {
  totalTasks: number;
  toDoCount: number;
  inProgressCount: number;
  doneCount: number;
  overdueCount: number;
  tasks: TaskItem[];
}
