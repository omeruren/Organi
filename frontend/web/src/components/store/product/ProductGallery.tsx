'use client'

// React Imports
import { useState } from 'react'

// Type Imports
import type { ProductImageResponse } from '@/types/api/product'

const FALLBACK_IMAGE = '/store/assets/images/product/product1.png'

const ProductGallery = ({
  images,
  primaryImageUrl,
  name
}: {
  images: ProductImageResponse[]
  primaryImageUrl: string | null
  name: string
}) => {
  // Prefer the images[] list (primary first); fall back to the summary primary image / template image.
  const gallery =
    images.length > 0
      ? [...images].sort((a, b) => Number(b.isPrimary) - Number(a.isPrimary) || a.displayOrder - b.displayOrder)
      : [{ id: 'fallback', imageUrl: primaryImageUrl || FALLBACK_IMAGE, altText: name, displayOrder: 0, isPrimary: true }]

  const [active, setActive] = useState(0)
  const current = gallery[Math.min(active, gallery.length - 1)]

  return (
    <div className='product_detail_gallery'>
      <div className='product_detail_main_img d-flex justify-content-center align-items-center bg-white rounded-4 p-4'>
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img src={current.imageUrl} alt={current.altText || name} style={{ maxHeight: 420, objectFit: 'contain' }} />
      </div>
      {gallery.length > 1 && (
        <div className='product_detail_thumbs d-flex gap-3 mt-4 flex-wrap'>
          {gallery.map((img, i) => (
            <button
              key={img.id}
              type='button'
              onClick={() => setActive(i)}
              className='bg-white rounded-3 p-2'
              style={{
                border: i === active ? '2px solid #7cc000' : '2px solid #eee',
                width: 84,
                height: 84,
                cursor: 'pointer'
              }}
              aria-label={`View image ${i + 1}`}
            >
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img
                src={img.imageUrl}
                alt={img.altText || name}
                style={{ width: '100%', height: '100%', objectFit: 'contain' }}
              />
            </button>
          ))}
        </div>
      )}
    </div>
  )
}

export default ProductGallery
