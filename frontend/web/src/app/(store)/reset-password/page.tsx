// React Imports
import { Suspense } from 'react'

// Next Imports
import type { Metadata } from 'next'

// Component Imports
import ResetPasswordView from '@/components/store/auth/ResetPasswordView'

export const metadata: Metadata = {
  title: 'Reset Password',
  robots: { index: false, follow: true }
}

// ResetPasswordView reads useSearchParams, which requires a Suspense boundary in the App Router.
const ResetPasswordPage = () => (
  <Suspense>
    <ResetPasswordView />
  </Suspense>
)

export default ResetPasswordPage
