'use client'

// React Imports
import { useEffect, useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Chip from '@mui/material/Chip'
import Avatar from '@mui/material/Avatar'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Grid from '@mui/material/Grid'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'
import type { ChipProps } from '@mui/material/Chip'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'

// Hook Imports
import { useVendors, useApproveVendor, useSuspendVendor } from '@/hooks/api/useVendors'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Type Imports
import type { VendorResponse, VendorStatus } from '@/types/api/vendor'

const VENDOR_STATUSES: VendorStatus[] = ['Pending', 'Approved', 'Suspended', 'Rejected']

const statusColorMap: Record<VendorStatus, ChipProps['color']> = {
  Pending: 'warning',
  Approved: 'success',
  Suspended: 'error',
  Rejected: 'default'
}

const VendorList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState('')
  const [approvingVendor, setApprovingVendor] = useState<VendorResponse | null>(null)
  const [suspendingVendor, setSuspendingVendor] = useState<VendorResponse | null>(null)

  // Hooks
  const { showToast } = useToast()

  // Debounce the search input so each keystroke doesn't hit the API.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setSearch(searchInput)
      setPaginationModel(prev => ({ ...prev, page: 0 }))
    }, 400)

    return () => clearTimeout(timeout)
  }, [searchInput])

  const { data, isLoading } = useVendors({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    status: status || undefined,
    search: search || undefined
  })

  const approveVendor = useApproveVendor()
  const suspendVendor = useSuspendVendor()

  const resetToFirstPage = () => setPaginationModel(prev => ({ ...prev, page: 0 }))

  const handleApprove = async () => {
    if (!approvingVendor) return

    try {
      await approveVendor.mutateAsync(approvingVendor.id)
      showToast(`Vendor ${approvingVendor.storeName} approved.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to approve vendor.', 'error')
    } finally {
      setApprovingVendor(null)
    }
  }

  const handleSuspend = async () => {
    if (!suspendingVendor) return

    try {
      await suspendVendor.mutateAsync(suspendingVendor.id)
      showToast(`Vendor ${suspendingVendor.storeName} suspended.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to suspend vendor.', 'error')
    } finally {
      setSuspendingVendor(null)
    }
  }

  const columns: GridColDef<VendorResponse>[] = [
    {
      field: 'storeName',
      headerName: 'Store',
      flex: 1.5,
      minWidth: 220,
      sortable: false,
      renderCell: params => (
        <div className='flex items-center gap-3'>
          <Avatar variant='rounded' src={params.row.logoUrl ?? undefined}>
            {params.row.storeName.charAt(0)}
          </Avatar>
          <div className='flex flex-col'>
            <Typography color='text.primary'>{params.row.storeName}</Typography>
            <Typography variant='caption' color='text.secondary'>
              {params.row.slug}
            </Typography>
          </div>
        </div>
      )
    },
    { field: 'city', headerName: 'City', flex: 1, minWidth: 120, sortable: false },
    {
      field: 'rating',
      headerName: 'Rating',
      width: 100,
      sortable: false,
      renderCell: params => (
        <div className='flex items-center gap-1'>
          <i className='ri-star-fill text-warning text-base' />
          <Typography>{params.row.rating.toFixed(1)}</Typography>
        </div>
      )
    },
    { field: 'totalSales', headerName: 'Sales', width: 90, sortable: false },
    {
      field: 'createdAt',
      headerName: 'Registered',
      width: 130,
      sortable: false,
      renderCell: params => <Typography>{new Date(params.row.createdAt).toLocaleDateString()}</Typography>
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 130,
      sortable: false,
      renderCell: params => (
        <Chip label={params.value} color={statusColorMap[params.row.status]} size='small' variant='tonal' />
      )
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: '',
      width: 100,
      getActions: (params: GridRowParams<VendorResponse>) => {
        const actions = []

        if (params.row.status !== 'Approved') {
          actions.push(
            <GridActionsCellItem
              key='approve'
              icon={<i className='ri-check-line' />}
              label='Approve'
              onClick={() => setApprovingVendor(params.row)}
            />
          )
        }

        if (params.row.status !== 'Suspended') {
          actions.push(
            <GridActionsCellItem
              key='suspend'
              icon={<i className='ri-forbid-line' />}
              label='Suspend'
              onClick={() => setSuspendingVendor(params.row)}
            />
          )
        }

        return actions
      }
    }
  ]

  return (
    <Card>
      <CardHeader title='Vendors' />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              size='small'
              label='Search'
              placeholder='Store name…'
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
              value={status}
              onChange={e => {
                setStatus(e.target.value)
                resetToFirstPage()
              }}
            >
              <MenuItem value=''>All Statuses</MenuItem>
              {VENDOR_STATUSES.map(option => (
                <MenuItem key={option} value={option}>
                  {option}
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
        open={approvingVendor !== null}
        title='Approve vendor?'
        description={`Vendor "${approvingVendor?.storeName}" will be approved and their products become visible in the store.`}
        confirmLabel='Approve'
        isPending={approveVendor.isPending}
        onConfirm={handleApprove}
        onCancel={() => setApprovingVendor(null)}
      />
      <ConfirmDialog
        open={suspendingVendor !== null}
        title='Suspend vendor?'
        description={`Vendor "${suspendingVendor?.storeName}" will be suspended and their products hidden from the store.`}
        confirmLabel='Suspend'
        isPending={suspendVendor.isPending}
        onConfirm={handleSuspend}
        onCancel={() => setSuspendingVendor(null)}
      />
    </Card>
  )
}

export default VendorList
