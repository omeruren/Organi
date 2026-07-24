// Mirrors Organi.Server.Application.Features.Reviews.DTOs.ReviewResponse (public, per-product)
export interface ReviewResponse {
  id: string
  rating: number
  title: string | null
  comment: string | null
  productId: string
  userId: string
  userFullName: string
  createdAt: string
  updatedAt: string | null
}

// Mirrors CreateReviewCommand (ProductId travels in the URL, not the body)
export interface CreateReviewRequest {
  rating: number
  title: string | null
  comment: string | null
}

// Mirrors Organi.Server.Application.Features.Reviews.DTOs.AdminReviewResponse
export interface AdminReviewResponse {
  id: string
  rating: number
  title: string | null
  comment: string | null
  productId: string
  productName: string
  userId: string
  userFullName: string
  createdAt: string
  updatedAt: string | null
}
