// Type Imports
import type { Metadata } from 'next'

// Component Imports
import FaqView from '@/components/store/content/FaqView'

export const metadata: Metadata = {
  title: 'FAQs',
  description: 'Answers to common questions about delivery, organic certification, returns, payment and becoming a vendor.'
}

const FaqPage = () => <FaqView />

export default FaqPage
