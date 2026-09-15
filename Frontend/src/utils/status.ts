import { AssignmentStatus, ASSIGNMENT_STATUS_LABELS, VisitStatus, VISIT_STATUS_LABELS } from '@/types/enums'

export type BadgeTone = 'success' | 'warning' | 'danger' | 'info' | 'neutral' | 'brand'

export function visitStatusTone(status: VisitStatus): BadgeTone {
  switch (status) {
    case VisitStatus.Scheduled:
      return 'info'
    case VisitStatus.InProgress:
      return 'brand'
    case VisitStatus.Completed:
      return 'success'
    case VisitStatus.Cancelled:
      return 'neutral'
    case VisitStatus.NoShow:
      return 'danger'
    default:
      return 'neutral'
  }
}

export function visitStatusLabel(status: VisitStatus): string {
  return VISIT_STATUS_LABELS[status] ?? 'Unknown'
}

export function assignmentStatusTone(status: AssignmentStatus): BadgeTone {
  switch (status) {
    case AssignmentStatus.Active:
      return 'success'
    case AssignmentStatus.Completed:
      return 'info'
    case AssignmentStatus.Cancelled:
      return 'neutral'
    default:
      return 'neutral'
  }
}

export function assignmentStatusLabel(status: AssignmentStatus): string {
  return ASSIGNMENT_STATUS_LABELS[status] ?? 'Unknown'
}

export function activeStatusTone(isActive: boolean): BadgeTone {
  return isActive ? 'success' : 'neutral'
}
