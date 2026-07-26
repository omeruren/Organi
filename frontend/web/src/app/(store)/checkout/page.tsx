// Type Imports
import type { Metadata } from 'next'

// Component Imports
import CheckoutView from '@/components/store/checkout/CheckoutView'

export const metadata: Metadata = {
  title: 'Checkout',
  robots: { index: false, follow: true }
}

const CheckoutPage = () => <CheckoutView />

export default CheckoutPage
