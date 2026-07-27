'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import { useSearchParams, useRouter, usePathname } from 'next/navigation'

// Hook Imports
import { useBlogPosts } from '@/hooks/api/useBlog'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import BlogCard from '@/components/store/ui/BlogCard'
import StorePagination from '@/components/store/ui/StorePagination'
import StoreStateMessage from '@/components/store/ui/StoreStateMessage'

const PAGE_SIZE = 9

const BlogView = () => {
  const searchParams = useSearchParams()
  const router = useRouter()
  const pathname = usePathname()

  const search = searchParams.get('search') ?? ''
  const page = Math.max(1, Number(searchParams.get('page')) || 1)

  const [searchInput, setSearchInput] = useState(search)

  const { data, isLoading, isError, refetch } = useBlogPosts({ page, pageSize: PAGE_SIZE, search: search || undefined })

  const update = (patch: { search?: string; page?: number }) => {
    const params = new URLSearchParams(searchParams.toString())

    if ('search' in patch) {
      if (patch.search) params.set('search', patch.search)
      else params.delete('search')
      params.delete('page')
    }

    if ('page' in patch) {
      if (patch.page && patch.page > 1) params.set('page', String(patch.page))
      else params.delete('page')
    }

    router.push(`${pathname}?${params.toString()}`, { scroll: false })
  }

  const onSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    update({ search: searchInput.trim() })
  }

  const posts = data?.items ?? []
  const totalPages = data?.totalPages ?? 0

  return (
    <>
      <Breadcrumb title='Blog' items={[{ label: 'Blog' }]} />
      <section className='sec_space_large'>
        <div className='container'>
          <div className='row justify-content-center mb-5'>
            <div className='col-lg-6'>
              <form onSubmit={onSearchSubmit}>
                <div className='position-relative'>
                  <input
                    type='search'
                    className='form-control rounded-pill py-3 pe-5'
                    aria-label='Search articles'
                    placeholder='Search articles…'
                    value={searchInput}
                    onChange={e => setSearchInput(e.target.value)}
                  />
                  <button
                    type='submit'
                    className='btn position-absolute end-0 top-50 translate-middle-y me-2'
                    aria-label='Search'
                    style={{ color: '#4f7d00' }}
                  >
                    <i className='fas fa-search' />
                  </button>
                </div>
              </form>
            </div>
          </div>

          {isLoading ? (
            <p className='text-center py-5'>Loading articles…</p>
          ) : isError ? (
            <StoreStateMessage
              icon='fas fa-triangle-exclamation'
              title='Could not load articles'
              message='Something went wrong while loading the blog. Please try again.'
              onRetry={() => refetch()}
            />
          ) : posts.length === 0 ? (
            <p className='text-center py-5' style={{ color: '#6b6b6b' }}>
              No articles found.
            </p>
          ) : (
            <div className='row g-4'>
              {posts.map(post => (
                <div key={post.id} className='col-sm-6 col-lg-4'>
                  <BlogCard post={post} />
                </div>
              ))}
            </div>
          )}

          <StorePagination page={page} totalPages={totalPages} onChange={p => update({ page: p })} />
        </div>
      </section>
    </>
  )
}

export default BlogView
