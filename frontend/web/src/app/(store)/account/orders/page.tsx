'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useMyOrders } from '@/hooks/api/useOrders'

// Component Imports
import OrderStatusBadge from '@/components/store/ui/OrderStatusBadge'
import StorePagination from '@/components/store/ui/StorePagination'

const AccountOrdersPage = () => {
  const { user } = useAuth()
  const [page, setPage] = useState(1)
  const { data, isLoading } = useMyOrders(page, 10, !!user)

  const orders = data?.items ?? []

  return (
    <div className='bg-white rounded-4 shadow-sm p-4'>
      <h5 className='mb-4' style={{ fontWeight: 800 }}>
        My Orders
      </h5>

      {isLoading ? (
        <p>Loading your orders…</p>
      ) : orders.length === 0 ? (
        <p className='text-muted mb-0'>
          You haven&rsquo;t placed any orders yet.{' '}
          <Link href='/shop' className='text-primary'>
            Start shopping
          </Link>
          .
        </p>
      ) : (
        <>
          <div className='table-responsive'>
            <table className='table align-middle mb-0'>
              <thead>
                <tr>
                  <th>Order</th>
                  <th>Date</th>
                  <th>Items</th>
                  <th>Total</th>
                  <th>Status</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {orders.map(order => (
                  <tr key={order.id}>
                    <td style={{ fontWeight: 600 }}>{order.orderNumber}</td>
                    <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                    <td>{order.itemCount}</td>
                    <td>${order.totalAmount.toFixed(2)}</td>
                    <td>
                      <OrderStatusBadge status={order.status} />
                    </td>
                    <td>
                      <Link href={`/account/orders/${order.id}`} className='text-primary'>
                        View
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <StorePagination page={page} totalPages={data?.totalPages ?? 0} onChange={setPage} />
        </>
      )}
    </div>
  )
}

export default AccountOrdersPage
