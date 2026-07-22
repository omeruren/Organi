// Mirrors Organi.Server.Application.Features.Categories.DTOs.CategoryResponse
export interface CategoryResponse {
  id: string
  name: string
  slug: string
  description: string | null
  imageUrl: string | null
  displayOrder: number
  isActive: boolean
  parentCategoryId: string | null
  children: CategoryResponse[]
}

// Mirrors CreateCategoryCommand — isActive is NOT settable on create (server forces true)
export interface CreateCategoryRequest {
  name: string
  description: string | null
  imageUrl: string | null
  displayOrder: number
  parentCategoryId: string | null
}

// Mirrors UpdateCategoryCommand (Id travels in the URL, not the body)
export interface UpdateCategoryRequest extends CreateCategoryRequest {
  isActive: boolean
}
