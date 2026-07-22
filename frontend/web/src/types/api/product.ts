// Mirrors Organi.Server.Domain.Enums.ProductStatus (serialized as string)
export type ProductStatus = 'Draft' | 'Active' | 'OutOfStock' | 'Discontinued' | 'PendingApproval'

// Mirrors Organi.Server.Application.Features.Products.DTOs.ProductSummaryResponse
export interface ProductSummaryResponse {
  id: string
  name: string
  slug: string
  price: number
  salePrice: number | null
  unit: string
  isOrganic: boolean
  status: ProductStatus
  categoryId: string
  categoryName: string
  vendorId: string
  vendorName: string
  primaryImageUrl: string | null
  averageRating: number
}

// Mirrors ProductImageResponse
export interface ProductImageResponse {
  id: string
  imageUrl: string
  altText: string | null
  displayOrder: number
  isPrimary: boolean
}

// Mirrors ProductResponse (full detail — list rows lack these extra fields)
export interface ProductResponse extends ProductSummaryResponse {
  description: string | null
  shortDescription: string | null
  sku: string
  stockQuantity: number
  weight: number | null
  isFeatured: boolean
  reviewCount: number
  images: ProductImageResponse[]
  createdAt: string
  updatedAt: string | null
}

// Mirrors ProductImageRequest (nested in create/update commands — no id, server-generated)
export interface ProductImageRequest {
  imageUrl: string
  altText: string | null
  displayOrder: number
  isPrimary: boolean
}

// Mirrors CreateProductCommand — no status (products are always created as Draft)
export interface CreateProductRequest {
  name: string
  description: string | null
  shortDescription: string | null
  price: number
  salePrice: number | null
  sku: string
  stockQuantity: number
  unit: string
  weight: number | null
  isOrganic: boolean
  isFeatured: boolean
  categoryId: string

  // null = leave existing images untouched; non-null = REPLACE ALL existing images
  images: ProductImageRequest[] | null
}

// Mirrors UpdateProductCommand (Id travels in the URL, not the body)
export interface UpdateProductRequest extends CreateProductRequest {
  status: ProductStatus
}
