'use client'

// React Imports
import { useEffect } from 'react'

// Third-party Imports
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

// MUI Imports
import Dialog from '@mui/material/Dialog'
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import TextField from '@mui/material/TextField'
import Switch from '@mui/material/Switch'
import FormControlLabel from '@mui/material/FormControlLabel'
import Grid from '@mui/material/Grid'
import Alert from '@mui/material/Alert'
import CircularProgress from '@mui/material/CircularProgress'

// Hook Imports
import { useBlogPost } from '@/hooks/api/useBlog'

// Type Imports
import type { CreateBlogPostRequest, UpdateBlogPostRequest } from '@/types/api/blog'

// Mirrors CreateBlogPostValidator / UpdateBlogPostValidator — see
// Features/Blog/Commands/{Create,Update}BlogPost/*Validator.cs
const blogPostSchema = z.object({
  title: z.string().min(1, 'Title is required.').max(200, 'Title must not exceed 200 characters.'),
  content: z.string().min(1, 'Content is required.').max(10000, 'Content must not exceed 10000 characters.'),
  excerpt: z.string().max(500, 'Excerpt must not exceed 500 characters.'),
  featuredImageUrl: z.string().max(500, 'Featured image URL must not exceed 500 characters.'),
  isPublished: z.boolean()
})

type BlogPostFormValues = z.infer<typeof blogPostSchema>

const emptyValues: BlogPostFormValues = {
  title: '',
  content: '',
  excerpt: '',
  featuredImageUrl: '',
  isPublished: false
}

interface BlogFormDialogProps {
  open: boolean
  postId: string | null // null = create mode
  isPending: boolean
  serverError: string | null
  onSubmit: (values: CreateBlogPostRequest | UpdateBlogPostRequest) => void
  onClose: () => void
}

const BlogFormDialog = ({ open, postId, isPending, serverError, onSubmit, onClose }: BlogFormDialogProps) => {
  const isEdit = postId !== null

  // Full detail — a summary row lacks the post content, so seed the form from it.
  const { data: post, isLoading: isPostLoading } = useBlogPost(open ? postId : null)

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<BlogPostFormValues>({
    resolver: zodResolver(blogPostSchema),
    defaultValues: emptyValues
  })

  useEffect(() => {
    if (!open) return

    if (!isEdit) {
      reset(emptyValues)

      return
    }

    if (post) {
      reset({
        title: post.title,
        content: post.content,
        excerpt: post.excerpt ?? '',
        featuredImageUrl: post.featuredImageUrl ?? '',
        isPublished: post.isPublished
      })
    }
  }, [open, isEdit, post, reset])

  const submitForm = (values: BlogPostFormValues) => {
    onSubmit({
      title: values.title,
      content: values.content,
      excerpt: values.excerpt || null,
      featuredImageUrl: values.featuredImageUrl || null,
      isPublished: values.isPublished
    })
  }

  const showLoader = isEdit && isPostLoading

  return (
    <Dialog open={open} onClose={onClose} maxWidth='md' fullWidth>
      <DialogTitle>{isEdit ? `Edit Post — ${post?.title ?? ''}` : 'New Post'}</DialogTitle>
      <form onSubmit={handleSubmit(submitForm)} noValidate>
        <DialogContent className='flex flex-col gap-5 pbs-2'>
          {serverError && <Alert severity='error'>{serverError}</Alert>}
          {showLoader ? (
            <div className='flex justify-center plb-12'>
              <CircularProgress />
            </div>
          ) : (
            <Grid container spacing={5}>
              <Grid item xs={12}>
                <Controller
                  name='title'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label='Title'
                      error={!!errors.title}
                      helperText={errors.title?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name='excerpt'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label='Excerpt'
                      placeholder='Short summary shown in listings…'
                      error={!!errors.excerpt}
                      helperText={errors.excerpt?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name='featuredImageUrl'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label='Featured Image URL'
                      error={!!errors.featuredImageUrl}
                      helperText={errors.featuredImageUrl?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name='content'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      multiline
                      minRows={12}
                      label='Content'
                      error={!!errors.content}
                      helperText={errors.content?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name='isPublished'
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Switch checked={field.value} onChange={field.onChange} />}
                      label='Published'
                    />
                  )}
                />
              </Grid>
            </Grid>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} color='secondary' disabled={isPending}>
            Cancel
          </Button>
          <Button type='submit' variant='contained' disabled={isPending || showLoader}>
            {isPending ? 'Saving…' : isEdit ? 'Save Changes' : 'Create Post'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  )
}

export default BlogFormDialog
