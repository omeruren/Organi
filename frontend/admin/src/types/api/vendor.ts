// Mirrors Organi.Server.Domain.Enums.VendorStatus (serialized as string)
export type VendorStatus = 'Pending' | 'Approved' | 'Suspended' | 'Rejected'

// Mirrors Organi.Server.Application.Features.Vendors.DTOs.VendorResponse
export interface VendorResponse {
  id: string
  storeName: string
  slug: string
  description: string | null
  logoUrl: string | null
  bannerUrl: string | null
  phoneNumber: string | null
  address: string | null
  city: string | null
  rating: number
  totalSales: number
  followerCount: number
  status: VendorStatus
  createdAt: string
  updatedAt: string | null
}
