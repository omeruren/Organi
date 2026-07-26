// React Imports
import { Suspense } from 'react'

// Next Imports
import type { Metadata } from 'next'

// Component Imports
import ForgotPasswordView from '@/components/store/auth/ForgotPasswordView'

export const metadata: Metadata = {
  title: 'Forgot Password',
  robots: { index: false, follow: true }
}

const ForgotPasswordPage = () => (
  <Suspense>
    <ForgotPasswordView />
  </Suspense>
)

export default ForgotPasswordPage
