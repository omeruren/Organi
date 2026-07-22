// Mirrors Organi.Server.Domain.Enums.OrderStatus (serialized as string).
// Processing and Refunded exist in the enum but no endpoint transitions into them.
export type OrderStatus = 'Pending' | 'Confirmed' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled' | 'Refunded'

// Mirrors Organi.Server.Application.Features.Orders.DTOs.OrderSummaryResponse
export interface OrderSummaryResponse {
  id: string
  orderNumber: string
  customerName: string
  totalAmount: number
  status: OrderStatus
  itemCount: number
  createdAt: string
}

// Mirrors OrderItemResponse
export interface OrderItemResponse {
  id: string
  productId: string
  productName: string
  productSKU: string
  vendorId: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

// Mirrors OrderResponse (full detail)
export interface OrderResponse {
  id: string
  orderNumber: string
  subTotal: number
  discountAmount: number
  shippingCost: number
  taxAmount: number
  totalAmount: number
  status: OrderStatus
  notes: string | null
  cancellationReason: string | null
  cancelledAt: string | null
  shippingFirstName: string
  shippingLastName: string
  shippingAddress: string
  shippingCity: string
  shippingPostalCode: string | null
  shippingPhone: string
  shippingEmail: string
  userId: string
  couponId: string | null
  items: OrderItemResponse[]
  createdAt: string
}
