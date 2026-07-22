// Component Imports
import StoreProviders from '@/components/store/StoreProviders'

// Type Imports
import type { ChildrenType } from '@core/types'

// NOTE: The storefront's global CSS (Bootstrap + the ported template stylesheet) and fonts
// will be imported here in B2, scoped to store routes only. For now the placeholder home
// uses inline styles so no global CSS loads on "/".
export const metadata = {
  title: 'Organi — Organic Marketplace',
  description: 'Shop fresh organic produce and groceries from local vendors on Organi.'
}

const StoreLayout = ({ children }: ChildrenType) => {
  return <StoreProviders>{children}</StoreProviders>
}

export default StoreLayout
