import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'tasks',
    canActivate: [authGuard],
    loadComponent: () => import('./tasks/task-list/task-list.component').then(m => m.TaskListComponent)
  },
  {
    path: 'tasks/new',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin', 'Manager'] },
    loadComponent: () => import('./tasks/task-form/task-form.component').then(m => m.TaskFormComponent)
  },
  {
    path: 'tasks/:id',
    canActivate: [authGuard],
    loadComponent: () => import('./tasks/task-detail/task-detail.component').then(m => m.TaskDetailComponent)
  },
  {
    path: 'teams',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin', 'Manager'] },
    loadComponent: () => import('./teams/team-list/team-list.component').then(m => m.TeamListComponent)
  },
  {
    path: 'teams/new',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] },
    loadComponent: () => import('./teams/team-form/team-form.component').then(m => m.TeamFormComponent)
  },
  {
    path: 'users',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] },
    loadComponent: () => import('./users/user-list/user-list.component').then(m => m.UserListComponent)
  },
  { path: '**', redirectTo: 'dashboard' }
];
