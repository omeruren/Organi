'use client'

// React Imports
import { useEffect, useState } from 'react'

// Next Imports
import { useRouter } from 'next/navigation'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Grid from '@mui/material/Grid'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'

// Hook Imports
import { useOrders } from '@/hooks/api/useOrders'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// View Imports
import { orderStatusColorMap, ORDER_STATUS_FILTER_OPTIONS } from '@views/orders/orderStatus'

// Type Imports
import type { OrderSummaryResponse } from '@/types/api/order'

const OrderList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState('')

  // Hooks
  const router = useRouter()
  const { user } = useAuth()

  // Admins list all orders; vendor-only users get the vendor-scoped endpoint
  // (orders containing at least one of their items).
  const isVendorOnly = (user?.roles.includes('Vendor') ?? false) && !(user?.roles.includes('Admin') ?? false)

  // Debounce the search input so each keystroke doesn't hit the API.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setSearch(searchInput)
      setPaginationModel(prev => ({ ...prev, page: 0 }))
    }, 400)

    return () => clearTimeout(timeout)
  }, [searchInput])

  const { data, isLoading } = useOrders({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    status: status || undefined,
    search: search || undefined,
    scope: isVendorOnly ? 'vendor' : 'admin'
  })

  const resetToFirstPage = () => setPaginationModel(prev => ({ ...prev, page: 0 }))

  const columns: GridColDef<OrderSummaryResponse>[] = [
    {
      field: 'orderNumber',
      headerName: 'Order',
      flex: 1,
      minWidth: 140,
      sortable: false,
      renderCell: params => <Typography color='text.primary'>{params.row.orderNumber}</Typography>
    },
    { field: 'customerName', headerName: 'Customer', flex: 1, minWidth: 160, sortable: false },
    {
      field: 'createdAt',
      headerName: 'Date',
      width: 130,
      sortable: false,
      renderCell: params => <Typography>{new Date(params.row.createdAt).toLocaleDateString()}</Typography>
    },
    { field: 'itemCount', headerName: 'Items', width: 90, sortable: false },
    {
      field: 'totalAmount',
      headerName: 'Total',
      width: 120,
      sortable: false,
      renderCell: params => <Typography color='text.primary'>{`$${params.row.totalAmount.toFixed(2)}`}</Typography>
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 130,
      sortable: false,
      renderCell: params => (
        <Chip label={params.value} color={orderStatusColorMap[params.row.status]} size='small' variant='tonal' />
      )
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: '',
      width: 70,
      getActions: (params: GridRowParams<OrderSummaryResponse>) => [
        <GridActionsCellItem
          key='view'
          icon={<i className='ri-eye-line' />}
          label='View'
          onClick={() => router.push(`/admin/orders/${params.row.id}`)}
        />
      ]
    }
  ]

  return (
    <Card>
      <CardHeader title='Orders' />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              size='small'
              label='Search'
              placeholder='Order number…'
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
              {ORDER_STATUS_FILTER_OPTIONS.map(option => (
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
        paginationMode='server'
        rowCount={data?.totalCount ?? 0}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
        pageSizeOptions={[10, 25, 50]}
        onRowClick={params => router.push(`/admin/orders/${params.row.id}`)}
        disableRowSelectionOnClick
        disableColumnMenu
      />
    </Card>
  )
}

export default OrderList
