'use client'

// Next Imports
import Link from 'next/link'

// Offcanvas mini-cart shell (template `#offcanvasRight`). Cart contents arrive in B4;
// for now it shows an empty state with links to the cart and checkout pages.
const MiniCartDrawer = ({ open, onClose }: { open: boolean; onClose: () => void }) => {
  return (
    <>
      <div className={`offcanvas offcanvas-end${open ? ' show' : ''}`} tabIndex={-1} style={{ visibility: open ? 'visible' : 'hidden' }}>
        <div className='offcanvas-header align-items-center'>
          <h5 className='mb-0'>Your Cart</h5>
          <button type='button' className='btn-close text-reset text-end' onClick={onClose} aria-label='Close' />
        </div>
        <div className='offcanvas-body'>
          <p className='text-center py-4' style={{ color: '#6b6b6b' }}>
            Your cart is empty.
          </p>
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
