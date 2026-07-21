'use client'

// Next Imports
import dynamic from 'next/dynamic'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'

// Third-party Imports
import type { ApexOptions } from 'apexcharts'

// Type Imports
import type { OrdersReportResponse } from '@/types/api/report'

// Styled dynamic wrapper (matches the Materio chart pattern)
const AppReactApexCharts = dynamic(() => import('@/libs/styles/AppReactApexCharts'))

// Order status → CSS palette var, matching the A4 order-status chip semantics.
const STATUS_SLICES: { label: string; key: keyof OrdersReportResponse; color: string }[] = [
  { label: 'Pending', key: 'pending', color: 'var(--mui-palette-warning-main)' },
  { label: 'Confirmed', key: 'confirmed', color: 'var(--mui-palette-info-main)' },
  { label: 'Processing', key: 'processing', color: 'var(--mui-palette-secondary-main)' },
  { label: 'Shipped', key: 'shipped', color: 'var(--mui-palette-primary-main)' },
  { label: 'Delivered', key: 'delivered', color: 'var(--mui-palette-success-main)' },
  { label: 'Cancelled', key: 'cancelled', color: 'var(--mui-palette-error-main)' },
  { label: 'Refunded', key: 'refunded', color: 'var(--mui-palette-text-disabled)' }
]

const OrderStatusChart = ({ data }: { data: OrdersReportResponse }) => {
  const series = STATUS_SLICES.map(slice => data[slice.key])

  const options: ApexOptions = {
    labels: STATUS_SLICES.map(slice => slice.label),
    colors: STATUS_SLICES.map(slice => slice.color),
    stroke: { width: 0 },
    dataLabels: { enabled: false },
    legend: {
      position: 'bottom',
      labels: { colors: 'var(--mui-palette-text-secondary)' },
      markers: { offsetX: -3 },
      itemMargin: { horizontal: 8, vertical: 4 }
    },
    plotOptions: {
      pie: {
        donut: {
          size: '70%',
          labels: {
            show: true,
            total: {
              show: true,
              label: 'Total Orders',
              color: 'var(--mui-palette-text-secondary)',
              formatter: () => String(data.totalOrders)
            },
            value: { color: 'var(--mui-palette-text-primary)' }
          }
        }
      }
    }
  }

  return (
    <Card className='bs-full'>
      <CardHeader title='Orders by Status' />
      <CardContent>
        {data.totalOrders > 0 ? (
          <AppReactApexCharts type='donut' height={320} width='100%' series={series} options={options} />
        ) : (
          <div className='flex justify-center plb-12 text-textSecondary'>No orders yet.</div>
        )}
      </CardContent>
    </Card>
  )
}

export default OrderStatusChart
