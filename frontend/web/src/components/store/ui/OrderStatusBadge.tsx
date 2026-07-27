// Type Imports
import type { OrderStatus } from '@/types/api/order'

const COLORS: Record<OrderStatus, string> = {
  Pending: 'var(--organi-warning)',
  Confirmed: 'var(--organi-info)',
  Processing: 'var(--organi-text-muted)',
  Shipped: 'var(--organi-accent)',
  Delivered: 'var(--organi-success)',
  Cancelled: 'var(--organi-danger)',
  Refunded: 'var(--organi-text-muted)'
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
