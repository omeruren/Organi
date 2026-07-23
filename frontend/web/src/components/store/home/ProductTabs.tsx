'use client'

// React Imports
import { useState } from 'react'

// Hook Imports
import { useProducts } from '@/hooks/api/useProducts'
import { useCategories } from '@/hooks/api/useCategories'

// Component Imports
import SectionTitle from '@/components/store/ui/SectionTitle'
import ProductCard from '@/components/store/ui/ProductCard'

const ProductTabs = () => {
  const { data: categories } = useCategories()

  const topCategories = (categories ?? []).filter(c => c.isActive).slice(0, 4)

  const tabs = [{ id: 'all', label: 'All', categoryId: undefined as string | undefined }].concat(
    topCategories.map(c => ({ id: c.id, label: c.name, categoryId: c.id }))
  )

  const [active, setActive] = useState('all')
  const activeTab = tabs.find(t => t.id === active) ?? tabs[0]

  // Anonymous callers get only Active products from approved vendors (backend-enforced).
  const { data, isLoading } = useProducts({ page: 1, pageSize: 8, categoryId: activeTab.categoryId })

  const products = data?.items ?? []

  return (
    <section className='product_section sec_space_xxs_50'>
      <div className='container'>
        <div className='row align-items-center'>
          <div className='col-lg-6'>
            <SectionTitle eyebrow='FRESH FROM OUR FARM' title='Our Organic Products' />
          </div>
          <div className='col-lg-6'>
            <ul className='product_tabnav_1 nav nav-pills ul_li_right mb-3'>
              {tabs.map(tab => (
                <li key={tab.id} className='nav-item'>
                  <button
                    type='button'
                    className={`nav-link rounded-pill me-1${active === tab.id ? ' active' : ''}`}
                    onClick={() => setActive(tab.id)}
                  >
                    {tab.label}
                  </button>
                </li>
              ))}
            </ul>
          </div>
        </div>

        <div className='tab-content'>
          <div className='tab-pane fade show active'>
            {isLoading ? (
              <p className='text-center py-5'>Loading products…</p>
            ) : products.length === 0 ? (
              <p className='text-center py-5' style={{ color: '#6b6b6b' }}>
                No products in this category yet.
              </p>
            ) : (
              <div className='row g-4'>
                {products.map(product => (
                  <div key={product.id} className='col-sm-6 col-md-6 col-xl-3'>
                    <ProductCard product={product} />
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </section>
  )
}

export default ProductTabs
