'use client'

// Next Imports
import Link from 'next/link'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useCart, useRemoveCartItem } from '@/hooks/api/useCart'

const FALLBACK_IMAGE = '/store/assets/images/product/product1.png'

// Offcanvas mini-cart (template `#offcanvasRight`) wired to the live per-user cart.
const MiniCartDrawer = ({ open, onClose }: { open: boolean; onClose: () => void }) => {
  const { user } = useAuth()
  const { data: cart } = useCart(!!user && open)
  const removeItem = useRemoveCartItem()

  const items = cart?.items ?? []

  return (
    <>
      <div
        className={`offcanvas offcanvas-end${open ? ' show' : ''}`}
        tabIndex={-1}
        style={{ visibility: open ? 'visible' : 'hidden' }}
      >
        <div className='offcanvas-header align-items-center'>
          <h5 className='mb-0'>Your Cart</h5>
          <button type='button' className='btn-close text-reset text-end' onClick={onClose} aria-label='Close' />
        </div>
        <div className='offcanvas-body'>
          {!user ? (
            <div className='text-center py-4'>
              <p style={{ color: 'var(--organi-text-muted)' }}>Please log in to use your cart.</p>
              <Link href='/login' onClick={onClose} className='btn custom_btn rounded-pill px-4'>
                Login
              </Link>
            </div>
          ) : items.length === 0 ? (
            <p className='text-center py-4' style={{ color: 'var(--organi-text-muted)' }}>
              Your cart is empty.
            </p>
          ) : (
            <>
              {items.map(item => (
                <div key={item.id} className='prdc_ctg_product_content mt-1 d-flex align-items-center'>
                  <div className='prdc_ctg_product_img d-flex justify-content-center align-items-center me-3'>
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img src={item.primaryImageUrl || FALLBACK_IMAGE} alt={item.productName} />
                  </div>
                  <div className='prdc_ctg_product_text flex-grow-1'>
                    <div className='prdc_ctg_product_title my-1'>
                      <Link href={`/product/${item.productSlug}`} onClick={onClose}>
                        <h5 className='mb-0'>{item.productName}</h5>
                      </Link>
                    </div>
                    <div className='prdc_ctg_product_price product_price'>
                      <span className='sale_price pe-1'>
                        {item.quantity} × ${item.unitPrice.toFixed(2)}
                      </span>
                    </div>
                  </div>
                  <button
                    type='button'
                    className='btn btn-link text-danger p-0 ms-2'
                    aria-label='Remove'
                    onClick={() => removeItem.mutate(item.id)}
                  >
                    <i className='fas fa-times' />
                  </button>
                </div>
              ))}
              <div className='total_price mt-3'>
                <ul className='ul_li_block mb_30 clearfix'>
                  <li className='d-flex justify-content-between'>
                    <span>Subtotal:</span>
                    <span>${(cart?.subTotal ?? 0).toFixed(2)}</span>
                  </li>
                </ul>
              </div>
            </>
          )}
          <div className='sidebar_btns'>
            <ul className='btns_group ul_li_block clearfix'>
              <li>
                <Link href='/cart' onClick={onClose}>
                  View Cart
                </Link>
              </li>
              <li>
                <Link href='/checkout' onClick={onClose}>
                  Checkout
                </Link>
              </li>
            </ul>
          </div>
        </div>
      </div>
      {open && <div className='offcanvas-backdrop fade show' onClick={onClose} />}
    </>
  )
}

export default MiniCartDrawer
