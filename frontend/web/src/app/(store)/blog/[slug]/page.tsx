// Component Imports
import BlogPostView from '@/components/store/blog/BlogPostView'

const BlogPostPage = ({ params }: { params: { slug: string } }) => <BlogPostView slug={params.slug} />

export default BlogPostPage
