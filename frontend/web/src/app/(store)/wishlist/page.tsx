// Type Imports
import type { Metadata } from 'next'

// Component Imports
import WishlistView from '@/components/store/wishlist/WishlistView'

export const metadata: Metadata = {
  title: 'Wishlist',
  robots: { index: false, follow: true }
}

const WishlistPage = () => <WishlistView />

export default WishlistPage
