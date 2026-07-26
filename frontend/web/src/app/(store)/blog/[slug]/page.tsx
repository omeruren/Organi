// Next Imports
import type { Metadata } from 'next'

// Type Imports
import type { BlogPostResponse } from '@/types/api/blog'

// Lib Imports
import { serverFetch } from '@/libs/server-fetch'

// Component Imports
import BlogPostView from '@/components/store/blog/BlogPostView'

export const generateMetadata = async ({ params }: { params: { slug: string } }): Promise<Metadata> => {
  const post = await serverFetch<BlogPostResponse>(`/api/blog-posts/slug/${params.slug}`)

  if (!post) return { title: 'Article' }

  const description = post.excerpt || post.content.slice(0, 160)

  return {
    title: post.title,
    description: description.slice(0, 160),
    openGraph: {
      type: 'article',
      title: post.title,
      description: description.slice(0, 160),
      images: post.featuredImageUrl ? [post.featuredImageUrl] : undefined
    }
  }
}

const BlogPostPage = ({ params }: { params: { slug: string } }) => <BlogPostView slug={params.slug} />

export default BlogPostPage
