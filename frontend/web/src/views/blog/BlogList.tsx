'use client'

// React Imports
import { useEffect, useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Button from '@mui/material/Button'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Grid from '@mui/material/Grid'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'
import BlogFormDialog from '@views/blog/BlogFormDialog'

// Hook Imports
import { useBlogPosts, useCreateBlogPost, useUpdateBlogPost, useDeleteBlogPost } from '@/hooks/api/useBlog'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Type Imports
import type { BlogPostSummaryResponse, CreateBlogPostRequest, UpdateBlogPostRequest } from '@/types/api/blog'

// All / Published / Drafts — the value maps to the isPublished query param.
const STATUS_OPTIONS = [
  { label: 'All Posts', value: '' },
  { label: 'Published', value: 'true' },
  { label: 'Drafts', value: 'false' }
]

const BlogList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState('')
  const [formOpen, setFormOpen] = useState(false)
  const [editingPostId, setEditingPostId] = useState<string | null>(null)
  const [deletingPost, setDeletingPost] = useState<BlogPostSummaryResponse | null>(null)
  const [serverError, setServerError] = useState<string | null>(null)

  // Hooks
  const { showToast } = useToast()
  const { user } = useAuth()

  // Blog authoring is Admin or Vendor (CanManageProducts on the backend).
  const canCreate = (user?.roles.includes('Admin') ?? false) || (user?.roles.includes('Vendor') ?? false)

  // Debounce the search input so each keystroke doesn't hit the API.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setSearch(searchInput)
      setPaginationModel(prev => ({ ...prev, page: 0 }))
    }, 400)

    return () => clearTimeout(timeout)
  }, [searchInput])

  const { data, isLoading } = useBlogPosts({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    search: search || undefined,
    isPublished: statusFilter === '' ? undefined : statusFilter === 'true',

    // Wait for auth to resolve — the public blog endpoint would otherwise return
    // published-only on a cold load and hide drafts from admins/vendors.
    enabled: user != null
  })

  const createPost = useCreateBlogPost()
  const updatePost = useUpdateBlogPost()
  const deletePost = useDeleteBlogPost()

  const isMutating = createPost.isPending || updatePost.isPending

  const resetToFirstPage = () => setPaginationModel(prev => ({ ...prev, page: 0 }))

  const handleOpenCreate = () => {
    setEditingPostId(null)
    setServerError(null)
    setFormOpen(true)
  }

  const handleOpenEdit = (post: BlogPostSummaryResponse) => {
    setEditingPostId(post.id)
    setServerError(null)
    setFormOpen(true)
  }

  const handleSubmit = async (values: CreateBlogPostRequest | UpdateBlogPostRequest) => {
    setServerError(null)

    try {
      if (editingPostId) {
        await updatePost.mutateAsync({ id: editingPostId, request: values })
        showToast(`Post "${values.title}" updated.`)
      } else {
        await createPost.mutateAsync(values)
        showToast(`Post "${values.title}" created.`)
      }

      setFormOpen(false)
    } catch (error) {
      setServerError(error instanceof ApiError ? error.message : 'Something went wrong. Please try again.')
    }
  }

  const handleDelete = async () => {
    if (!deletingPost) return

    try {
      await deletePost.mutateAsync(deletingPost.id)
      showToast(`Post "${deletingPost.title}" deleted.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to delete post.', 'error')
    } finally {
      setDeletingPost(null)
    }
  }

  const columns: GridColDef<BlogPostSummaryResponse>[] = [
    {
      field: 'title',
      headerName: 'Title',
      flex: 1.5,
      minWidth: 240,
      sortable: false,
      renderCell: params => (
        <div className='flex flex-col'>
          <Typography color='text.primary'>{params.row.title}</Typography>
          <Typography variant='caption' color='text.secondary'>
            {params.row.slug}
          </Typography>
        </div>
      )
    },
    { field: 'authorName', headerName: 'Author', flex: 1, minWidth: 150, sortable: false },
    { field: 'commentCount', headerName: 'Comments', width: 110, sortable: false },
    {
      field: 'publishedAt',
      headerName: 'Published',
      width: 130,
      sortable: false,
      renderCell: params => (
        <Typography>{params.row.publishedAt ? new Date(params.row.publishedAt).toLocaleDateString() : '—'}</Typography>
      )
    },
    {
      field: 'isPublished',
      headerName: 'Status',
      width: 120,
      sortable: false,
      renderCell: params => (
        <Chip
          label={params.row.isPublished ? 'Published' : 'Draft'}
          color={params.row.isPublished ? 'success' : 'default'}
          size='small'
          variant='tonal'
        />
      )
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: '',
      width: 100,
      getActions: (params: GridRowParams<BlogPostSummaryResponse>) => [
        <GridActionsCellItem
          key='edit'
          icon={<i className='ri-pencil-line' />}
          label='Edit'
          onClick={() => handleOpenEdit(params.row)}
        />,
        <GridActionsCellItem
          key='delete'
          icon={<i className='ri-delete-bin-7-line' />}
          label='Delete'
          onClick={() => setDeletingPost(params.row)}
        />
      ]
    }
  ]

  return (
    <Card>
      <CardHeader
        title='Blog'
        action={
          canCreate ? (
            <Button variant='contained' startIcon={<i className='ri-add-line' />} onClick={handleOpenCreate}>
              New Post
            </Button>
          ) : undefined
        }
      />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              size='small'
              label='Search'
              placeholder='Title…'
              value={searchInput}
              onChange={e => setSearchInput(e.target.value)}
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              size='small'
              label='Status'
              value={statusFilter}
              onChange={e => {
                setStatusFilter(e.target.value)
                resetToFirstPage()
              }}
            >
              {STATUS_OPTIONS.map(option => (
                <MenuItem key={option.value} value={option.value}>
                  {option.label}
                </MenuItem>
              ))}
            </TextField>
          </Grid>
        </Grid>
      </CardContent>
      <DataGrid
        rows={data?.items ?? []}
        columns={columns}
        loading={isLoading}
        autoHeight
        rowHeight={64}
        paginationMode='server'
        rowCount={data?.totalCount ?? 0}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
        pageSizeOptions={[10, 25, 50]}
        disableRowSelectionOnClick
        disableColumnMenu
      />
      <BlogFormDialog
        open={formOpen}
        postId={editingPostId}
        isPending={isMutating}
        serverError={serverError}
        onSubmit={handleSubmit}
        onClose={() => setFormOpen(false)}
      />
      <ConfirmDialog
        open={deletingPost !== null}
        title='Delete post?'
        description={`Post "${deletingPost?.title}" will be permanently deleted.`}
        isPending={deletePost.isPending}
        onConfirm={handleDelete}
        onCancel={() => setDeletingPost(null)}
      />
    </Card>
  )
}

export default BlogList
