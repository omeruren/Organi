// Mirrors Organi.Server.Application.Common.Models.PagedResponse<T>
export interface PagedResponse<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

// Mirrors the RFC 9457 ProblemDetails shape returned by GlobalExceptionHandler
export interface ProblemDetails {
  type?: string
  title: string
  status: number
  detail: string
  traceId?: string
}

// Mirrors FluentValidation's ValidationProblemDetails shape (400 responses)
export interface ValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>
}
