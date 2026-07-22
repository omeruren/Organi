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
