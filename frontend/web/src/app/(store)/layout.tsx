// Next Imports
import type { Metadata } from 'next'

// Component Imports
import StoreProviders from '@/components/store/StoreProviders'
import StoreHeader from '@/components/store/layout/StoreHeader'
import StoreFooter from '@/components/store/layout/StoreFooter'

// Type Imports
import type { ChildrenType } from '@core/types'

const siteUrl = process.env.NEXT_PUBLIC_SITE_URL ?? 'http://localhost:3000'

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl),
  title: {
    default: 'Organi — Organic Marketplace',
    template: '%s — Organi'
  },
  description: 'Shop fresh organic produce and groceries from local vendors on Organi.',
  openGraph: {
    type: 'website',
    siteName: 'Organi',
    title: 'Organi — Organic Marketplace',
    description: 'Shop fresh organic produce and groceries from local vendors on Organi.'
  }
}

// The storefront's global CSS is loaded here via <link> (React 18 hoists these to <head>),
// scoped to store routes only — so the admin's MUI/Tailwind never mixes with Bootstrap.
// Serving the template CSS from /public keeps its url(../images/…) background paths intact.
const StoreLayout = ({ children }: ChildrenType) => {
  return (
    <>
      {/* Warm up the cross-origin font/icon hosts early so the render-blocking stylesheets and
          webfonts below resolve their DNS/TLS handshake in parallel with HTML parsing. */}
      <link rel='preconnect' href='https://fonts.googleapis.com' />
      <link rel='preconnect' href='https://fonts.gstatic.com' crossOrigin='anonymous' />
      <link rel='preconnect' href='https://cdnjs.cloudflare.com' />
      {/* Intentional <link> loading (not JS import) so the template's url(../images/…) paths
          resolve from /public and the CSS stays scoped to store routes. */}
      {/* eslint-disable-next-line @next/next/no-css-tags */}
      <link rel='stylesheet' href='/store/assets/css/bootstrap.min.css' />
      {/* eslint-disable-next-line @next/next/no-css-tags */}
      <link rel='stylesheet' href='/store/assets/css/style.css' />
      {/* Theme tokens load last so they can redefine both Bootstrap and the template. */}
      {/* eslint-disable-next-line @next/next/no-css-tags */}
      <link rel='stylesheet' href='/store/assets/css/theme.css' />
      {/* Applies the stored/OS theme before first paint. Without this the page renders
          light and then snaps to dark, which is the classic dark-mode flash. It has to be
          inline and synchronous — a deferred script runs too late to prevent it. */}
      <script
        dangerouslySetInnerHTML={{
          __html: `(function(){try{var t=localStorage.getItem('organi-theme');if(t!=='light'&&t!=='dark'){t=window.matchMedia('(prefers-color-scheme: dark)').matches?'dark':'light'}document.documentElement.setAttribute('data-theme',t)}catch(e){}})()`
        }}
      />
      <link
        rel='stylesheet'
        href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css'
      />
      <link
        rel='stylesheet'
        href='https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700;800;900&family=Roboto:wght@400;500;700;900&display=swap'
      />
      <StoreProviders>
        <div className='body-wrap overflow-hidden'>
          <StoreHeader />
          <main>{children}</main>
          <StoreFooter />
        </div>
      </StoreProviders>
    </>
  )
}

export default StoreLayout
