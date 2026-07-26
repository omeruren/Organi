// React Imports
import { Suspense } from 'react'

// Component Imports
import VendorsView from '@/components/store/vendors/VendorsView'

const VendorsPage = () => (
  <Suspense>
    <VendorsView />
  </Suspense>
)

export default VendorsPage
