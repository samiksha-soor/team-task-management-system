export interface TeamMember {
  id: number;
  fullName: string;
  email: string;
  role: string;
}

export interface Team {
  id: number;
  name: string;
  description?: string;
  managerId: number;
  managerName: string;
  members: TeamMember[];
}

export interface CreateTeamRequest {
  name: string;
  description?: string;
  managerId: number;
}
