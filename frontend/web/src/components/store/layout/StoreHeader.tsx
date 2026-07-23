'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'
import { usePathname, useRouter } from 'next/navigation'

// Hook Imports
import { useStickyHeader } from '@/hooks/useStickyHeader'

// Component Imports
import MiniCartDrawer from '@/components/store/layout/MiniCartDrawer'

const NAV_LINKS = [
  { label: 'Home', href: '/' },
  { label: 'Shop', href: '/shop' },
  { label: 'Vendors', href: '/vendors' },
  { label: 'Blog', href: '/blog' },
  { label: 'About', href: '/about' },
  { label: 'Contact', href: '/contact' }
]

const StoreHeader = () => {
  const pathname = usePathname()
  const router = useRouter()
  const isSticky = useStickyHeader()

  const [searchOpen, setSearchOpen] = useState(false)
  const [mobileOpen, setMobileOpen] = useState(false)
  const [cartOpen, setCartOpen] = useState(false)
  const [userOpen, setUserOpen] = useState(false)
  const [search, setSearch] = useState('')

  const isActive = (href: string) => (href === '/' ? pathname === '/' : pathname.startsWith(href))

  const onSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setSearchOpen(false)

    if (search.trim()) router.push(`/shop?search=${encodeURIComponent(search.trim())}`)
  }

  const navList = (onNavigate?: () => void) => (
    <ul className='navbar-nav main_menu_list m-auto'>
      {NAV_LINKS.map(link => (
        <li key={link.href} className='nav-item px-2'>
          <Link
            className={`nav-link${isActive(link.href) ? ' active' : ''}`}
            href={link.href}
            onClick={onNavigate}
          >
            {link.label}
          </Link>
        </li>
      ))}
    </ul>
  )

  const userIcons = (
    <div className='navbar_user_icon'>
      <ul className='list-unstyled d-flex mb-0 align-items-center'>
        <li className='pe-3'>
          <button className='main_search_btn' type='button' onClick={() => setSearchOpen(o => !o)} aria-label='Search'>
            <i className={`search_icon fas ${searchOpen ? 'fa-times' : 'fa-search'}`} />
          </button>
        </li>
        <li className='pe-2 position-relative'>
          <button
            type='button'
            className='main_search_btn'
            onClick={() => setUserOpen(o => !o)}
            aria-label='Account'
          >
            <i className='far fa-user' />
          </button>
          <div className={`collapse_dropdown collapse${userOpen ? ' show' : ''}`}>
            <div className='dropdown_content'>
              <ul className='settings_options ul_li_block clearfix'>
                <li>
                  <Link href='/login' onClick={() => setUserOpen(false)}>
                    <i className='fas fa-sign-in-alt' /> Login
                  </Link>
                </li>
                <li>
                  <Link href='/register' onClick={() => setUserOpen(false)}>
                    <i className='fas fa-user-plus' /> Register
                  </Link>
                </li>
                <li>
                  <Link href='/account' onClick={() => setUserOpen(false)}>
                    <i className='fas fa-user-circle' /> My Account
                  </Link>
                </li>
              </ul>
            </div>
          </div>
        </li>
        <li className='pe-2'>
          <Link href='/wishlist' aria-label='Wishlist'>
            <i className='far fa-heart' />
          </Link>
        </li>
        <li>
          <button type='button' className='main_search_btn' onClick={() => setCartOpen(true)} aria-label='Cart'>
            <i className='fas fa-shopping-bag' />
          </button>
        </li>
      </ul>
    </div>
  )

  return (
    <header className='header_section header_1'>
      {/* top header */}
      <div className='top_header_main d-none d-lg-block'>
        <div className='container'>
          <div className='header_top d-flex align-items-center justify-content-between'>
            <div className='header_top_content d-flex pt-2'>
              <div className='mail_text_content d-flex'>
                <p className='mail_icon'>
                  <span>
                    <i className='far fa-envelope text-white pe-2' />
                  </span>
                </p>
                <p className='mail_text'>info@organi.dev</p>
              </div>
              <div className='address_text_content d-flex'>
                <p className='mail_icon'>
                  <span>
                    <i className='fas fa-map-marker-alt text-white pe-2' />
                  </span>
                </p>
                <p className='address_text'>15/A, Nest Tower, NYC</p>
              </div>
            </div>
            <div className='header_top_socials pt-2'>
              <ul className='list-unstyled d-flex'>
                <li>
                  <a href='#!'>
                    <i className='fab fa-facebook-f text-white pe-3' />
                  </a>
                </li>
                <li>
                  <a href='#!'>
                    <i className='fab fa-twitter text-white pe-3' />
                  </a>
                </li>
                <li>
                  <a href='#!'>
                    <i className='fab fa-instagram text-white pe-3' />
                  </a>
                </li>
                <li>
                  <a href='#!'>
                    <i className='fab fa-linkedin-in text-white' />
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      {/* bottom header */}
      <div className={`header_bottom_main${isSticky ? ' sticky' : ''}`}>
        <div className='container'>
          {/* desktop */}
          <div className='webMenu d-none d-lg-block position-relative'>
            <nav className='navbar navbar-expand-lg navbar-light'>
              <Link className='navbar-brand position-relative' href='/'>
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img src='/store/assets/images/logo/logo.png' alt='Organi' />
              </Link>
              <div className='collapse navbar-collapse'>{navList()}</div>
              {userIcons}
            </nav>
          </div>

          {/* mobile */}
          <div className='mobileMenu d-block d-lg-none'>
            <nav className='navbar navbar-expand-lg navbar-light'>
              <Link className='navbar-brand' href='/'>
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img src='/store/assets/images/logo/logo.png' alt='Organi' />
              </Link>
              <button
                className='navbar-toggler'
                type='button'
                onClick={() => setMobileOpen(true)}
                aria-label='Toggle navigation'
              >
                <span className='navbar-toggler-icon' />
              </button>
              <div className={`offcanvas offcanvas-start${mobileOpen ? ' show' : ''}`} tabIndex={-1}>
                <div className='offcanvas-header'>
                  <button type='button' className='btn-close mobile_menu_close' onClick={() => setMobileOpen(false)} aria-label='Close' />
                </div>
                <div className='offcanvas-body'>{navList(() => setMobileOpen(false))}</div>
              </div>
              <div className='navbar_user me-2'>{userIcons}</div>
            </nav>
          </div>
        </div>

        {/* collapse search */}
        <div className={`main_search_collapse collapse${searchOpen ? ' show' : ''}`}>
          <div className='main_search_form position-relative card'>
            <div className='container'>
              <form onSubmit={onSearchSubmit}>
                <div className='form_item position-relative'>
                  <input
                    type='search'
                    className='form-control rounded-pill py-3'
                    placeholder='Search Your Product...'
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                  />
                  <button type='submit' className='submit_btn'>
                    <i className='fas fa-search' />
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>

      {/* mobile menu backdrop */}
      {mobileOpen && <div className='offcanvas-backdrop fade show' onClick={() => setMobileOpen(false)} />}

      <MiniCartDrawer open={cartOpen} onClose={() => setCartOpen(false)} />
    </header>
  )
}

export default StoreHeader
