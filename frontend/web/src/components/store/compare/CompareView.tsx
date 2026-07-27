'use client'

// Next Imports
import Link from 'next/link'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useCompare, useRemoveFromCompare } from '@/hooks/api/useCompare'
import { useCartActions } from '@/hooks/useCartActions'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import StoreStateMessage from '@/components/store/ui/StoreStateMessage'

const FALLBACK_IMAGE = '/store/assets/images/product/product1.png'

const CompareView = () => {
  const { user, isLoading: authLoading } = useAuth()
  const { data: items, isLoading, isError, refetch } = useCompare(!!user)
  const removeItem = useRemoveFromCompare()
  const { addToCart } = useCartActions()

  const body = () => {
    if (authLoading) return <p className='text-center py-5'>Loading…</p>

    if (!user) {
      return (
        <div className='text-center py-5'>
          <p>Please log in to compare products.</p>
          <Link href='/login?redirectTo=/compare' className='btn custom_btn rounded-pill px-4'>
            Login
          </Link>
        </div>
      )
    }

    if (isLoading) return <p className='text-center py-5'>Loading…</p>

    if (isError) {
      return (
        <StoreStateMessage
          icon='fas fa-triangle-exclamation'
          title='Could not load your compare list'
          message='Something went wrong while loading your compare list. Please try again.'
          onRetry={() => refetch()}
        />
      )
    }

    if (!items || items.length === 0) {
      return (
        <div className='text-center py-5'>
          <p style={{ color: '#6b6b6b' }}>Your compare list is empty.</p>
          <Link href='/shop' className='btn custom_btn rounded-pill px-4'>
            Browse Products
          </Link>
        </div>
      )
    }

    return (
      <div className='table-responsive bg-white rounded-4 shadow-sm p-3'>
        <table className='table align-middle text-center mb-0'>
          <tbody>
            <tr>
              <th style={{ width: 140 }}>Product</th>
              {items.map(item => (
                <td key={item.id}>
                  <Link href={`/product/${item.productSlug}`}>
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img src={item.imageUrl || FALLBACK_IMAGE} alt={item.productName} style={{ height: 110, objectFit: 'contain' }} />
                  </Link>
                </td>
              ))}
            </tr>
            <tr>
              <th>Name</th>
              {items.map(item => (
                <td key={item.id}>
                  <Link href={`/product/${item.productSlug}`} style={{ color: '#292929', fontWeight: 600 }}>
                    {item.productName}
                  </Link>
                </td>
              ))}
            </tr>
            <tr>
              <th>Price</th>
              {items.map(item => {
                const hasSale = item.salePrice != null && item.salePrice < item.price

                return (
                  <td key={item.id}>
                    <span style={{ color: '#4f7d00', fontWeight: 700 }}>
                      ${(hasSale ? item.salePrice! : item.price).toFixed(2)}
                    </span>{' '}
                    {hasSale && <del style={{ color: '#999' }}>${item.price.toFixed(2)}</del>}
                  </td>
                )
              })}
            </tr>
            <tr>
              <th>Action</th>
              {items.map(item => (
                <td key={item.id}>
                  <button
                    type='button'
                    className='btn custom_btn rounded-pill px-3 mb-2 w-100'
                    onClick={() => addToCart(item.productId, 1, item.productName)}
                  >
                    Add to Cart
                  </button>
                  <button
                    type='button'
                    className='btn btn-link text-danger p-0'
                    onClick={() => removeItem.mutate(item.productId)}
                  >
                    <i className='fas fa-times' /> Remove
                  </button>
                </td>
              ))}
            </tr>
          </tbody>
        </table>
      </div>
    )
  }

  return (
    <>
      <Breadcrumb title='Compare Products' items={[{ label: 'Compare' }]} />
      <section className='sec_space_large'>
        <div className='container'>{body()}</div>
      </section>
    </>
  )
}

export default CompareView
