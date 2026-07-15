// Data-driven admin nav — mirrors backend authorization policies exactly, see
// ai/09-AI-Frontend-Skills.md §10 for the policy → section mapping table.
export type AppRole = 'Admin' | 'Vendor'

export interface NavItem {
  label: string
  href: string
  icon: string
  roles: AppRole[]
}

export interface NavSection {
  label: string
  items: NavItem[]
}

export const navigationConfig: NavSection[] = [
  {
    label: 'General',
    items: [{ label: 'Dashboard', href: '/', icon: 'ri-home-smile-line', roles: ['Admin', 'Vendor'] }]
  },
  {
    label: 'Catalog',
    items: [
      { label: 'Products', href: '/products', icon: 'ri-shopping-bag-line', roles: ['Admin', 'Vendor'] },
      { label: 'Categories', href: '/categories', icon: 'ri-price-tag-3-line', roles: ['Admin'] },
      { label: 'Reviews', href: '/reviews', icon: 'ri-star-line', roles: ['Admin'] }
    ]
  },
  {
    label: 'Sales',
    items: [
      { label: 'Orders', href: '/orders', icon: 'ri-shopping-cart-line', roles: ['Admin', 'Vendor'] },
      { label: 'Coupons', href: '/coupons', icon: 'ri-coupon-line', roles: ['Admin'] }
    ]
  },
  {
    label: 'Marketplace',
    items: [
      { label: 'Vendors', href: '/vendors', icon: 'ri-store-line', roles: ['Admin'] },
      { label: 'Users', href: '/users', icon: 'ri-user-line', roles: ['Admin'] }
    ]
  },
  {
    label: 'Content',
    items: [
      { label: 'Blog', href: '/blog', icon: 'ri-article-line', roles: ['Admin', 'Vendor'] },
      { label: 'Newsletter', href: '/newsletter', icon: 'ri-mail-line', roles: ['Admin'] }
    ]
  },
  {
    label: 'System',
    items: [
      { label: 'Reports', href: '/reports', icon: 'ri-bar-chart-line', roles: ['Admin'] },
      { label: 'Audit Logs', href: '/audit-logs', icon: 'ri-history-line', roles: ['Admin'] }
    ]
  }
]
