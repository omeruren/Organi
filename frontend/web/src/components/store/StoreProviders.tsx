'use client'

// React Imports
import type { ReactNode } from 'react'

// Shared Providers (framework-neutral — no MUI, so the storefront stays MUI-free)
import QueryProvider from '@/libs/QueryProvider'
import { AuthProvider } from '@/contexts/AuthContext'

// The storefront reuses the same React Query client and auth session (BFF /api/auth/*)
// as the admin, but not the MUI theme/Materio providers.
const StoreProviders = ({ children }: { children: ReactNode }) => {
  return (
    <QueryProvider>
      <AuthProvider>{children}</AuthProvider>
    </QueryProvider>
  )
}

export default StoreProviders
