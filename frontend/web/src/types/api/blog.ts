// Mirrors Organi.Server.Application.Features.Blog.DTOs.BlogPostSummaryResponse
export interface BlogPostSummaryResponse {
  id: string
  title: string
  slug: string
  excerpt: string | null
  featuredImageUrl: string | null
  isPublished: boolean
  publishedAt: string | null
  authorName: string
  commentCount: number
  createdAt: string
}

// Mirrors BlogPostResponse (full detail — list rows lack content)
export interface BlogPostResponse extends BlogPostSummaryResponse {
  content: string
  viewCount: number
  authorId: string
  updatedAt: string | null
}

// Mirrors Organi.Server.Application.Features.Blog.DTOs.BlogCommentResponse
export interface BlogCommentResponse {
  id: string
  content: string
  blogPostId: string
  userId: string
  userFullName: string
  createdAt: string
}

// Mirrors CreateBlogCommentCommand (BlogPostId travels in the URL, not the body)
export interface CreateBlogCommentRequest {
  content: string
}

// Mirrors CreateBlogPostCommand
export interface CreateBlogPostRequest {
  title: string
  content: string
  excerpt: string | null
  featuredImageUrl: string | null
  isPublished: boolean
}

// Mirrors UpdateBlogPostCommand (Id travels in the URL, not the body)
export type UpdateBlogPostRequest = CreateBlogPostRequest
