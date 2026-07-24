// React Imports
import { Suspense } from 'react'

// Component Imports
import ShopView from '@/components/store/shop/ShopView'

// ShopView reads useSearchParams, which requires a Suspense boundary in the App Router.
const ShopPage = () => {
  return (
    <Suspense fallback={<div className='container py-5 text-center'>Loading shop…</div>}>
      <ShopView />
    </Suspense>
  )
}

export default ShopPage
