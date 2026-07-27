'use client'

// Next Imports
import Link from 'next/link'

// Hook Imports
import { useOrder, useCancelOrder } from '@/hooks/api/useOrders'

// Context Imports
import { useStoreToast } from '@/components/store/StoreToast'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Component Imports
import OrderStatusBadge from '@/components/store/ui/OrderStatusBadge'

const CANCELLABLE = new Set(['Pending', 'Confirmed'])

const AccountOrderDetail = ({ orderId }: { orderId: string }) => {
  const { data: order, isLoading, error } = useOrder(orderId)
  const cancelOrder = useCancelOrder()
  const { showToast } = useStoreToast()

  if (isLoading) return <p>Loading order…</p>

  if (error || !order) {
    return (
      <div className='bg-white rounded-4 shadow-sm p-4 text-center'>
        <p>Order not found.</p>
        <Link href='/account/orders' className='btn custom_btn rounded-pill px-4'>
          Back to Orders
        </Link>
      </div>
    )
  }

  const onCancel = async () => {
    try {
      await cancelOrder.mutateAsync({ id: order.id, reason: 'Cancelled by customer' })
      showToast('Order cancelled.')
    } catch (e) {
      showToast(e instanceof ApiError ? e.message : 'Could not cancel the order.', 'error')
    }
  }

  const totals: [string, string, boolean?][] = [
    ['Subtotal', `$${order.subTotal.toFixed(2)}`],
    ...(order.discountAmount > 0 ? ([['Discount', `-$${order.discountAmount.toFixed(2)}`]] as [string, string][]) : []),
    ['Shipping', `$${order.shippingCost.toFixed(2)}`],
    ['Tax', `$${order.taxAmount.toFixed(2)}`],
    ['Total', `$${order.totalAmount.toFixed(2)}`, true]
  ]

  return (
    <div className='d-flex flex-column gap-4'>
      <div className='bg-white rounded-4 shadow-sm p-4'>
        <div className='d-flex justify-content-between align-items-center flex-wrap gap-2'>
          <div>
            <h5 className='mb-1' style={{ fontWeight: 800 }}>
              {order.orderNumber}
            </h5>
            <small className='text-muted'>Placed on {new Date(order.createdAt).toLocaleString()}</small>
          </div>
          <div className='d-flex align-items-center gap-3'>
            <OrderStatusBadge status={order.status} />
            {CANCELLABLE.has(order.status) && (
              <button
                type='button'
                className='btn btn-outline-danger rounded-pill btn-sm'
                disabled={cancelOrder.isPending}
                onClick={onCancel}
              >
                Cancel Order
              </button>
            )}
          </div>
        </div>
        {order.status === 'Cancelled' && order.cancellationReason && (
          <p className='text-danger mt-2 mb-0'>Cancellation reason: {order.cancellationReason}</p>
        )}
      </div>

      <div className='row g-4'>
        <div className='col-lg-8'>
          <div className='bg-white rounded-4 shadow-sm p-4'>
            <h6 className='mb-3' style={{ fontWeight: 800 }}>
              Items
            </h6>
            <div className='table-responsive'>
              <table className='table align-middle mb-0'>
                <tbody>
                  {order.items.map(item => (
                    <tr key={item.id}>
                      <td>
                        <div style={{ fontWeight: 600 }}>{item.productName}</div>
                        <small className='text-muted'>{item.productSKU}</small>
                      </td>
                      <td className='text-end'>{item.quantity}</td>
                      <td className='text-end'>${item.unitPrice.toFixed(2)}</td>
                      <td className='text-end' style={{ fontWeight: 700 }}>
                        ${item.totalPrice.toFixed(2)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <ul className='list-unstyled mt-3 ms-auto' style={{ maxWidth: 260 }}>
              {totals.map(([label, value, bold]) => (
                <li
                  key={label}
                  className={`d-flex justify-content-between${bold ? ' fw-bold border-top pt-2 mt-2' : ''}`}
                >
                  <span>{label}</span>
                  <span>{value}</span>
                </li>
              ))}
            </ul>
          </div>
        </div>
        <div className='col-lg-4'>
          <div className='bg-white rounded-4 shadow-sm p-4'>
            <h6 className='mb-3' style={{ fontWeight: 800 }}>
              Shipping
            </h6>
            <p className='mb-1' style={{ fontWeight: 600 }}>
              {order.shippingFirstName} {order.shippingLastName}
            </p>
            <p className='mb-1 text-muted'>{order.shippingAddress}</p>
            <p className='mb-1 text-muted'>
              {order.shippingCity}
              {order.shippingPostalCode ? `, ${order.shippingPostalCode}` : ''}
            </p>
            <p className='mb-1 text-muted'>{order.shippingPhone}</p>
            <p className='mb-0 text-muted'>{order.shippingEmail}</p>
            {order.notes && (
              <>
                <hr />
                <small className='text-muted'>Notes</small>
                <p className='mb-0'>{order.notes}</p>
              </>
            )}
          </div>
        </div>
      </div>

      <div>
        <Link href='/account/orders' className='btn rounded-pill px-4 border'>
          <i className='fas fa-arrow-left me-2' /> Back to Orders
        </Link>
      </div>
    </div>
  )
}

export default AccountOrderDetail
