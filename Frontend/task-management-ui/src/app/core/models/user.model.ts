export type UserRole = 'Admin' | 'Manager' | 'User';

export interface AuthResponse {
  token: string;
  expiresAt: string;
  userId: number;
  fullName: string;
  email: string;
  role: UserRole;
}

export interface AppUser {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
  teamId?: number | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}
