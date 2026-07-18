'use client'

// React Imports
import { useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Rating from '@mui/material/Rating'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Grid from '@mui/material/Grid'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'

// Hook Imports
import { useReviews, useDeleteReview } from '@/hooks/api/useReviews'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Type Imports
import type { AdminReviewResponse } from '@/types/api/review'

const formatDate = (iso: string) => new Date(iso).toLocaleDateString()

const ReviewList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [ratingFilter, setRatingFilter] = useState('')
  const [deletingReview, setDeletingReview] = useState<AdminReviewResponse | null>(null)

  // Hooks
  const { showToast } = useToast()

  const { data, isLoading } = useReviews({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    rating: ratingFilter === '' ? undefined : Number(ratingFilter)
  })

  const deleteReview = useDeleteReview()

  const handleDelete = async () => {
    if (!deletingReview) return

    try {
      await deleteReview.mutateAsync(deletingReview.id)
      showToast('Review deleted.')
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to delete review.', 'error')
    } finally {
      setDeletingReview(null)
    }
  }

  const columns: GridColDef<AdminReviewResponse>[] = [
    { field: 'productName', headerName: 'Product', flex: 1, minWidth: 160, sortable: false },
    {
      field: 'rating',
      headerName: 'Rating',
      width: 150,
      sortable: false,
      renderCell: params => <Rating value={params.row.rating} readOnly size='small' />
    },
    {
      field: 'comment',
      headerName: 'Review',
      flex: 1.5,
      minWidth: 220,
      sortable: false,
      renderCell: params => (
        <Tooltip title={params.row.comment ?? ''}>
          <div className='flex flex-col overflow-hidden'>
            {params.row.title && (
              <Typography variant='body2' className='font-medium' color='text.primary' noWrap>
                {params.row.title}
              </Typography>
            )}
            <Typography variant='caption' color='text.secondary' noWrap>
              {params.row.comment ?? '—'}
            </Typography>
          </div>
        </Tooltip>
      )
    },
    { field: 'userFullName', headerName: 'User', flex: 1, minWidth: 140, sortable: false },
    {
      field: 'createdAt',
      headerName: 'Date',
      width: 120,
      sortable: false,
      valueFormatter: params => formatDate(params.value)
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: '',
      width: 70,
      getActions: (params: GridRowParams<AdminReviewResponse>) => [
        <GridActionsCellItem
          key='delete'
          icon={<i className='ri-delete-bin-7-line' />}
          label='Delete'
          onClick={() => setDeletingReview(params.row)}
        />
      ]
    }
  ]

  return (
    <Card>
      <CardHeader title='Reviews' />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              size='small'
              label='Rating'
              value={ratingFilter}
              onChange={e => {
                setRatingFilter(e.target.value)
                setPaginationModel(prev => ({ ...prev, page: 0 }))
              }}
            >
              <MenuItem value=''>All Ratings</MenuItem>
              {[5, 4, 3, 2, 1].map(rating => (
                <MenuItem key={rating} value={String(rating)}>
                  {'★'.repeat(rating)}
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
      <ConfirmDialog
        open={deletingReview !== null}
        title='Delete review?'
        description={`This review of "${deletingReview?.productName}" will be deleted and the product's average rating will be recalculated.`}
        isPending={deleteReview.isPending}
        onConfirm={handleDelete}
        onCancel={() => setDeletingReview(null)}
      />
    </Card>
  )
}

export default ReviewList
