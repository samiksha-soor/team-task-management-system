export type TaskStatusValue = 'ToDo' | 'InProgress' | 'Done';
export type TaskPriorityValue = 'Low' | 'Medium' | 'High';

export interface TaskItem {
  id: number;
  title: string;
  description?: string;
  status: TaskStatusValue;
  priority: TaskPriorityValue;
  deadline?: string | null;
  teamId: number;
  teamName: string;
  assignedToId: number;
  assignedToName: string;
  createdById: number;
  createdByName: string;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  priority: TaskPriorityValue;
  deadline?: string | null;
  teamId: number;
  assignedToId: number;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  priority?: TaskPriorityValue;
  deadline?: string | null;
  assignedToId?: number;
}

export interface TaskFilter {
  status?: TaskStatusValue;
  priority?: TaskPriorityValue;
  deadlineFrom?: string;
  deadlineTo?: string;
}
