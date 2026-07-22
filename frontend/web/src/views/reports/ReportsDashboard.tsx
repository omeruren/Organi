'use client'

// React Imports
import { useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import TextField from '@mui/material/TextField'
import Typography from '@mui/material/Typography'
import CircularProgress from '@mui/material/CircularProgress'

// Hook Imports
import { useSalesReport, useOrdersReport, useVendorsReport } from '@/hooks/api/useReports'

// View Imports
import ReportStatCard from '@views/reports/ReportStatCard'
import OrderStatusChart from '@views/reports/OrderStatusChart'
import TopVendorsChart from '@views/reports/TopVendorsChart'

const currency = (value: number) => `$${value.toFixed(2)}`

const ReportsDashboard = () => {
  const [fromDate, setFromDate] = useState('')
  const [toDate, setToDate] = useState('')

  const { data: sales, isLoading: salesLoading } = useSalesReport({
    fromDate: fromDate || undefined,
    toDate: toDate || undefined
  })

  const { data: orders, isLoading: ordersLoading } = useOrdersReport()
  const { data: vendors, isLoading: vendorsLoading } = useVendorsReport({ top: 10 })

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Card>
          <CardContent>
            <div className='flex flex-wrap items-center justify-between gap-4'>
              <Typography variant='h5'>Reports</Typography>
              <div className='flex flex-wrap items-center gap-3'>
                <TextField
                  size='small'
                  type='date'
                  label='From'
                  InputLabelProps={{ shrink: true }}
                  value={fromDate}
                  onChange={e => setFromDate(e.target.value)}
                />
                <TextField
                  size='small'
                  type='date'
                  label='To'
                  InputLabelProps={{ shrink: true }}
                  value={toDate}
                  onChange={e => setToDate(e.target.value)}
                />
              </div>
            </div>
          </CardContent>
        </Card>
      </Grid>

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

export default ReportsDashboard
