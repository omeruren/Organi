'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'

// Hook Imports
import { useProductBySlug } from '@/hooks/api/useProducts'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import RatingStars from '@/components/store/ui/RatingStars'
import Price from '@/components/store/ui/Price'
import QuantityStepper from '@/components/store/ui/QuantityStepper'
import ProductGallery from '@/components/store/product/ProductGallery'
import ProductReviews from '@/components/store/product/ProductReviews'
import RelatedProducts from '@/components/store/product/RelatedProducts'

type Tab = 'description' | 'info' | 'reviews'

const ProductDetailView = ({ slug }: { slug: string }) => {
  const { data: product, isLoading, error } = useProductBySlug(slug)
  const [qty, setQty] = useState(1)
  const [tab, setTab] = useState<Tab>('description')
  const [notice, setNotice] = useState('')

  const stub = (message: string) => {
    setNotice(message)
    window.setTimeout(() => setNotice(''), 2500)
  }

  if (isLoading) {
    return <div className='container py-5 text-center'>Loading product…</div>
  }

  if (error || !product) {
    // A failed PDP fetch is, in practice, a missing/removed product (404 dominates); a
    // customer doesn't benefit from distinguishing transport errors here.
    return (
      <div className='container py-5 text-center'>
        <h3>Product not found</h3>
        <p className='text-muted'>The product you&rsquo;re looking for isn&rsquo;t available.</p>
        <Link href='/shop' className='btn custom_btn rounded-pill px-4 text-white mt-3'>
          Back to Shop
        </Link>
      </div>
    )
  }

  const inStock = product.stockQuantity > 0

  const meta: [string, React.ReactNode][] = [
    ['SKU', product.sku],
    ['Category', product.categoryName],
    ['Vendor', product.vendorName],
    ['Unit', product.unit],
    ...(product.weight != null ? ([['Weight', `${product.weight} g`]] as [string, React.ReactNode][]) : []),
    ['Availability', inStock ? `In stock (${product.stockQuantity})` : 'Out of stock']
  ]

  return (
    <>
      <Breadcrumb title={product.name} items={[{ label: 'Shop', href: '/shop' }, { label: product.name }]} />

      <section className='product10_sec sec_space_small'>
        <div className='container'>
          <div className='row justify-content-center'>
            <div className='col-lg-6 position-relative'>
              <ProductGallery images={product.images} primaryImageUrl={product.primaryImageUrl} name={product.name} />
            </div>
            <div className='col-lg-6'>
              <div className='rating_wrap d-flex align-items-center gap-3'>
                <RatingStars rating={product.averageRating} />
                <a
                  href='#reviews'
                  className='review'
                  onClick={() => setTab('reviews')}
                >{`${product.reviewCount} Reviews`}</a>
              </div>

              <h2 className='product_detail_title mt-2'>{product.name}</h2>

              <div className='my-3' style={{ fontSize: 26, fontWeight: 800 }}>
                <Price price={product.price} salePrice={product.salePrice} />
              </div>

              <div className='d-flex gap-2 mb-3'>
                {product.isOrganic && (
                  <span className='badge_meats rounded-pill text-uppercase px-3 py-1 text-white' style={{ background: '#7cc000' }}>
                    Organic
                  </span>
                )}
                <span
                  className='rounded-pill px-3 py-1 text-white'
                  style={{ background: inStock ? '#28a745' : '#dc3545' }}
                >
                  {inStock ? 'In Stock' : 'Out of Stock'}
                </span>
              </div>

              {(product.shortDescription || product.description) && (
                <p className='product_detail_desc py-2'>{product.shortDescription || product.description}</p>
              )}

              <ul className='list-unstyled product_meta_list my-3'>
                {meta.map(([label, value]) => (
                  <li key={label} className='d-flex' style={{ padding: '4px 0' }}>
                    <span style={{ width: 130, color: '#6b6b6b' }}>{label}:</span>
                    <span style={{ color: '#292929', fontWeight: 500 }}>{value}</span>
                  </li>
                ))}
              </ul>

              <div className='product10_quantity_btn_wrap d-flex align-items-center flex-wrap gap-3 mt-3'>
                <QuantityStepper value={qty} onChange={setQty} max={Math.max(1, product.stockQuantity)} />
                <button
                  type='button'
                  className='btn custom_btn rounded-pill px-5 py-3 text-white'
                  disabled={!inStock}
                  onClick={() => stub('Cart & checkout are coming soon.')}
                >
                  Add to Cart <i className='fas fa-shopping-bag ms-1' />
                </button>
                <button
                  type='button'
                  className='btn rounded-circle p-3 border'
                  aria-label='Add to wishlist'
                  onClick={() => stub('Wishlist is coming soon.')}
                >
                  <i className='far fa-heart' />
                </button>
                <button
                  type='button'
                  className='btn rounded-circle p-3 border'
                  aria-label='Add to compare'
                  onClick={() => stub('Compare is coming soon.')}
                >
                  <i className='fas fa-exchange-alt' />
                </button>
              </div>
              {notice && (
                <p className='mt-2 mb-0' style={{ color: '#7cc000', fontWeight: 600 }}>
                  {notice}
                </p>
              )}
            </div>
          </div>
        </div>
      </section>

      {/* Tabs */}
      <section id='reviews' className='product10_reviews sec_inner_bottom_80'>
        <div className='container'>
          <ul className='product_tabnav_3 nav nav-pills my-4'>
            {(
              [
                ['description', 'Description'],
                ['info', 'Additional Information'],
                ['reviews', `Reviews (${product.reviewCount})`]
              ] as [Tab, string][]
            ).map(([id, label]) => (
              <li key={id} className='nav-item'>
                <button
                  type='button'
                  className={`nav-link shadow rounded-pill text-uppercase${tab === id ? ' active' : ''}`}
                  onClick={() => setTab(id)}
                >
                  {label}
                </button>
              </li>
            ))}
          </ul>

          <div className='tab-content'>
            {tab === 'description' && (
              <div className='content_wrap'>
                <h3 className='title_text'>Description</h3>
                <p className='mb_15'>{product.description || 'No description available for this product.'}</p>
              </div>
            )}
            {tab === 'info' && (
              <div className='content_wrap'>
                <h3 className='info_content_title'>Additional Information</h3>
                <table className='table w-auto'>
                  <tbody>
                    {meta.map(([label, value]) => (
                      <tr key={label}>
                        <th scope='row' className='pe-4'>
                          {label}
                        </th>
                        <td>{value}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
            {tab === 'reviews' && <ProductReviews productId={product.id} />}
          </div>
        </div>
      </section>

      <RelatedProducts categoryId={product.categoryId} excludeId={product.id} />
    </>
  )
}

export default ProductDetailView
