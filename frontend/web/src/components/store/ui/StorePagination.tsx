'use client'

// Template `.pagination_nav` — server pagination, current page marked `.active`.
const StorePagination = ({
  page,
  totalPages,
  onChange
}: {
  page: number
  totalPages: number
  onChange: (nextPage: number) => void
}) => {
  if (totalPages <= 1) return null

  const pages = Array.from({ length: totalPages }, (_, i) => i + 1)

  return (
    <ul className='pagination_nav mt-5 list-unstyled d-flex justify-content-center text-uppercase clearfix'>
      {pages.map(p => (
        <li key={p} className={p === page ? 'active' : ''}>
          <a
            href='#!'
            onClick={e => {
              e.preventDefault()
              if (p !== page) onChange(p)
            }}
          >
            {p}
          </a>
        </li>
      ))}
      {page < totalPages && (
        <li>
          <a
            href='#!'
            aria-label='Next page'
            onClick={e => {
              e.preventDefault()
              onChange(page + 1)
            }}
          >
            <i className='fas fa-arrow-right' />
          </a>
        </li>
      )}
    </ul>
  )
}

export default StorePagination
