export interface AppNotification {
  id: number;
  type: string;
  message: string;
  taskItemId?: number;
  isRead: boolean;
  createdAt: string;
}
