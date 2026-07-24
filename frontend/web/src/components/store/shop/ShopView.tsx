'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import { useSearchParams, useRouter, usePathname } from 'next/navigation'

// Hook Imports
import { useProducts } from '@/hooks/api/useProducts'
import { useCategories } from '@/hooks/api/useCategories'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import ProductCard from '@/components/store/ui/ProductCard'
import ProductListCard from '@/components/store/ui/ProductListCard'
import StorePagination from '@/components/store/ui/StorePagination'
import ShopSidebar from '@/components/store/shop/ShopSidebar'
import ShopToolbar from '@/components/store/shop/ShopToolbar'

export type ShopViewMode = 'grid' | 'list'

export interface ShopFilters {
  category: string // category slug ('' = all)
  search: string
  minPrice: string
  maxPrice: string
  organic: boolean
  sort: string
  page: number
}

const PAGE_SIZE = 9

const SORT_MAP: Record<string, { sortBy?: string; sortOrder?: string }> = {
  newest: { sortBy: 'createdAt', sortOrder: 'desc' },
  'price-asc': { sortBy: 'price', sortOrder: 'asc' },
  'price-desc': { sortBy: 'price', sortOrder: 'desc' },
  rating: { sortBy: 'rating', sortOrder: 'desc' },
  name: { sortBy: 'name', sortOrder: 'asc' }
}

const ShopView = () => {
  const searchParams = useSearchParams()
  const router = useRouter()
  const pathname = usePathname()
  const [view, setView] = useState<ShopViewMode>('grid')

  const filters: ShopFilters = {
    category: searchParams.get('category') ?? '',
    search: searchParams.get('search') ?? '',
    minPrice: searchParams.get('minPrice') ?? '',
    maxPrice: searchParams.get('maxPrice') ?? '',
    organic: searchParams.get('organic') === '1',
    sort: searchParams.get('sort') ?? 'newest',
    page: Math.max(1, Number(searchParams.get('page')) || 1)
  }

  const { data: categories } = useCategories()
  const categoryId = filters.category ? (categories ?? []).find(c => c.slug === filters.category)?.id : undefined

  // Wait for categories before fetching when a category slug is in the URL, so we don't
  // briefly fetch the whole (unfiltered) catalog before the slug→id resolves.
  const categoryResolving = !!filters.category && categories === undefined
  const sort = SORT_MAP[filters.sort] ?? SORT_MAP.newest

  const { data, isLoading } = useProducts({
    page: filters.page,
    pageSize: PAGE_SIZE,
    categoryId,
    search: filters.search || undefined,
    isOrganic: filters.organic ? true : undefined,
    minPrice: filters.minPrice ? Number(filters.minPrice) : undefined,
    maxPrice: filters.maxPrice ? Number(filters.maxPrice) : undefined,
    sortBy: sort.sortBy,
    sortOrder: sort.sortOrder,
    enabled: !categoryResolving
  })

  const update = (patch: Partial<ShopFilters>) => {
    const params = new URLSearchParams(searchParams.toString())
    const set = (key: string, value: string) => (value ? params.set(key, value) : params.delete(key))

    if ('category' in patch) set('category', patch.category ?? '')
    if ('search' in patch) set('search', patch.search ?? '')
    if ('minPrice' in patch) set('minPrice', patch.minPrice ?? '')
    if ('maxPrice' in patch) set('maxPrice', patch.maxPrice ?? '')
    if ('organic' in patch) set('organic', patch.organic ? '1' : '')
    if ('sort' in patch) set('sort', patch.sort === 'newest' ? '' : patch.sort ?? '')

    // Page: explicit page change keeps it; any other filter change resets to page 1.
    if ('page' in patch) set('page', patch.page && patch.page > 1 ? String(patch.page) : '')
    else params.delete('page')

    router.push(`${pathname}?${params.toString()}`, { scroll: false })
  }

  const products = data?.items ?? []
  const total = data?.totalCount ?? 0
  const totalPages = data?.totalPages ?? 0
  const start = total === 0 ? 0 : (filters.page - 1) * PAGE_SIZE + 1
  const end = Math.min(filters.page * PAGE_SIZE, total)
  const rangeText = total === 0 ? 'No results' : `Showing ${start}–${end} of ${total} results`

  return (
    <>
      <Breadcrumb title='Shop' items={[{ label: 'Shop' }]} />
      <section className='shop_list_sidebar sec_space_large'>
        <div className='container'>
          <div className='row'>
            <div className='col-lg-3'>
              <ShopSidebar filters={filters} onChange={update} />
            </div>
            <div className='col-lg-9'>
              <ShopToolbar
                filters={filters}
                onChange={update}
                view={view}
                onViewChange={setView}
                rangeText={rangeText}
              />
              {isLoading ? (
                <p className='text-center py-5'>Loading products…</p>
              ) : products.length === 0 ? (
                <p className='text-center py-5' style={{ color: '#6b6b6b' }}>
                  No products match your filters.
                </p>
              ) : view === 'grid' ? (
                <div className='row g-4'>
                  {products.map(product => (
                    <div key={product.id} className='col-sm-6 col-lg-4'>
                      <ProductCard product={product} />
                    </div>
                  ))}
                </div>
              ) : (
                <div className='row g-3'>
                  {products.map(product => (
                    <div key={product.id} className='col-12'>
                      <ProductListCard product={product} />
                    </div>
                  ))}
                </div>
              )}
              <StorePagination page={filters.page} totalPages={totalPages} onChange={page => update({ page })} />
            </div>
          </div>
        </div>
      </section>
    </>
  )
}

export default ShopView
