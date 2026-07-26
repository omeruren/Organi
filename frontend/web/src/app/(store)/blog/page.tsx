// React Imports
import { Suspense } from 'react'

// Type Imports
import type { Metadata } from 'next'

// Component Imports
import BlogView from '@/components/store/blog/BlogView'

export const metadata: Metadata = {
  title: 'Blog',
  description: 'Tips, recipes and stories about organic food, sustainable farming and healthy living.'
}

const BlogPage = () => (
  <Suspense>
    <BlogView />
  </Suspense>
)

export default BlogPage
