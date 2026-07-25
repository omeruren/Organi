// Type Imports
import type { OrderStatus } from '@/types/api/order'

const COLORS: Record<OrderStatus, string> = {
  Pending: '#f0ad4e',
  Confirmed: '#5bc0de',
  Processing: '#6c757d',
  Shipped: '#0d6efd',
  Delivered: '#28a745',
  Cancelled: '#dc3545',
  Refunded: '#6c757d'
}

const OrderStatusBadge = ({ status }: { status: OrderStatus }) => (
  <span
    className='rounded-pill px-3 py-1 text-white'
    style={{ background: COLORS[status], fontSize: 13, fontWeight: 600, whiteSpace: 'nowrap' }}
  >
    {status}
  </span>
)

export default OrderStatusBadge
