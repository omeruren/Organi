// Component Imports
import HeroBanner from '@/components/store/home/HeroBanner'
import CategoryStrip from '@/components/store/home/CategoryStrip'
import ProductTabs from '@/components/store/home/ProductTabs'
import DealBanners from '@/components/store/home/DealBanners'
import DealCountdown from '@/components/store/home/DealCountdown'
import Testimonials from '@/components/store/home/Testimonials'

const HomePage = () => {
  return (
    <>
      <HeroBanner />
      <CategoryStrip />
      <ProductTabs />
      <DealBanners />
      <DealCountdown />
      <Testimonials />
    </>
  )
}

export default HomePage
