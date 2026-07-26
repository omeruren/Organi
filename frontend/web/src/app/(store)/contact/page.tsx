// Type Imports
import type { Metadata } from 'next'

// Component Imports
import ContactView from '@/components/store/content/ContactView'

export const metadata: Metadata = {
  title: 'Contact Us',
  description: 'Get in touch with the Organi team — questions about orders, products or becoming a vendor.'
}

const ContactPage = () => <ContactView />

export default ContactPage
