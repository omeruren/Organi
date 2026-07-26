'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import { useRouter } from 'next/navigation'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import Chip from '@mui/material/Chip'
import Button from '@mui/material/Button'
import IconButton from '@mui/material/IconButton'
import Typography from '@mui/material/Typography'
import Tooltip from '@mui/material/Tooltip'
import Divider from '@mui/material/Divider'
import Avatar from '@mui/material/Avatar'
import CircularProgress from '@mui/material/CircularProgress'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'

// Hook Imports
import { useBlogPost, useBlogComments, useDeleteBlogComment } from '@/hooks/api/useBlog'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Type Imports
import type { BlogCommentResponse } from '@/types/api/blog'

const formatDate = (value: string | null) =>
  value ? new Date(value).toLocaleString() : '—'

const BlogDetail = ({ postId }: { postId: string }) => {
  // States
  const [deletingComment, setDeletingComment] = useState<BlogCommentResponse | null>(null)

  // Hooks
  const router = useRouter()
  const { showToast } = useToast()

  const { data: post, isLoading, error } = useBlogPost(postId)
  const { data: comments, isLoading: commentsLoading, isError: commentsError, refetch } = useBlogComments(postId)
  const deleteComment = useDeleteBlogComment(postId)

  if (isLoading) {
    return (
      <div className='flex justify-center p-12'>
        <CircularProgress />
      </div>
    )
  }

  if (error || !post) {
    return (
      <Card>
        <CardContent>
          <Typography>
            {error instanceof ApiError && error.status === 404 ? 'Post not found.' : 'Failed to load post.'}
          </Typography>
        </CardContent>
      </Card>
    )
  }

  const handleDeleteComment = async () => {
    if (!deletingComment) return

    try {
      await deleteComment.mutateAsync(deletingComment.id)
      showToast('Comment deleted.')
    } catch (err) {
      showToast(err instanceof ApiError ? err.message : 'Failed to delete comment.', 'error')
    } finally {
      setDeletingComment(null)
    }
  }

  const commentList = comments ?? []

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <div className='flex flex-wrap items-center justify-between gap-4'>
          <div className='flex items-center gap-3'>
            <IconButton onClick={() => router.push('/admin/blog')} aria-label='Back to blog'>
              <i className='ri-arrow-left-line' />
            </IconButton>
            <div className='flex flex-col'>
              <div className='flex items-center gap-3'>
                <Typography variant='h5'>{post.title}</Typography>
                <Chip
                  label={post.isPublished ? 'Published' : 'Draft'}
                  color={post.isPublished ? 'success' : 'default'}
                  size='small'
                  variant='tonal'
                />
              </div>
              <Typography variant='body2' color='text.secondary'>
                By {post.authorName} · {post.isPublished ? `Published ${formatDate(post.publishedAt)}` : 'Not published'}
              </Typography>
            </div>
          </div>
          <Button
            variant='outlined'
            startIcon={<i className='ri-external-link-line' />}
            onClick={() => router.push(`/blog/${post.slug}`)}
          >
            View on site
          </Button>
        </div>
      </Grid>

      {/* Comments — the focus of this page */}
      <Grid item xs={12} md={8}>
        <Card>
          <CardHeader title={`Comments (${commentList.length})`} />
          <CardContent>
            {commentsLoading ? (
              <div className='flex justify-center p-6'>
                <CircularProgress size={28} />
              </div>
            ) : commentsError ? (
              <div className='flex flex-col items-start gap-2'>
                <Typography color='text.secondary'>Failed to load comments.</Typography>
                <Button size='small' onClick={() => refetch()}>
                  Try again
                </Button>
              </div>
            ) : commentList.length === 0 ? (
              <Typography color='text.secondary'>No comments yet.</Typography>
            ) : (
              <div className='flex flex-col'>
                {commentList.map((comment, index) => (
                  <div key={comment.id}>
                    {index > 0 && <Divider className='mbs-4 mbe-4' />}
                    <div className='flex items-start gap-3'>
                      <Avatar sx={{ bgcolor: 'primary.main', width: 40, height: 40 }}>
                        {comment.userFullName.trim().charAt(0).toUpperCase() || '?'}
                      </Avatar>
                      <div className='flex flex-col flex-grow'>
                        <div className='flex items-center justify-between gap-2'>
                          <Typography color='text.primary' className='font-medium'>
                            {comment.userFullName}
                          </Typography>
                          <div className='flex items-center gap-2'>
                            <Typography variant='caption' color='text.secondary'>
                              {formatDate(comment.createdAt)}
                            </Typography>
                            <Tooltip title='Delete comment'>
                              <IconButton
                                size='small'
                                color='error'
                                aria-label='Delete comment'
                                onClick={() => setDeletingComment(comment)}
                              >
                                <i className='ri-delete-bin-7-line text-[20px]' />
                              </IconButton>
                            </Tooltip>
                          </div>
                        </div>
                        <Typography variant='body2' color='text.primary'>
                          {comment.content}
                        </Typography>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </Grid>

      {/* Post summary */}
      <Grid item xs={12} md={4}>
        <Card>
          <CardHeader title='Post' />
          <CardContent>
            <div className='flex flex-col gap-2'>
              <div className='flex justify-between'>
                <Typography variant='body2' color='text.secondary'>
                  Author
                </Typography>
                <Typography variant='body2'>{post.authorName}</Typography>
              </div>
              <div className='flex justify-between'>
                <Typography variant='body2' color='text.secondary'>
                  Views
                </Typography>
                <Typography variant='body2'>{post.viewCount}</Typography>
              </div>
              <div className='flex justify-between'>
                <Typography variant='body2' color='text.secondary'>
                  Comments
                </Typography>
                <Typography variant='body2'>{commentList.length}</Typography>
              </div>
              <div className='flex justify-between'>
                <Typography variant='body2' color='text.secondary'>
                  Created
                </Typography>
                <Typography variant='body2'>{formatDate(post.createdAt)}</Typography>
              </div>
              {post.excerpt && (
                <>
                  <Divider className='mbs-2 mbe-2' />
                  <Typography variant='caption' color='text.secondary'>
                    Excerpt
                  </Typography>
                  <Typography variant='body2'>{post.excerpt}</Typography>
                </>
              )}
            </div>
          </CardContent>
        </Card>
      </Grid>

      <ConfirmDialog
        open={deletingComment !== null}
        title='Delete comment?'
        description={`This comment by ${deletingComment?.userFullName ?? ''} will be permanently deleted.`}
        confirmLabel='Delete'
        isPending={deleteComment.isPending}
        onConfirm={handleDeleteComment}
        onCancel={() => setDeletingComment(null)}
      />
    </Grid>
  )
}

export default BlogDetail
