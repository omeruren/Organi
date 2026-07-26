// React Imports
import { Suspense } from 'react'

// Next Imports
import type { Metadata } from 'next'

// Component Imports
import ConfirmEmailView from '@/components/store/auth/ConfirmEmailView'

export const metadata: Metadata = {
  title: 'Confirm Email',
  robots: { index: false, follow: true }
}

const ConfirmEmailPage = () => (
  <Suspense>
    <ConfirmEmailView />
  </Suspense>
)

export default ConfirmEmailPage
