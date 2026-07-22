'use client'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import Grid from '@mui/material/Grid'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import { DataGrid } from '@mui/x-data-grid'
import type { GridColDef } from '@mui/x-data-grid'

// Hook Imports
import { useOrders } from '@/hooks/api/useOrders'
import { useProducts } from '@/hooks/api/useProducts'

// View Imports
import ReportStatCard from '@views/reports/ReportStatCard'
import { orderStatusColorMap } from '@views/orders/orderStatus'

// Type Imports
import type { OrderSummaryResponse } from '@/types/api/order'

const VendorDashboard = ({ vendorId }: { vendorId: string }) => {
  // totalCount from a 1-row page is the cheapest way to get each metric.
  const { data: products } = useProducts({ page: 1, pageSize: 1, vendorId })
  const { data: pendingOrders } = useOrders({ page: 1, pageSize: 1, scope: 'vendor', status: 'Pending' })
  const { data: recentOrders, isLoading: recentLoading } = useOrders({ page: 1, pageSize: 5, scope: 'vendor' })

  const columns: GridColDef<OrderSummaryResponse>[] = [
    {
      field: 'orderNumber',
      headerName: 'Order',
      flex: 1,
      minWidth: 140,
      sortable: false,
      renderCell: params => <Typography color='text.primary'>{params.row.orderNumber}</Typography>
    },
    { field: 'customerName', headerName: 'Customer', flex: 1, minWidth: 150, sortable: false },
    {
      field: 'createdAt',
      headerName: 'Date',
      width: 130,
      sortable: false,
      renderCell: params => <Typography>{new Date(params.row.createdAt).toLocaleDateString()}</Typography>
    },
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
    }
  ]

  return (
    <Grid container spacing={6}>
      <Grid item xs={12} sm={4}>
        <ReportStatCard title='My Products' value={String(products?.totalCount ?? 0)} icon='ri-shopping-bag-line' color='primary' />
      </Grid>
      <Grid item xs={12} sm={4}>
        <ReportStatCard title='My Orders' value={String(recentOrders?.totalCount ?? 0)} icon='ri-shopping-cart-line' color='info' />
      </Grid>
      <Grid item xs={12} sm={4}>
        <ReportStatCard title='Pending Orders' value={String(pendingOrders?.totalCount ?? 0)} icon='ri-time-line' color='warning' />
      </Grid>
      <Grid item xs={12}>
        <Card>
          <CardHeader title='Recent Orders' />
          <DataGrid
            rows={recentOrders?.items ?? []}
            columns={columns}
            loading={recentLoading}
            autoHeight
            hideFooter
            disableRowSelectionOnClick
            disableColumnMenu
          />
        </Card>
      </Grid>
    </Grid>
  )
}

export default VendorDashboard
