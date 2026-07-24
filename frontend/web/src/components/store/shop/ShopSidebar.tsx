'use client'

// React Imports
import { useEffect, useState } from 'react'

// Hook Imports
import { useCategories } from '@/hooks/api/useCategories'

// Type Imports
import type { ShopFilters } from '@/components/store/shop/ShopView'

const ShopSidebar = ({
  filters,
  onChange
}: {
  filters: ShopFilters
  onChange: (patch: Partial<ShopFilters>) => void
}) => {
  const { data: categories } = useCategories()
  const cats = (categories ?? []).filter(c => c.isActive)

  // Local (uncommitted) input state, seeded from the committed filters.
  const [search, setSearch] = useState(filters.search)
  const [minPrice, setMinPrice] = useState(filters.minPrice)
  const [maxPrice, setMaxPrice] = useState(filters.maxPrice)

  useEffect(() => setSearch(filters.search), [filters.search])
  useEffect(() => setMinPrice(filters.minPrice), [filters.minPrice])
  useEffect(() => setMaxPrice(filters.maxPrice), [filters.maxPrice])

  return (
    <div className='shop_sidebar'>
      <div className='shop_sidebar_searchbar position-relative mb-4'>
        <form
          onSubmit={e => {
            e.preventDefault()
            onChange({ search })
          }}
        >
          <input
            type='search'
            className='form-control rounded-pill'
            placeholder='Search products…'
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
          <button type='submit' className='submit_btn' aria-label='Search'>
            <i className='fas fa-search' />
          </button>
        </form>
      </div>

      <div className='blog_category'>
        <div className='blog_category_wrap'>
          <h3 className='blog_category_title py-3'>Categories</h3>
          <div className='blog_category_item'>
            <ul className='list-unstyled'>
              <li>
                <button
                  type='button'
                  className={`shop_cat_link${filters.category === '' ? ' active' : ''}`}
                  onClick={() => onChange({ category: '' })}
                >
                  All Categories
                </button>
              </li>
              {cats.map(cat => (
                <li key={cat.id}>
                  <button
                    type='button'
                    className={`shop_cat_link${filters.category === cat.slug ? ' active' : ''}`}
                    onClick={() => onChange({ category: cat.slug })}
                  >
                    {cat.name}
                  </button>
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>

      <div className='price-range-area mt-4'>
        <h3 className='price_range_title mb-3'>Filter By Price</h3>
        <div className='d-flex align-items-center gap-2'>
          <input
            type='number'
            className='form-control'
            placeholder='Min'
            min={0}
            value={minPrice}
            onChange={e => setMinPrice(e.target.value)}
          />
          <span>–</span>
          <input
            type='number'
            className='form-control'
            placeholder='Max'
            min={0}
            value={maxPrice}
            onChange={e => setMaxPrice(e.target.value)}
          />
        </div>
        <div className='price-text d-flex align-items-center mt-3'>
          <button
            type='button'
            className='btn custom_btn rounded-pill px-4 text-white'
            onClick={() => onChange({ minPrice, maxPrice })}
          >
            Filter <i className='fas fa-long-arrow-alt-right' />
          </button>
        </div>
      </div>

      <div className='mt-4'>
        <label className='d-flex align-items-center gap-2' style={{ cursor: 'pointer' }}>
          <input
            type='checkbox'
            className='form-check-input mt-0'
            checked={filters.organic}
            onChange={e => onChange({ organic: e.target.checked })}
          />
          <span>Organic only</span>
        </label>
      </div>
    </div>
  )
}

export default ShopSidebar
