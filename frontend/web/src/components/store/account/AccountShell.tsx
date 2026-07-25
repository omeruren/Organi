'use client'

// Next Imports
import Link from 'next/link'
import { usePathname } from 'next/navigation'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'

const NAV = [
  { label: 'Dashboard', href: '/account', icon: 'fa-th-large' },
  { label: 'My Orders', href: '/account/orders', icon: 'fa-box' },
  { label: 'Profile', href: '/account/profile', icon: 'fa-user' },
  { label: 'Wishlist', href: '/wishlist', icon: 'fa-heart' },
  { label: 'Compare', href: '/compare', icon: 'fa-exchange-alt' }
]

const AccountShell = ({ children }: { children: React.ReactNode }) => {
  const pathname = usePathname()
  const { user, isLoading, logout } = useAuth()

  const isActive = (href: string) => (href === '/account' ? pathname === '/account' : pathname.startsWith(href))

  return (
    <>
      <Breadcrumb title='My Account' items={[{ label: 'Account' }]} />
      <section className='sec_space_large'>
        <div className='container'>
          {isLoading ? (
            <p className='text-center py-5'>Loading…</p>
          ) : !user ? (
            <div className='text-center py-5'>
              <p>Please log in to access your account.</p>
              <Link href='/login?redirectTo=/account' className='btn custom_btn rounded-pill px-4 text-white'>
                Login
              </Link>
            </div>
          ) : (
            <div className='row g-4'>
              <div className='col-lg-3'>
                <div className='bg-white rounded-4 shadow-sm p-4 account_sidebar'>
                  <div className='mb-3'>
                    <strong>{user.name}</strong>
                    <br />
                    <small className='text-muted'>{user.email}</small>
                  </div>
                  <ul className='list-unstyled mb-0 d-flex flex-column gap-1'>
                    {NAV.map(item => (
                      <li key={item.href}>
                        <Link
                          href={item.href}
                          className='d-flex align-items-center gap-2 p-2 rounded'
                          style={{
                            color: isActive(item.href) ? '#fff' : '#292929',
                            background: isActive(item.href) ? '#7cc000' : 'transparent',
                            fontWeight: 600
                          }}
                        >
                          <i className={`fas ${item.icon}`} /> {item.label}
                        </Link>
                      </li>
                    ))}
                    <li>
                      <button
                        type='button'
                        onClick={() => logout()}
                        className='btn btn-link text-start text-danger p-2 d-flex align-items-center gap-2'
                        style={{ textDecoration: 'none', fontWeight: 600 }}
                      >
                        <i className='fas fa-sign-out-alt' /> Logout
                      </button>
                    </li>
                  </ul>
                </div>
              </div>
              <div className='col-lg-9'>{children}</div>
            </div>
          )}
        </div>
      </section>
    </>
  )
}

export default AccountShell
