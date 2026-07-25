// Mirrors Organi.Server.Application.Features.Compare.DTOs.CompareItemResponse
export interface CompareItemResponse {
  id: string
  productId: string
  productName: string
  productSlug: string
  price: number
  salePrice: number | null
  imageUrl: string | null
  createdAt: string
}
