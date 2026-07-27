'use client'

// Next Imports
import Link from 'next/link'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useWishlist, useRemoveFromWishlist } from '@/hooks/api/useWishlist'
import { useCartActions } from '@/hooks/useCartActions'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import StoreStateMessage from '@/components/store/ui/StoreStateMessage'

const FALLBACK_IMAGE = '/store/assets/images/product/product1.png'

const WishlistView = () => {
  const { user, isLoading: authLoading } = useAuth()
  const { data: items, isLoading, isError, refetch } = useWishlist(!!user)
  const removeItem = useRemoveFromWishlist()
  const { addToCart } = useCartActions()

  const body = () => {
    if (authLoading) return <p className='text-center py-5'>Loading…</p>

    if (!user) {
      return (
        <div className='text-center py-5'>
          <p>Please log in to view your wishlist.</p>
          <Link href='/login?redirectTo=/wishlist' className='btn custom_btn rounded-pill px-4'>
            Login
          </Link>
        </div>
      )
    }

    if (isLoading) return <p className='text-center py-5'>Loading your wishlist…</p>

    if (isError) {
      return (
        <StoreStateMessage
          icon='fas fa-triangle-exclamation'
          title='Could not load your wishlist'
          message='Something went wrong while loading your wishlist. Please try again.'
          onRetry={() => refetch()}
        />
      )
    }

    if (!items || items.length === 0) {
      return (
        <div className='text-center py-5'>
          <p style={{ color: '#6b6b6b' }}>Your wishlist is empty.</p>
          <Link href='/shop' className='btn custom_btn rounded-pill px-4'>
            Browse Products
          </Link>
        </div>
      )
    }

    return (
      <div className='row g-4'>
        {items.map(item => {
          const hasSale = item.salePrice != null && item.salePrice < item.price

          return (
            <div key={item.id} className='col-sm-6 col-lg-3'>
              <div className='bg-white rounded-4 shadow-sm p-3 h-100 text-center position-relative'>
                <button
                  type='button'
                  className='btn btn-link text-danger position-absolute top-0 end-0 p-2'
                  aria-label='Remove'
                  onClick={() => removeItem.mutate(item.productId)}
                >
                  <i className='fas fa-times' />
                </button>
                <Link href={`/product/${item.productSlug}`}>
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img
                    src={item.imageUrl || FALLBACK_IMAGE}
                    alt={item.productName}
                    style={{ height: 140, objectFit: 'contain' }}
                  />
                </Link>
                <h6 className='mt-3'>
                  <Link href={`/product/${item.productSlug}`} style={{ color: '#292929' }}>
                    {item.productName}
                  </Link>
                </h6>
                <div className='product_price justify-content-center d-flex gap-2 mb-3'>
                  <span className='sale_price'>${(hasSale ? item.salePrice! : item.price).toFixed(2)}</span>
                  {hasSale && <del>${item.price.toFixed(2)}</del>}
                </div>
                <button
                  type='button'
                  className='btn custom_btn rounded-pill px-4 w-100'
                  onClick={() => addToCart(item.productId, 1, item.productName)}
                >
                  Add to Cart
                </button>
              </div>
            </div>
          )
        })}
      </div>
    )
  }

  return (
    <>
      <Breadcrumb title='My Wishlist' items={[{ label: 'Wishlist' }]} />
      <section className='sec_space_large'>
        <div className='container'>{body()}</div>
      </section>
    </>
  )
}

export default WishlistView
