'use client'

// Hook Imports
import { useProducts } from '@/hooks/api/useProducts'

// Component Imports
import SectionTitle from '@/components/store/ui/SectionTitle'
import ProductCard from '@/components/store/ui/ProductCard'

const RelatedProducts = ({ categoryId, excludeId }: { categoryId: string; excludeId: string }) => {
  const { data } = useProducts({ page: 1, pageSize: 8, categoryId })
  const items = (data?.items ?? []).filter(p => p.id !== excludeId).slice(0, 4)

  if (items.length === 0) return null

  return (
    <section className='product_section sec_top_space_50 sec_inner_bottom_100'>
      <div className='container'>
        <div className='mb-4 d-flex flex-column align-items-start'>
          <SectionTitle eyebrow='FRESH FROM OUR FARM' title='Related Products' />
        </div>
        <div className='row g-4'>
          {items.map(product => (
            <div key={product.id} className='col-sm-6 col-lg-3'>
              <ProductCard product={product} />
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

export default RelatedProducts
