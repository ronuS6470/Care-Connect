/**
 * The backend does not register a JsonStringEnumConverter, so every enum crosses the wire
 * as its underlying integer (System.Text.Json's default). These mirror the C# enums
 * (CareConnect.DTOs.Enums) value-for-value; the *_LABELS maps below are the only place
 * that turns them into display text.
 */

/** Mirrors CareConnect.DTOs.Enums.UserRole. */
export enum UserRole {
  Admin = 1,
  Caregiver = 2,
  Client = 3,
}

/** Mirrors CareConnect.DTOs.Enums.VisitStatus. */
export enum VisitStatus {
  Scheduled = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4,
  NoShow = 5,
}

/** Mirrors CareConnect.DTOs.Enums.AssignmentStatus. */
export enum AssignmentStatus {
  Active = 1,
  Completed = 2,
  Cancelled = 3,
}

export const USER_ROLE_LABELS: Record<UserRole, string> = {
  [UserRole.Admin]: 'Admin',
  [UserRole.Caregiver]: 'Caregiver',
  [UserRole.Client]: 'Client',
}

export const VISIT_STATUS_LABELS: Record<VisitStatus, string> = {
  [VisitStatus.Scheduled]: 'Scheduled',
  [VisitStatus.InProgress]: 'In Progress',
  [VisitStatus.Completed]: 'Completed',
  [VisitStatus.Cancelled]: 'Cancelled',
  [VisitStatus.NoShow]: 'No Show',
}

export const ASSIGNMENT_STATUS_LABELS: Record<AssignmentStatus, string> = {
  [AssignmentStatus.Active]: 'Active',
  [AssignmentStatus.Completed]: 'Completed',
  [AssignmentStatus.Cancelled]: 'Cancelled',
}

/** Route-friendly, lowercase role segments used under /admin, /caregiver, /client. */
export const ROLE_HOME_PATH: Record<UserRole, string> = {
  [UserRole.Admin]: '/admin/dashboard',
  [UserRole.Caregiver]: '/caregiver/dashboard',
  [UserRole.Client]: '/client/dashboard',
}
