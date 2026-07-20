// Type Imports
import type { ChipProps } from '@mui/material/Chip'

import type { OrderStatus } from '@/types/api/order'

export const orderStatusColorMap: Record<OrderStatus, ChipProps['color']> = {
  Pending: 'warning',
  Confirmed: 'info',
  Processing: 'default',
  Shipped: 'primary',
  Delivered: 'success',
  Cancelled: 'error',
  Refunded: 'default'
}

// Statuses reachable through the current lifecycle endpoints — Processing and
// Refunded stay out of the filter dropdown but keep chip colors above.
export const ORDER_STATUS_FILTER_OPTIONS: OrderStatus[] = ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled']
