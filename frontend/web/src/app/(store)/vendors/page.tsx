// React Imports
import { Suspense } from 'react'

// Type Imports
import type { Metadata } from 'next'

// Component Imports
import VendorsView from '@/components/store/vendors/VendorsView'

export const metadata: Metadata = {
  title: 'Vendors',
  description: 'Discover trusted organic vendors and local farms selling fresh produce and groceries on Organi.'
}

const VendorsPage = () => (
  <Suspense>
    <VendorsView />
  </Suspense>
)

export default VendorsPage
