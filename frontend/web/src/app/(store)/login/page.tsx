// React Imports
import { Suspense } from 'react'

// Type Imports
import type { Metadata } from 'next'

// Component Imports
import StoreLoginView from '@/components/store/auth/StoreLoginView'

export const metadata: Metadata = {
  title: 'Login',
  robots: { index: false, follow: true }
}

const LoginPage = () => (
  <Suspense fallback={<div className='container py-5 text-center'>Loading…</div>}>
    <StoreLoginView />
  </Suspense>
)

export default LoginPage
