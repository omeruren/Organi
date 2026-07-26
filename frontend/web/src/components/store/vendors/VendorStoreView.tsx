'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'

// Hook Imports
import { useVendorBySlug, useVendorProducts } from '@/hooks/api/useVendors'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import ProductCard from '@/components/store/ui/ProductCard'
import RatingStars from '@/components/store/ui/RatingStars'
import StorePagination from '@/components/store/ui/StorePagination'

const FALLBACK_BANNER = '/store/assets/images/vendor/vendor_bg_2.png'
const FALLBACK_LOGO = '/store/assets/images/brands/brand1.png'
const PAGE_SIZE = 9

const VendorStoreView = ({ slug }: { slug: string }) => {
  const [page, setPage] = useState(1)
  const { data: vendor, isLoading, error } = useVendorBySlug(slug)

  const { data: products, isLoading: productsLoading } = useVendorProducts({
    vendorId: vendor?.id ?? null,
    page,
    pageSize: PAGE_SIZE
  })

  if (isLoading) {
    return (
      <>
        <Breadcrumb title='Vendor' items={[{ label: 'Vendors', href: '/vendors' }]} />
        <section className='sec_space_large'>
          <div className='container'>
            <p className='text-center py-5'>Loading vendor…</p>
          </div>
        </section>
      </>
    )
  }

  if (error || !vendor) {
    const message = error instanceof ApiError && error.status === 404 ? 'This vendor could not be found.' : 'Something went wrong loading this vendor.'

    return (
      <>
        <Breadcrumb title='Vendor Not Found' items={[{ label: 'Vendors', href: '/vendors' }]} />
        <section className='sec_space_large'>
          <div className='container text-center py-5'>
            <p style={{ color: '#6b6b6b' }}>{message}</p>
            <Link href='/vendors' className='btn custom_btn rounded-pill px-4 text-white'>
              Back to Vendors
            </Link>
          </div>
        </section>
      </>
    )
  }

  const banner = vendor.bannerUrl || FALLBACK_BANNER
  const logo = vendor.logoUrl || FALLBACK_LOGO
  const items = products?.items ?? []
  const totalPages = products?.totalPages ?? 0

  return (
    <>
      <Breadcrumb title={vendor.storeName} items={[{ label: 'Vendors', href: '/vendors' }, { label: vendor.storeName }]} />

      <section className='sec_space_large'>
        <div className='container'>
          {/* Vendor header */}
          <div className='bg-white rounded-4 shadow-sm overflow-hidden mb-5'>
            <div style={{ height: 200 }}>
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src={banner} alt={vendor.storeName} className='w-100 h-100' style={{ objectFit: 'cover' }} />
            </div>
            <div className='p-4 d-flex flex-column flex-md-row align-items-md-center gap-4' style={{ marginTop: -48 }}>
              <div
                className='bg-white rounded-circle shadow-sm flex-shrink-0'
                style={{ width: 100, height: 100, overflow: 'hidden' }}
              >
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img src={logo} alt={vendor.storeName} className='w-100 h-100' style={{ objectFit: 'cover' }} />
              </div>
              <div className='flex-grow-1'>
                <h1 className='mb-1' style={{ fontWeight: 800, fontSize: 26 }}>
                  {vendor.storeName}
                </h1>
                <div className='d-flex flex-wrap align-items-center gap-3' style={{ color: '#6b6b6b', fontSize: 14 }}>
                  <RatingStars rating={vendor.rating} />
                  <span>
                    <i className='far fa-heart me-1' style={{ color: '#7cc000' }} />
                    {vendor.followerCount} followers
                  </span>
                  {vendor.city && (
                    <span>
                      <i className='fas fa-map-marker-alt me-1' style={{ color: '#7cc000' }} />
                      {vendor.city}
                    </span>
                  )}
                  {vendor.phoneNumber && (
                    <span>
                      <i className='fas fa-phone-alt me-1' style={{ color: '#7cc000' }} />
                      {vendor.phoneNumber}
                    </span>
                  )}
                </div>
                {vendor.description && (
                  <p className='mt-2 mb-0' style={{ color: '#6b6b6b', fontSize: 14 }}>
                    {vendor.description}
                  </p>
                )}
              </div>
            </div>
          </div>

          {/* Vendor products */}
          <h2 className='mb-4' style={{ fontWeight: 800, fontSize: 22 }}>
            Products
          </h2>
          {productsLoading ? (
            <p className='text-center py-5'>Loading products…</p>
          ) : items.length === 0 ? (
            <p className='text-center py-5' style={{ color: '#6b6b6b' }}>
              This vendor has no products yet.
            </p>
          ) : (
            <div className='row g-4'>
              {items.map(product => (
                <div key={product.id} className='col-sm-6 col-lg-4'>
                  <ProductCard product={product} />
                </div>
              ))}
            </div>
          )}

          <StorePagination page={page} totalPages={totalPages} onChange={setPage} />
        </div>
      </section>
    </>
  )
}

export default VendorStoreView
