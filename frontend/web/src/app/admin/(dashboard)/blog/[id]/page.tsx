// View Imports
import BlogDetail from '@views/blog/BlogDetail'

const BlogDetailPage = ({ params }: { params: { id: string } }) => <BlogDetail postId={params.id} />

export default BlogDetailPage
