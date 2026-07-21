'use client'

// MUI Imports
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import CircularProgress from '@mui/material/CircularProgress'
import Typography from '@mui/material/Typography'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useSalesReport, useOrdersReport, useVendorsReport } from '@/hooks/api/useReports'

// View Imports
import ReportStatCard from '@views/reports/ReportStatCard'
import OrderStatusChart from '@views/reports/OrderStatusChart'
import TopVendorsChart from '@views/reports/TopVendorsChart'
import VendorDashboard from '@views/home/VendorDashboard'

const currency = (value: number) => `$${value.toFixed(2)}`

const AdminDashboard = () => {
  const { data: sales, isLoading: salesLoading } = useSalesReport()
  const { data: orders, isLoading: ordersLoading } = useOrdersReport()
  const { data: vendors, isLoading: vendorsLoading } = useVendorsReport({ top: 5 })

  return (
    <Grid container spacing={6}>
      {salesLoading || !sales ? (
        <Grid item xs={12}>
          <div className='flex justify-center plb-6'>
            <CircularProgress />
          </div>
        </Grid>
      ) : (
        <>
          <Grid item xs={12} sm={4}>
            <ReportStatCard title='Total Revenue' value={currency(sales.totalRevenue)} icon='ri-money-dollar-circle-line' color='success' />
          </Grid>
          <Grid item xs={12} sm={4}>
            <ReportStatCard title='Total Orders' value={String(sales.totalOrders)} icon='ri-shopping-cart-line' color='primary' />
          </Grid>
          <Grid item xs={12} sm={4}>
            <ReportStatCard title='Avg Order Value' value={currency(sales.averageOrderValue)} icon='ri-line-chart-line' color='info' />
          </Grid>
        </>
      )}
      <Grid item xs={12} md={5}>
        {ordersLoading || !orders ? (
          <Card>
            <CardContent>
              <div className='flex justify-center plb-12'>
                <CircularProgress />
              </div>
            </CardContent>
          </Card>
        ) : (
          <OrderStatusChart data={orders} />
        )}
      </Grid>
      <Grid item xs={12} md={7}>
        {vendorsLoading || !vendors ? (
          <Card>
            <CardContent>
              <div className='flex justify-center plb-12'>
                <CircularProgress />
              </div>
            </CardContent>
          </Card>
        ) : (
          <TopVendorsChart data={vendors} />
        )}
      </Grid>
    </Grid>
  )
}

const DashboardHome = () => {
  const { user, isLoading } = useAuth()

  if (isLoading || !user) {
    return (
      <div className='flex justify-center plb-12'>
        <CircularProgress />
      </div>
    )
  }

  const isAdmin = user.roles.includes('Admin')

  if (isAdmin) return <AdminDashboard />

  // Vendor (non-admin). Requires a vendor profile to scope the widgets.
  if (user.vendorId) return <VendorDashboard vendorId={user.vendorId} />

  return (
    <Typography color='text.secondary'>No dashboard data available for your account.</Typography>
  )
}

export default DashboardHome
