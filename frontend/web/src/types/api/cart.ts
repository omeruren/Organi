// Mirrors Organi.Server.Application.Features.Cart.DTOs.CartItemResponse
export interface CartItemResponse {
  id: string
  productId: string
  productName: string
  productSlug: string
  unitPrice: number
  quantity: number
  lineTotal: number
  primaryImageUrl: string | null
}

// Mirrors CartResponse
export interface CartResponse {
  id: string
  items: CartItemResponse[]
  subTotal: number
}
