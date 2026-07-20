'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import { useRouter } from 'next/navigation'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import Chip from '@mui/material/Chip'
import Button from '@mui/material/Button'
import IconButton from '@mui/material/IconButton'
import Typography from '@mui/material/Typography'
import Tooltip from '@mui/material/Tooltip'
import Divider from '@mui/material/Divider'
import CircularProgress from '@mui/material/CircularProgress'
import Table from '@mui/material/Table'
import TableBody from '@mui/material/TableBody'
import TableCell from '@mui/material/TableCell'
import TableHead from '@mui/material/TableHead'
import TableRow from '@mui/material/TableRow'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'

// Hook Imports
import { useOrder, useConfirmOrder, useShipOrder, useDeliverOrder, useCancelOrder } from '@/hooks/api/useOrders'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// View Imports
import OrderCancelDialog from '@views/orders/OrderCancelDialog'
import { orderStatusColorMap } from '@views/orders/orderStatus'

type LifecycleAction = 'confirm' | 'ship' | 'deliver'

const actionCopy: Record<LifecycleAction, { title: string; description: string; confirmLabel: string; toast: string }> = {
  confirm: {
    title: 'Confirm order?',
    description: 'The order will be confirmed and stock for its items will be decremented.',
    confirmLabel: 'Confirm Order',
    toast: 'Order confirmed.'
  },
  ship: {
    title: 'Ship order?',
    description: 'The order will be marked as shipped.',
    confirmLabel: 'Ship Order',
    toast: 'Order marked as shipped.'
  },
  deliver: {
    title: 'Deliver order?',
    description: 'The order will be marked as delivered. This completes the order.',
    confirmLabel: 'Deliver Order',
    toast: 'Order marked as delivered.'
  }
}

const OrderDetail = ({ orderId }: { orderId: string }) => {
  // States
  const [pendingAction, setPendingAction] = useState<LifecycleAction | null>(null)
  const [cancelOpen, setCancelOpen] = useState(false)

  // Hooks
  const router = useRouter()
  const { showToast } = useToast()
  const { user } = useAuth()

  const { data: order, isLoading, error } = useOrder(orderId)

  const confirmOrder = useConfirmOrder()
  const shipOrder = useShipOrder()
  const deliverOrder = useDeliverOrder()
  const cancelOrder = useCancelOrder()

  const mutations: Record<LifecycleAction, typeof confirmOrder> = {
    confirm: confirmOrder,
    ship: shipOrder,
    deliver: deliverOrder
  }

  if (isLoading) {
    return (
      <div className='flex justify-center p-12'>
        <CircularProgress />
      </div>
    )
  }

  if (error || !order) {
    return (
      <Card>
        <CardContent>
          <Typography>
            {error instanceof ApiError && error.status === 404 ? 'Order not found.' : 'Failed to load order.'}
          </Typography>
        </CardContent>
      </Card>
    )
  }

  const isAdmin = user?.roles.includes('Admin') ?? false

  // Vendors may confirm/ship only when every item in the order is theirs —
  // the backend rejects mixed orders with 403 (ConfirmOrderHandler/ShipOrderHandler).
  const ownsAllItems = order.items.every(item => item.vendorId === user?.vendorId)

  const availableAction: LifecycleAction | null =
    order.status === 'Pending' ? 'confirm' : order.status === 'Confirmed' ? 'ship' : order.status === 'Shipped' ? 'deliver' : null

  const showAction = availableAction !== null && (isAdmin || availableAction !== 'deliver')
  const actionDisabled = !isAdmin && !ownsAllItems

  // Cancel is owner-or-admin on the backend; vendors never see it.
  const canCancel = isAdmin && order.status !== 'Delivered' && order.status !== 'Cancelled'

  const handleLifecycleAction = async (action: LifecycleAction) => {
    try {
      await mutations[action].mutateAsync(order.id)
      showToast(actionCopy[action].toast)
    } catch (err) {
      showToast(err instanceof ApiError ? err.message : 'Something went wrong. Please try again.', 'error')
    } finally {
      setPendingAction(null)
    }
  }

  const handleCancel = async (reason: string | null) => {
    try {
      await cancelOrder.mutateAsync({ id: order.id, reason })
      showToast('Order cancelled.')
    } catch (err) {
      showToast(err instanceof ApiError ? err.message : 'Failed to cancel order.', 'error')
    } finally {
      setCancelOpen(false)
    }
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <div className='flex flex-wrap items-center justify-between gap-4'>
          <div className='flex items-center gap-3'>
            <IconButton onClick={() => router.push('/orders')} aria-label='Back to orders'>
              <i className='ri-arrow-left-line' />
            </IconButton>
            <div className='flex flex-col'>
              <div className='flex items-center gap-3'>
                <Typography variant='h5'>{order.orderNumber}</Typography>
                <Chip label={order.status} color={orderStatusColorMap[order.status]} size='small' variant='tonal' />
              </div>
              <Typography variant='body2' color='text.secondary'>
                Placed on {new Date(order.createdAt).toLocaleString()}
              </Typography>
            </div>
          </div>
          <div className='flex items-center gap-3'>
            {showAction && (
              <Tooltip title={actionDisabled ? "Order contains other vendors' items" : ''}>
                <span>
                  <Button
                    variant='contained'
                    disabled={actionDisabled || mutations[availableAction].isPending}
                    onClick={() => setPendingAction(availableAction)}
                  >
                    {actionCopy[availableAction].confirmLabel}
                  </Button>
                </span>
              </Tooltip>
            )}
            {canCancel && (
              <Button color='error' variant='outlined' disabled={cancelOrder.isPending} onClick={() => setCancelOpen(true)}>
                Cancel Order
              </Button>
            )}
          </div>
        </div>
      </Grid>
      <Grid item xs={12} md={8}>
        <Card>
          <CardHeader title={`Items (${order.items.length})`} />
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Product</TableCell>
                <TableCell align='right'>Qty</TableCell>
                <TableCell align='right'>Unit Price</TableCell>
                <TableCell align='right'>Total</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {order.items.map(item => {
                const isForeignItem = !isAdmin && item.vendorId !== user?.vendorId

                return (
                  <TableRow key={item.id} sx={isForeignItem ? { opacity: 0.5 } : undefined}>
                    <TableCell>
                      <div className='flex flex-col'>
                        <Typography color='text.primary'>{item.productName}</Typography>
                        <Typography variant='caption' color='text.secondary'>
                          {item.productSKU}
                          {isForeignItem && ' · Other vendor'}
                        </Typography>
                      </div>
                    </TableCell>
                    <TableCell align='right'>{item.quantity}</TableCell>
                    <TableCell align='right'>{`$${item.unitPrice.toFixed(2)}`}</TableCell>
                    <TableCell align='right'>{`$${item.totalPrice.toFixed(2)}`}</TableCell>
                  </TableRow>
                )
              })}
            </TableBody>
          </Table>
          <CardContent>
            <div className='flex flex-col items-end gap-1'>
              <Typography variant='body2'>{`Subtotal: $${order.subTotal.toFixed(2)}`}</Typography>
              {order.discountAmount > 0 && (
                <Typography variant='body2' color='success.main'>
                  {`Discount: -$${order.discountAmount.toFixed(2)}`}
                </Typography>
              )}
              <Typography variant='body2'>{`Shipping: $${order.shippingCost.toFixed(2)}`}</Typography>
              <Typography variant='body2'>{`Tax: $${order.taxAmount.toFixed(2)}`}</Typography>
              <Divider className='is-48 mbs-1 mbe-1' />
              <Typography variant='h6'>{`Total: $${order.totalAmount.toFixed(2)}`}</Typography>
            </div>
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12} md={4}>
        <Grid container spacing={6}>
          <Grid item xs={12}>
            <Card>
              <CardHeader title='Shipping' />
              <CardContent>
                <div className='flex flex-col gap-1'>
                  <Typography color='text.primary'>{`${order.shippingFirstName} ${order.shippingLastName}`}</Typography>
                  <Typography variant='body2'>{order.shippingAddress}</Typography>
                  <Typography variant='body2'>
                    {order.shippingCity}
                    {order.shippingPostalCode ? `, ${order.shippingPostalCode}` : ''}
                  </Typography>
                  <Typography variant='body2'>{order.shippingPhone}</Typography>
                  <Typography variant='body2'>{order.shippingEmail}</Typography>
                  {order.notes && (
                    <>
                      <Divider className='mbs-2 mbe-2' />
                      <Typography variant='caption' color='text.secondary'>
                        Notes
                      </Typography>
                      <Typography variant='body2'>{order.notes}</Typography>
                    </>
                  )}
                </div>
              </CardContent>
            </Card>
          </Grid>
          {order.status === 'Cancelled' && (
            <Grid item xs={12}>
              <Card>
                <CardHeader title='Cancellation' />
                <CardContent>
                  <div className='flex flex-col gap-1'>
                    <Typography variant='body2'>{order.cancellationReason ?? 'No reason provided.'}</Typography>
                    {order.cancelledAt && (
                      <Typography variant='caption' color='text.secondary'>
                        Cancelled on {new Date(order.cancelledAt).toLocaleString()}
                      </Typography>
                    )}
                  </div>
                </CardContent>
              </Card>
            </Grid>
          )}
        </Grid>
      </Grid>
      {pendingAction && (
        <ConfirmDialog
          open
          title={actionCopy[pendingAction].title}
          description={actionCopy[pendingAction].description}
          confirmLabel={actionCopy[pendingAction].confirmLabel}
          isPending={mutations[pendingAction].isPending}
          onConfirm={() => handleLifecycleAction(pendingAction)}
          onCancel={() => setPendingAction(null)}
        />
      )}
      <OrderCancelDialog
        open={cancelOpen}
        isPending={cancelOrder.isPending}
        onConfirm={handleCancel}
        onCancel={() => setCancelOpen(false)}
      />
    </Grid>
  )
}

export default OrderDetail
