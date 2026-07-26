// Next Imports
import type { Metadata } from 'next'

// Component Imports
import AccountShell from '@/components/store/account/AccountShell'

// Type Imports
import type { ChildrenType } from '@core/types'

export const metadata: Metadata = {
  title: 'My Account',
  robots: { index: false, follow: false }
}

const AccountLayout = ({ children }: ChildrenType) => <AccountShell>{children}</AccountShell>

export default AccountLayout
