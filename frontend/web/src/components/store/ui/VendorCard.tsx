// Next Imports
import Link from 'next/link'

// Component Imports
import RatingStars from '@/components/store/ui/RatingStars'

// Type Imports
import type { VendorResponse } from '@/types/api/vendor'

const FALLBACK_BANNER = '/store/assets/images/vendor/vendor_bg_1.png'
const FALLBACK_LOGO = '/store/assets/images/brands/brand1.png'

// Storefront vendor directory card — banner + overlaid logo, store meta, and a link to the store page.
const VendorCard = ({ vendor }: { vendor: VendorResponse }) => {
  const href = `/vendors/${vendor.slug}`
  const banner = vendor.bannerUrl || FALLBACK_BANNER
  const logo = vendor.logoUrl || FALLBACK_LOGO

  return (
    <div className='bg-white rounded-4 shadow-sm overflow-hidden h-100 d-flex flex-column'>
      <Link href={href} className='d-block position-relative' style={{ height: 140 }}>
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img src={banner} alt={vendor.storeName} className='w-100 h-100' style={{ objectFit: 'cover' }} />
      </Link>
      <div className='px-4 pb-4 flex-grow-1 d-flex flex-column' style={{ marginTop: -36 }}>
        <Link
          href={href}
          className='d-inline-flex align-items-center justify-content-center bg-white rounded-circle shadow-sm mb-3'
          style={{ width: 72, height: 72, overflow: 'hidden' }}
        >
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={logo} alt={vendor.storeName} className='w-100 h-100' style={{ objectFit: 'cover' }} />
        </Link>
        <h3 className='product_title mb-1' style={{ fontSize: 18 }}>
          <Link href={href}>{vendor.storeName}</Link>
        </h3>
        {vendor.city && (
          <p className='mb-2' style={{ color: '#6b6b6b', fontSize: 13 }}>
            <i className='fas fa-map-marker-alt me-1' style={{ color: '#7cc000' }} />
            {vendor.city}
          </p>
        )}
        <RatingStars rating={vendor.rating} />
        {vendor.description && (
          <p className='mt-2 mb-3' style={{ color: '#6b6b6b', fontSize: 14, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
            {vendor.description}
          </p>
        )}
        <div className='d-flex justify-content-between align-items-center mt-auto pt-2'>
          <span style={{ color: '#6b6b6b', fontSize: 13 }}>
            <i className='far fa-heart me-1' />
            {vendor.followerCount} followers
          </span>
          <Link href={href} className='btn custom_btn rounded-pill px-3 py-2 text-white' style={{ fontSize: 13 }}>
            Visit Store
          </Link>
        </div>
      </div>
    </div>
  )
}

export default VendorCard
