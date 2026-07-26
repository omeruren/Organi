// React Imports
import { Suspense } from 'react'

// Component Imports
import BlogView from '@/components/store/blog/BlogView'

const BlogPage = () => (
  <Suspense>
    <BlogView />
  </Suspense>
)

export default BlogPage
