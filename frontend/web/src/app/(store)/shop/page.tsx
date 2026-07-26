// React Imports
import { Suspense } from 'react'

// Type Imports
import type { Metadata } from 'next'

// Component Imports
import ShopView from '@/components/store/shop/ShopView'

export const metadata: Metadata = {
  title: 'Shop',
  description: 'Browse fresh organic produce, groceries and pantry staples from trusted local vendors.'
}

// ShopView reads useSearchParams, which requires a Suspense boundary in the App Router.
const ShopPage = () => {
  return (
    <Suspense fallback={<div className='container py-5 text-center'>Loading shop…</div>}>
      <ShopView />
    </Suspense>
  )
}

export default ShopPage
