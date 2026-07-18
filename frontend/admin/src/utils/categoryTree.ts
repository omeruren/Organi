// Type Imports
import type { CategoryResponse } from '@/types/api/category'

export type FlattenedCategory = CategoryResponse & { depth: number }

// Depth-first flatten of the nested tree the backend returns — children arrive pre-sorted
// by displayOrder, so the resulting row order encodes the hierarchy (parents directly
// above their children). Shared by the categories screen and the product form's category select.
export const flattenCategories = (tree: CategoryResponse[], depth = 0): FlattenedCategory[] =>
  tree.flatMap(category => [{ ...category, depth }, ...flattenCategories(category.children, depth + 1)])
