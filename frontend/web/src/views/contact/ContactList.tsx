'use client'

// React Imports
import { useEffect, useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Chip from '@mui/material/Chip'
import Button from '@mui/material/Button'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Grid from '@mui/material/Grid'
import Divider from '@mui/material/Divider'
import Dialog from '@mui/material/Dialog'
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogActions from '@mui/material/DialogActions'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'

// Component Imports
import { useToast } from '@components/ToastProvider'

// Hook Imports
import { useContactMessages, useSetContactMessageHandled } from '@/hooks/api/useContact'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Type Imports
import type { ContactMessageResponse } from '@/types/api/contact'

const STATUS_OPTIONS = [
  { label: 'All Messages', value: '' },
  { label: 'New', value: 'false' },
  { label: 'Handled', value: 'true' }
]

const ContactList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState('')
  const [selected, setSelected] = useState<ContactMessageResponse | null>(null)

  // Hooks
  const { showToast } = useToast()
  const setHandled = useSetContactMessageHandled()

  // Debounce the search input so each keystroke doesn't hit the API.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setSearch(searchInput)
      setPaginationModel(prev => ({ ...prev, page: 0 }))
    }, 400)

    return () => clearTimeout(timeout)
  }, [searchInput])

  const { data, isLoading } = useContactMessages({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    search: search || undefined,
    isHandled: statusFilter === '' ? undefined : statusFilter === 'true'
  })

  const resetToFirstPage = () => setPaginationModel(prev => ({ ...prev, page: 0 }))

  const toggleHandled = async (message: ContactMessageResponse) => {
    const next = !message.isHandled

    try {
      await setHandled.mutateAsync({ id: message.id, isHandled: next })
      showToast(next ? 'Marked as handled.' : 'Reopened.')

      // Keep the open dialog in step with what was just saved.
      setSelected(prev => (prev && prev.id === message.id ? { ...prev, isHandled: next } : prev))
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Could not update the message.', 'error')
    }
  }

  const columns: GridColDef<ContactMessageResponse>[] = [
    {
      field: 'name',
      headerName: 'From',
      flex: 1,
      minWidth: 180,
      sortable: false,
      renderCell: params => (
        <div className='flex flex-col'>
          <Typography color='text.primary'>{params.row.name}</Typography>
          <Typography variant='caption' color='text.secondary'>
            {params.row.email}
          </Typography>
        </div>
      )
    },
    {
      field: 'subject',
      headerName: 'Subject',
      flex: 1.5,
      minWidth: 220,
      sortable: false,
      renderCell: params => (
        <Typography color='text.primary' className='truncate'>
          {params.row.subject}
        </Typography>
      )
    },
    {
      field: 'isHandled',
      headerName: 'Status',
      width: 120,
      sortable: false,
      renderCell: params => (
        <Chip
          label={params.row.isHandled ? 'Handled' : 'New'}
          color={params.row.isHandled ? 'success' : 'warning'}
          size='small'
          variant='tonal'
        />
      )
    },
    {
      field: 'createdAt',
      headerName: 'Received',
      width: 140,
      sortable: false,
      renderCell: params => <Typography>{new Date(params.row.createdAt).toLocaleDateString()}</Typography>
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: '',
      width: 70,
      getActions: (params: GridRowParams<ContactMessageResponse>) => [
        <GridActionsCellItem
          key='view'
          icon={<i className='ri-mail-open-line' />}
          label='Read message'
          onClick={() => setSelected(params.row)}
        />
      ]
    }
  ]

  return (
    <Card>
      <CardHeader title='Contact Messages' />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              size='small'
              label='Search'
              placeholder='Name, email or subject…'
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
        onRowClick={params => setSelected(params.row)}
        disableRowSelectionOnClick
        disableColumnMenu
      />

      {/* The message body runs to 2000 characters, which no grid cell can show — reading it
          needs its own surface. */}
      <Dialog open={selected !== null} onClose={() => setSelected(null)} maxWidth='sm' fullWidth>
        {selected && (
          <>
            <DialogTitle>
              <div className='flex items-center justify-between gap-3'>
                <span>{selected.subject}</span>
                <Chip
                  label={selected.isHandled ? 'Handled' : 'New'}
                  color={selected.isHandled ? 'success' : 'warning'}
                  size='small'
                  variant='tonal'
                />
              </div>
            </DialogTitle>
            <DialogContent>
              <div className='flex flex-col gap-1'>
                <Typography variant='body2' color='text.secondary'>
                  From
                </Typography>
                <Typography color='text.primary'>
                  {selected.name} · {selected.email}
                </Typography>
                <Typography variant='caption' color='text.secondary'>
                  Received {new Date(selected.createdAt).toLocaleString()}
                </Typography>
              </div>
              <Divider className='mbs-4 mbe-4' />
              <Typography color='text.primary' style={{ whiteSpace: 'pre-wrap' }}>
                {selected.message}
              </Typography>
            </DialogContent>
            <DialogActions>
              <Button
                color='secondary'
                href={`mailto:${selected.email}?subject=${encodeURIComponent(`Re: ${selected.subject}`)}`}
              >
                Reply by email
              </Button>
              <Button
                variant='contained'
                color={selected.isHandled ? 'secondary' : 'primary'}
                disabled={setHandled.isPending}
                onClick={() => toggleHandled(selected)}
              >
                {selected.isHandled ? 'Reopen' : 'Mark as handled'}
              </Button>
              <Button onClick={() => setSelected(null)}>Close</Button>
            </DialogActions>
          </>
        )}
      </Dialog>
    </Card>
  )
}

export default ContactList
