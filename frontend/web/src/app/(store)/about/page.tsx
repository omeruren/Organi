// Type Imports
import type { Metadata } from 'next'

// Component Imports
import AboutView from '@/components/store/content/AboutView'

export const metadata: Metadata = {
  title: 'About Us',
  description: 'Organi connects you with local farms and trusted vendors for fresh, certified-organic food delivered to your door.'
}

const AboutPage = () => <AboutView />

export default AboutPage
