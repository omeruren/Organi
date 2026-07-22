'use client'

// React Imports
import { useEffect, useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Grid from '@mui/material/Grid'
import { DataGrid } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel } from '@mui/x-data-grid'

// Hook Imports
import { useNewsletterSubscribers } from '@/hooks/api/useNewsletter'

// Type Imports
import type { NewsletterSubscriberResponse } from '@/types/api/newsletter'

const STATUS_OPTIONS = [
  { label: 'All Subscribers', value: '' },
  { label: 'Active', value: 'true' },
  { label: 'Unsubscribed', value: 'false' }
]

const NewsletterList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState('')

  // Debounce the search input so each keystroke doesn't hit the API.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setSearch(searchInput)
      setPaginationModel(prev => ({ ...prev, page: 0 }))
    }, 400)

    return () => clearTimeout(timeout)
  }, [searchInput])

  const { data, isLoading } = useNewsletterSubscribers({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    search: search || undefined,
    isActive: statusFilter === '' ? undefined : statusFilter === 'true'
  })

  const resetToFirstPage = () => setPaginationModel(prev => ({ ...prev, page: 0 }))

  const columns: GridColDef<NewsletterSubscriberResponse>[] = [
    {
      field: 'email',
      headerName: 'Email',
      flex: 1.5,
      minWidth: 240,
      sortable: false,
      renderCell: params => <Typography color='text.primary'>{params.row.email}</Typography>
    },
    {
      field: 'isActive',
      headerName: 'Status',
      width: 140,
      sortable: false,
      renderCell: params => (
        <Chip
          label={params.row.isActive ? 'Active' : 'Unsubscribed'}
          color={params.row.isActive ? 'success' : 'default'}
          size='small'
          variant='tonal'
        />
      )
    },
    {
      field: 'subscribedAt',
      headerName: 'Subscribed',
      width: 140,
      sortable: false,
      renderCell: params => <Typography>{new Date(params.row.subscribedAt).toLocaleDateString()}</Typography>
    },
    {
      field: 'unsubscribedAt',
      headerName: 'Unsubscribed',
      width: 140,
      sortable: false,
      renderCell: params => (
        <Typography>
          {params.row.unsubscribedAt ? new Date(params.row.unsubscribedAt).toLocaleDateString() : '—'}
        </Typography>
      )
    }
  ]

  return (
    <Card>
      <CardHeader title='Newsletter Subscribers' />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              size='small'
              label='Search'
              placeholder='Email…'
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
        paginationMode='server'
        rowCount={data?.totalCount ?? 0}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
        pageSizeOptions={[10, 25, 50]}
        disableRowSelectionOnClick
        disableColumnMenu
      />
    </Card>
  )
}

export default NewsletterList
