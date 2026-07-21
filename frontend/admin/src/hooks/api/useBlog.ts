// Third-party Imports
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

// Lib Imports
import { apiFetch } from '@/libs/api-client'

// Type Imports
import type { PagedResponse } from '@/types/api/common'
import type { BlogPostResponse, BlogPostSummaryResponse, CreateBlogPostRequest, UpdateBlogPostRequest } from '@/types/api/blog'

export interface UseBlogPostsParams {
  page: number
  pageSize: number
  search?: string
  isPublished?: boolean

  // GET /api/blog-posts is public — before the access token is restored (hard reload / direct
  // URL) it succeeds as an anonymous request returning published-only, which would hide drafts.
  // The caller gates on auth readiness so the first fetch carries the admin/vendor token.
  enabled?: boolean
}

export const useBlogPosts = ({ enabled = true, ...params }: UseBlogPostsParams) =>
  useQuery({
    queryKey: ['blog-posts', params],
    enabled,
    queryFn: () => {
      const query = new URLSearchParams({ page: String(params.page), pageSize: String(params.pageSize) })

      if (params.search) query.set('search', params.search)
      if (params.isPublished !== undefined) query.set('isPublished', String(params.isPublished))

      return apiFetch<PagedResponse<BlogPostSummaryResponse>>(`/api/blog-posts?${query}`)
    }
  })

// Full detail — required before editing, since list rows lack the post content.
export const useBlogPost = (id: string | null) =>
  useQuery({
    queryKey: ['blog-posts', 'detail', id],
    queryFn: () => apiFetch<BlogPostResponse>(`/api/blog-posts/${id}`),
    enabled: id !== null
  })

export const useCreateBlogPost = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: CreateBlogPostRequest) =>
      apiFetch<BlogPostResponse>('/api/blog-posts', { method: 'POST', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['blog-posts'] })
  })
}

export const useUpdateBlogPost = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateBlogPostRequest }) =>
      apiFetch<BlogPostResponse>(`/api/blog-posts/${id}`, { method: 'PUT', body: request }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['blog-posts'] })
  })
}

export const useDeleteBlogPost = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => apiFetch<void>(`/api/blog-posts/${id}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['blog-posts'] })
  })
}
