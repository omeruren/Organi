'use client'

// Next Imports
import Link from 'next/link'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useCart, useUpdateCartItem, useRemoveCartItem, useClearCart } from '@/hooks/api/useCart'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import QuantityStepper from '@/components/store/ui/QuantityStepper'
import StoreStateMessage from '@/components/store/ui/StoreStateMessage'

const FALLBACK_IMAGE = '/store/assets/images/product/product1.png'

const CartView = () => {
  const { user, isLoading: authLoading } = useAuth()
  const { data: cart, isLoading, isError, refetch } = useCart(!!user)
  const updateItem = useUpdateCartItem()
  const removeItem = useRemoveCartItem()
  const clearCart = useClearCart()

  const body = () => {
    if (authLoading) return <p className='text-center py-5'>Loading…</p>

    if (!user) {
      return (
        <div className='text-center py-5'>
          <p>Please log in to view your cart.</p>
          <Link href='/login?redirectTo=/cart' className='btn custom_btn rounded-pill px-4'>
            Login
          </Link>
        </div>
      )
    }

    if (isLoading) return <p className='text-center py-5'>Loading your cart…</p>

    if (isError) {
      return (
        <StoreStateMessage
          icon='fas fa-triangle-exclamation'
          title='Could not load your cart'
          message='Something went wrong while loading your cart. Please try again.'
          onRetry={() => refetch()}
        />
      )
    }

    if (!cart || cart.items.length === 0) {
      return (
        <div className='text-center py-5'>
          <p style={{ color: 'var(--organi-text-muted)' }}>Your cart is empty.</p>
          <Link href='/shop' className='btn custom_btn rounded-pill px-4'>
            Continue Shopping
          </Link>
        </div>
      )
    }

    return (
      <div className='row g-4'>
        <div className='col-lg-8'>
          <div className='table-responsive bg-white rounded-4 shadow-sm p-3'>
            <table className='table align-middle mb-0'>
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Price</th>
                  <th>Quantity</th>
                  <th>Total</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {cart.items.map(item => (
                  <tr key={item.id}>
                    <td>
                      <div className='d-flex align-items-center gap-3'>
                        {/* eslint-disable-next-line @next/next/no-img-element */}
                        <img
                          src={item.primaryImageUrl || FALLBACK_IMAGE}
                          alt={item.productName}
                          width={56}
                          height={56}
                          style={{ objectFit: 'contain' }}
                        />
                        <Link href={`/product/${item.productSlug}`} style={{ color: 'var(--organi-text)', fontWeight: 600 }}>
                          {item.productName}
                        </Link>
                      </div>
                    </td>
                    <td>${item.unitPrice.toFixed(2)}</td>
                    <td>
                      <QuantityStepper
                        value={item.quantity}
                        onChange={quantity => updateItem.mutate({ id: item.id, quantity })}
                      />
                    </td>
                    <td style={{ fontWeight: 700 }}>${item.lineTotal.toFixed(2)}</td>
                    <td>
                      <button
                        type='button'
                        className='btn btn-link text-danger p-0'
                        aria-label='Remove item'
                        onClick={() => removeItem.mutate(item.id)}
                      >
                        <i className='fas fa-times' />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className='d-flex justify-content-between mt-3'>
            <Link href='/shop' className='btn rounded-pill px-4 border'>
              Continue Shopping
            </Link>
            <button type='button' className='btn rounded-pill px-4 border text-danger' onClick={() => clearCart.mutate()}>
              Clear Cart
            </button>
          </div>
        </div>

        <div className='col-lg-4'>
          <div className='bg-white rounded-4 shadow-sm p-4'>
            <h5 className='mb-3' style={{ fontWeight: 800 }}>
              Cart Summary
            </h5>
            <div className='d-flex justify-content-between mb-2'>
              <span style={{ color: 'var(--organi-text-muted)' }}>Subtotal</span>
              <span style={{ fontWeight: 700 }}>${cart.subTotal.toFixed(2)}</span>
            </div>
            <p style={{ color: 'var(--organi-text-muted)', fontSize: 14 }}>Shipping, taxes and discounts are calculated at checkout.</p>
            <Link href='/checkout' className='btn custom_btn rounded-pill py-3 w-100 mt-2'>
              Proceed to Checkout <i className='fas fa-long-arrow-alt-right' />
            </Link>
          </div>
        </div>
      </div>
    )
  }

  return (
    <>
      <Breadcrumb title='Shopping Cart' items={[{ label: 'Cart' }]} />
      <section className='sec_space_large'>
        <div className='container'>{body()}</div>
      </section>
    </>
  )
}

export default CartView
