'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useBlogPostBySlug, useBlogComments, useCreateBlogComment, useDeleteBlogComment } from '@/hooks/api/useBlog'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import { useStoreToast } from '@/components/store/StoreToast'

const FALLBACK_IMAGE = '/store/assets/images/blog/blog2.png'

const formatDate = (value: string | null) =>
  value ? new Date(value).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' }) : ''

const BlogPostView = ({ slug }: { slug: string }) => {
  const { data: post, isLoading, error } = useBlogPostBySlug(slug)

  if (isLoading) {
    return (
      <>
        <Breadcrumb title='Article' items={[{ label: 'Blog', href: '/blog' }]} />
        <section className='sec_space_large'>
          <div className='container'>
            <p className='text-center py-5'>Loading article…</p>
          </div>
        </section>
      </>
    )
  }

  if (error || !post) {
    const message =
      error instanceof ApiError && error.status === 404
        ? 'This article could not be found.'
        : 'Something went wrong loading this article.'

    return (
      <>
        <Breadcrumb title='Article Not Found' items={[{ label: 'Blog', href: '/blog' }]} />
        <section className='sec_space_large'>
          <div className='container text-center py-5'>
            <p style={{ color: '#6b6b6b' }}>{message}</p>
            <Link href='/blog' className='btn custom_btn rounded-pill px-4'>
              Back to Blog
            </Link>
          </div>
        </section>
      </>
    )
  }

  const image = post.featuredImageUrl || FALLBACK_IMAGE

  return (
    <>
      <Breadcrumb title={post.title} items={[{ label: 'Blog', href: '/blog' }, { label: post.title }]} />
      <section className='sec_space_large'>
        <div className='container'>
          <div className='row justify-content-center'>
            <div className='col-lg-8'>
              <article className='bg-white rounded-4 shadow-sm overflow-hidden'>
                <div style={{ maxHeight: 420, overflow: 'hidden' }}>
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img src={image} alt={post.title} className='w-100' style={{ objectFit: 'cover' }} />
                </div>
                <div className='p-4 p-md-5'>
                  <div className='d-flex flex-wrap align-items-center gap-3 mb-3' style={{ color: '#6b6b6b', fontSize: 14 }}>
                    <span>
                      <i className='far fa-calendar-alt me-1' style={{ color: '#4f7d00' }} />
                      {formatDate(post.publishedAt ?? post.createdAt)}
                    </span>
                    <span>
                      <i className='far fa-user me-1' style={{ color: '#4f7d00' }} />
                      {post.authorName}
                    </span>
                    <span>
                      <i className='far fa-eye me-1' style={{ color: '#4f7d00' }} />
                      {post.viewCount} views
                    </span>
                    <span>
                      <i className='far fa-comment me-1' style={{ color: '#4f7d00' }} />
                      {post.commentCount} comments
                    </span>
                  </div>
                  {/* The breadcrumb band above already carries this page's <h1>; this repeat of the
                      title stays visually identical but drops a level to keep the outline valid. */}
                  <h2 className='mb-4' style={{ fontWeight: 800, fontSize: 30 }}>
                    {post.title}
                  </h2>
                  <div style={{ color: '#4a4a4a', fontSize: 16, lineHeight: 1.8, whiteSpace: 'pre-wrap' }}>
                    {post.content}
                  </div>
                </div>
              </article>

              <CommentsSection postId={post.id} />
            </div>
          </div>
        </div>
      </section>
    </>
  )
}

const CommentsSection = ({ postId }: { postId: string }) => {
  const { user } = useAuth()
  const { showToast } = useStoreToast()
  const { data: comments, isLoading } = useBlogComments(postId)
  const createComment = useCreateBlogComment(postId)
  const deleteComment = useDeleteBlogComment(postId)

  const [content, setContent] = useState('')
  const [formError, setFormError] = useState<string | null>(null)

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setFormError(null)

    if (!content.trim()) {
      setFormError('Please write a comment before posting.')

      return
    }

    try {
      await createComment.mutateAsync({ content: content.trim() })
      setContent('')
      showToast('Comment posted!', 'success')
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : 'Could not post your comment. Please try again.')
    }
  }

  const onDelete = async (commentId: string) => {
    try {
      await deleteComment.mutateAsync(commentId)
      showToast('Comment deleted.', 'info')
    } catch (err) {
      showToast(err instanceof ApiError ? err.message : 'Could not delete the comment.', 'error')
    }
  }

  const list = comments ?? []
  const isAdmin = user?.roles.includes('Admin') ?? false

  return (
    <div className='bg-white rounded-4 shadow-sm p-4 p-md-5 mt-4'>
      <h3 className='mb-4' style={{ fontWeight: 800, fontSize: 22 }}>
        Comments ({list.length})
      </h3>

      {isLoading ? (
        <p style={{ color: '#6b6b6b' }}>Loading comments…</p>
      ) : list.length === 0 ? (
        <p style={{ color: '#6b6b6b' }}>No comments yet. Be the first to share your thoughts!</p>
      ) : (
        <ul className='list-unstyled mb-4'>
          {list.map(comment => (
            <li key={comment.id} className='d-flex gap-3 py-3 border-bottom'>
              <div
                className='d-flex align-items-center justify-content-center rounded-circle flex-shrink-0'
                style={{ width: 44, height: 44, background: '#7cc000', fontWeight: 700 }}
              >
                {comment.userFullName.trim().charAt(0).toUpperCase() || '?'}
              </div>
              <div className='flex-grow-1'>
                <div className='d-flex justify-content-between align-items-center'>
                  <strong>{comment.userFullName}</strong>
                  <span style={{ color: '#6b6b6b', fontSize: 12 }}>{formatDate(comment.createdAt)}</span>
                </div>
                <p className='mb-1 mt-1' style={{ color: '#4a4a4a', fontSize: 14 }}>
                  {comment.content}
                </p>
                {(comment.userId === user?.id || isAdmin) && (
                  <button
                    type='button'
                    className='btn btn-link p-0 text-danger'
                    style={{ fontSize: 13 }}
                    disabled={deleteComment.isPending}
                    onClick={() => onDelete(comment.id)}
                  >
                    <i className='far fa-trash-alt me-1' />
                    Delete
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>
      )}

      {user ? (
        <form onSubmit={onSubmit}>
          <h5 className='mb-3' style={{ fontWeight: 700 }}>
            Leave a comment
          </h5>
          {formError && <div className='alert alert-danger'>{formError}</div>}
          <textarea
            className='form-control rounded-4 p-3 mb-3'
            rows={4}
            aria-label='Write your comment'
            placeholder='Write your comment…'
            value={content}
            onChange={e => setContent(e.target.value)}
          />
          <button
            type='submit'
            className='btn custom_btn rounded-pill px-4'
            disabled={createComment.isPending}
          >
            {createComment.isPending ? 'Posting…' : 'Post Comment'}
          </button>
        </form>
      ) : (
        <div className='text-center py-3' style={{ color: '#6b6b6b' }}>
          <p className='mb-2'>Please log in to join the conversation.</p>
          <Link href={`/login?redirectTo=/blog`} className='btn custom_btn rounded-pill px-4'>
            Login
          </Link>
        </div>
      )}
    </div>
  )
}

export default BlogPostView
