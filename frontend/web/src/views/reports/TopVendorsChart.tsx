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
import type { VendorReportItemResponse } from '@/types/api/report'

const AppReactApexCharts = dynamic(() => import('@/libs/styles/AppReactApexCharts'))

const TopVendorsChart = ({ data }: { data: VendorReportItemResponse[] }) => {
  const options: ApexOptions = {
    chart: { toolbar: { show: false } },
    colors: ['var(--mui-palette-primary-main)'],
    plotOptions: {
      bar: { horizontal: true, borderRadius: 6, barHeight: '60%', distributed: false }
    },
    dataLabels: { enabled: false },
    grid: {
      strokeDashArray: 7,
      borderColor: 'var(--mui-palette-divider)',
      xaxis: { lines: { show: true } },
      yaxis: { lines: { show: false } }
    },
    xaxis: {
      categories: data.map(vendor => vendor.storeName),
      labels: {
        style: { colors: 'var(--mui-palette-text-disabled)' },
        formatter: value => `$${Number(value).toFixed(0)}`
      },
      axisTicks: { show: false },
      axisBorder: { show: false }
    },
    yaxis: {
      labels: { style: { colors: 'var(--mui-palette-text-secondary)' } }
    },
    tooltip: {
      y: { formatter: value => `$${value.toFixed(2)}` }
    }
  }

  const series = [{ name: 'Revenue', data: data.map(vendor => vendor.totalRevenue) }]

  return (
    <Card className='bs-full'>
      <CardHeader title='Top Vendors by Revenue' />
      <CardContent>
        {data.length > 0 ? (
          <AppReactApexCharts type='bar' height={320} width='100%' series={series} options={options} />
        ) : (
          <div className='flex justify-center plb-12 text-textSecondary'>No vendor sales yet.</div>
        )}
      </CardContent>
    </Card>
  )
}

export default TopVendorsChart
