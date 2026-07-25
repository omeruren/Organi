// Mirrors Organi.Server.Application.Features.Wishlist.DTOs.WishlistItemResponse
export interface WishlistItemResponse {
  id: string
  productId: string
  productName: string
  productSlug: string
  price: number
  salePrice: number | null
  imageUrl: string | null
  createdAt: string
}
