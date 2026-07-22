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
    items: [{ label: 'Dashboard', href: '/admin', icon: 'ri-home-smile-line', roles: ['Admin', 'Vendor'] }]
  },
  {
    label: 'Catalog',
    items: [
      { label: 'Products', href: '/admin/products', icon: 'ri-shopping-bag-line', roles: ['Admin', 'Vendor'] },
      { label: 'Categories', href: '/admin/categories', icon: 'ri-price-tag-3-line', roles: ['Admin'] },
      { label: 'Reviews', href: '/admin/reviews', icon: 'ri-star-line', roles: ['Admin'] }
    ]
  },
  {
    label: 'Sales',
    items: [
      { label: 'Orders', href: '/admin/orders', icon: 'ri-shopping-cart-line', roles: ['Admin', 'Vendor'] },
      { label: 'Coupons', href: '/admin/coupons', icon: 'ri-coupon-line', roles: ['Admin'] }
    ]
  },
  {
    label: 'Marketplace',
    items: [
      { label: 'Vendors', href: '/admin/vendors', icon: 'ri-store-line', roles: ['Admin'] },
      { label: 'Users', href: '/admin/users', icon: 'ri-user-line', roles: ['Admin'] }
    ]
  },
  {
    label: 'Content',
    items: [
      { label: 'Blog', href: '/admin/blog', icon: 'ri-article-line', roles: ['Admin', 'Vendor'] },
      { label: 'Newsletter', href: '/admin/newsletter', icon: 'ri-mail-line', roles: ['Admin'] }
    ]
  },
  {
    label: 'System',
    items: [
      { label: 'Reports', href: '/admin/reports', icon: 'ri-bar-chart-line', roles: ['Admin'] },
      { label: 'Audit Logs', href: '/admin/audit-logs', icon: 'ri-history-line', roles: ['Admin'] }
    ]
  }
]
