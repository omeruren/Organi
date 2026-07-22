'use client'

// MUI Imports
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Typography from '@mui/material/Typography'

// Component Imports
import CustomAvatar from '@core/components/mui/Avatar'

// Type Imports
import type { ThemeColor } from '@core/types'

interface ReportStatCardProps {
  title: string
  value: string
  icon: string
  color?: ThemeColor
}

const ReportStatCard = ({ title, value, icon, color = 'primary' }: ReportStatCardProps) => {
  return (
    <Card className='bs-full'>
      <CardContent className='flex items-center gap-4'>
        <CustomAvatar color={color} skin='light' variant='rounded' size={48}>
          <i className={`${icon} text-[26px]`} />
        </CustomAvatar>
        <div className='flex flex-col'>
          <Typography variant='h5'>{value}</Typography>
          <Typography variant='body2' color='text.secondary'>
            {title}
          </Typography>
        </div>
      </CardContent>
    </Card>
  )
}

export default ReportStatCard
