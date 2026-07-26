// Type Imports
import type { Metadata } from 'next'

// Component Imports
import CartView from '@/components/store/cart/CartView'

export const metadata: Metadata = {
  title: 'Shopping Cart',
  robots: { index: false, follow: true }
}

const CartPage = () => <CartView />

export default CartPage
