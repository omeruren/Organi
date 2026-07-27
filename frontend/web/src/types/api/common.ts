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

  // Machine-readable discriminator set by some endpoints (e.g. 'email_not_confirmed' from
  // RequireConfirmedEmailFilter). 403 is also used for ownership violations, so callers that
  // need to tell them apart must branch on this rather than on the human-readable detail.
  code?: string
}

// Mirrors FluentValidation's ValidationProblemDetails shape (400 responses)
export interface ValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>
}
