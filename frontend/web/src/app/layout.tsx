// Type Imports
import type { ChildrenType } from '@core/types'

// NOTE: This root layout is intentionally free of CSS and providers. The admin surface
// (/admin/*) and the customer storefront (/) each own their own global CSS, fonts, and
// provider tree in their route-group layouts, so MUI/Materio (admin) and Bootstrap (store)
// never load on the same page. The body's Tailwind utility classes are inert on store
// routes (globals.css isn't loaded there) and active under /admin where globals.css loads.
export const metadata = {
  title: 'Organi',
  description: 'Organi — the organic marketplace.'
}

// suppressHydrationWarning: the storefront's anti-flash script stamps data-theme onto <html>
// before React hydrates, so the client tree legitimately differs from the server tree by that
// one attribute. The suppression applies to this element's attributes only, not its subtree.
const RootLayout = ({ children }: ChildrenType) => {
  return (
    <html id='__next' lang='en' dir='ltr' suppressHydrationWarning>
      <body className='flex is-full min-bs-full flex-auto flex-col'>{children}</body>
    </html>
  )
}

export default RootLayout
