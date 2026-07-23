'use client'

// Next Imports
import Link from 'next/link'

// Hook Imports
import { useCategories } from '@/hooks/api/useCategories'

// Component Imports
import SectionTitle from '@/components/store/ui/SectionTitle'

const FALLBACKS = [1, 2, 3, 4, 5, 6, 7, 8].map(n => `/store/assets/images/category/cat${n}.png`)

const CategoryStrip = () => {
  const { data: categories } = useCategories()

  const items = (categories ?? []).filter(c => c.isActive).slice(0, 8)

  if (items.length === 0) return null

  return (
    <section className='category_section sec_ptb_100 position-relative overflow-hidden clearfix'>
      <div className='container'>
        <div className='row'>
          <div className='category_top_content d-flex justify-content-between'>
            <SectionTitle eyebrow='FRESH FROM OUR FARM' title='Popular Categories' />
          </div>
          <div className='row g-4 justify-content-center'>
            {items.map((cat, i) => (
              <div key={cat.id} className='col-6 col-sm-4 col-md-3 col-xl-2 item_content slider_item text-center'>
                <Link href={`/shop?category=${cat.slug}`}>
                  <div className='item_image_content overflow-hidden position-relative'>
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img src={cat.imageUrl || FALLBACKS[i % FALLBACKS.length]} alt={cat.name} />
                    <h6 className='item_title position-absolute rounded-pill'>{cat.name}</h6>
                  </div>
                </Link>
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  )
}

export default CategoryStrip
