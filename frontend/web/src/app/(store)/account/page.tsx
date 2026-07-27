'use client'

// Next Imports
import Link from 'next/link'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useMyOrders } from '@/hooks/api/useOrders'

// Component Imports
import OrderStatusBadge from '@/components/store/ui/OrderStatusBadge'

const AccountDashboardPage = () => {
  const { user } = useAuth()
  const { data, isLoading } = useMyOrders(1, 5, !!user)

  const orders = data?.items ?? []

  return (
    <>
      <div className='bg-white rounded-4 shadow-sm p-4 mb-4'>
        <h4 style={{ fontWeight: 800 }}>Welcome back, {user?.name.split(' ')[0]} 👋</h4>
        <p className='text-muted mb-0'>Manage your orders, profile and wishlist from your account.</p>
      </div>

      <div className='bg-white rounded-4 shadow-sm p-4'>
        <div className='d-flex justify-content-between align-items-center mb-3'>
          <h5 className='mb-0' style={{ fontWeight: 800 }}>
            Recent Orders
          </h5>
          <Link href='/account/orders' className='text-primary'>
            View all
          </Link>
        </div>

        {isLoading ? (
          <p>Loading…</p>
        ) : orders.length === 0 ? (
          <p className='text-muted mb-0'>
            You haven&rsquo;t placed any orders yet.{' '}
            <Link href='/shop' className='text-primary'>
              Start shopping
            </Link>
            .
          </p>
        ) : (
          <div className='table-responsive'>
            <table className='table align-middle mb-0'>
              <thead>
                <tr>
                  <th>Order</th>
                  <th>Date</th>
                  <th>Items</th>
                  <th>Total</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {orders.map(order => (
                  <tr key={order.id}>
                    <td>
                      <Link href={`/account/orders/${order.id}`} style={{ color: 'var(--organi-text)', fontWeight: 600 }}>
                        {order.orderNumber}
                      </Link>
                    </td>
                    <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                    <td>{order.itemCount}</td>
                    <td>${order.totalAmount.toFixed(2)}</td>
                    <td>
                      <OrderStatusBadge status={order.status} />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </>
  )
}

export default AccountDashboardPage
