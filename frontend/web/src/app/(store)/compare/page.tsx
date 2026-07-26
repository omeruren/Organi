// Type Imports
import type { Metadata } from 'next'

// Component Imports
import CompareView from '@/components/store/compare/CompareView'

export const metadata: Metadata = {
  title: 'Compare Products',
  robots: { index: false, follow: true }
}

const ComparePage = () => <CompareView />

export default ComparePage
