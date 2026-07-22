// Mirrors Organi.Server.Application.Features.Coupons.DTOs.CouponResponse
export interface CouponResponse {
  id: string
  code: string
  description: string | null
  discountType: 'Percentage' | 'FixedAmount'
  discountValue: number
  minimumOrderAmount: number | null
  maxUsageCount: number | null
  currentUsageCount: number
  startDate: string
  endDate: string
  isActive: boolean
  createdAt: string
  updatedAt: string | null
}

// Mirrors CreateCouponCommand
export interface CreateCouponRequest {
  code: string
  description: string | null
  discountType: string
  discountValue: number
  minimumOrderAmount: number | null
  maxUsageCount: number | null
  startDate: string
  endDate: string
}

// Mirrors UpdateCouponCommand (Id travels in the URL, not the body)
export interface UpdateCouponRequest extends CreateCouponRequest {
  isActive: boolean
}
