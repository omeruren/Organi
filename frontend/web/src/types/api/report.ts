// Mirrors Organi.Server.Application.Features.Reports.DTOs.SalesReportResponse
export interface SalesReportResponse {
  totalRevenue: number
  totalOrders: number
  averageOrderValue: number
  fromDate: string | null
  toDate: string | null
}

// Mirrors OrdersReportResponse — order counts grouped by status
export interface OrdersReportResponse {
  totalOrders: number
  pending: number
  confirmed: number
  processing: number
  shipped: number
  delivered: number
  cancelled: number
  refunded: number
}

// Mirrors VendorReportItemResponse — one row per top vendor by revenue
export interface VendorReportItemResponse {
  vendorId: string
  storeName: string
  totalRevenue: number
  totalOrders: number
}
