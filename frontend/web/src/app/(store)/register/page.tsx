// Type Imports
import type { Metadata } from 'next'

// Component Imports
import StoreRegisterView from '@/components/store/auth/StoreRegisterView'

export const metadata: Metadata = {
  title: 'Create Account',
  robots: { index: false, follow: true }
}

const RegisterPage = () => <StoreRegisterView />

export default RegisterPage
