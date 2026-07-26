// Next Imports
import Link from 'next/link'

// Type Imports
import type { BlogPostSummaryResponse } from '@/types/api/blog'

const FALLBACK_IMAGE = '/store/assets/images/blog/blog1.png'

const formatDate = (value: string | null) =>
  value
    ? new Date(value).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
    : ''

// Storefront blog list card — featured image, meta, title, excerpt.
const BlogCard = ({ post }: { post: BlogPostSummaryResponse }) => {
  const href = `/blog/${post.slug}`
  const image = post.featuredImageUrl || FALLBACK_IMAGE

  return (
    <div className='bg-white rounded-4 shadow-sm overflow-hidden h-100 d-flex flex-column'>
      <Link href={href} className='d-block position-relative' style={{ height: 200 }}>
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img src={image} alt={post.title} className='w-100 h-100' style={{ objectFit: 'cover' }} />
      </Link>
      <div className='p-4 flex-grow-1 d-flex flex-column'>
        <div className='d-flex align-items-center gap-3 mb-2' style={{ color: '#6b6b6b', fontSize: 13 }}>
          <span>
            <i className='far fa-calendar-alt me-1' style={{ color: '#7cc000' }} />
            {formatDate(post.publishedAt ?? post.createdAt)}
          </span>
          <span>
            <i className='far fa-user me-1' style={{ color: '#7cc000' }} />
            {post.authorName}
          </span>
          <span>
            <i className='far fa-comment me-1' style={{ color: '#7cc000' }} />
            {post.commentCount}
          </span>
        </div>
        <h3 className='product_title mb-2' style={{ fontSize: 18 }}>
          <Link href={href}>{post.title}</Link>
        </h3>
        {post.excerpt && (
          <p className='mb-3' style={{ color: '#6b6b6b', fontSize: 14, display: '-webkit-box', WebkitLineClamp: 3, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
            {post.excerpt}
          </p>
        )}
        <Link href={href} className='mt-auto fw-semibold text-decoration-none' style={{ color: '#7cc000' }}>
          Read More <i className='fas fa-long-arrow-alt-right ms-1' />
        </Link>
      </div>
    </div>
  )
}

export default BlogCard
