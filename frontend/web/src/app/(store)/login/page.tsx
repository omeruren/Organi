// React Imports
import { Suspense } from 'react'

// Component Imports
import StoreLoginView from '@/components/store/auth/StoreLoginView'

const LoginPage = () => (
  <Suspense fallback={<div className='container py-5 text-center'>Loading…</div>}>
    <StoreLoginView />
  </Suspense>
)

export default LoginPage
