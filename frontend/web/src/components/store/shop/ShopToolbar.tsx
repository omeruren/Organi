'use client'

// Type Imports
import type { ShopFilters, ShopViewMode } from '@/components/store/shop/ShopView'

export const SORT_OPTIONS: { value: string; label: string }[] = [
  { value: 'newest', label: 'Newest' },
  { value: 'price-asc', label: 'Price: Low to High' },
  { value: 'price-desc', label: 'Price: High to Low' },
  { value: 'rating', label: 'Top Rated' },
  { value: 'name', label: 'Name: A–Z' }
]

const ShopToolbar = ({
  filters,
  onChange,
  view,
  onViewChange,
  rangeText
}: {
  filters: ShopFilters
  onChange: (patch: Partial<ShopFilters>) => void
  view: ShopViewMode
  onViewChange: (view: ShopViewMode) => void
  rangeText: string
}) => {
  return (
    <div className='filter_area d-flex justify-content-between align-items-center flex-wrap gap-3 mb_30'>
      <ul className='nav layout_tab_nav ul_li align-items-center mb-0'>
        <li>
          <button
            type='button'
            className={view === 'grid' ? 'active' : ''}
            onClick={() => onViewChange('grid')}
            aria-label='Grid view'
          >
            <i className='fas fa-th' />
          </button>
        </li>
        <li>
          <button
            type='button'
            className={view === 'list' ? 'active' : ''}
            onClick={() => onViewChange('list')}
            aria-label='List view'
          >
            <i className='fas fa-list' />
          </button>
        </li>
        <span className='show_result ms-2'>{rangeText}</span>
      </ul>

      <div className='sorting_from d-flex align-items-center gap-2'>
        <span className='sorting_from_title'>Sort by:</span>
        <select
          className='form-select rounded-pill'
          value={filters.sort}
          onChange={e => onChange({ sort: e.target.value })}
        >
          {SORT_OPTIONS.map(option => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </div>
    </div>
  )
}

export default ShopToolbar
