// Next Imports
import type { Metadata } from 'next'

// Type Imports
import type { VendorResponse } from '@/types/api/vendor'

// Lib Imports
import { serverFetch } from '@/libs/server-fetch'

// Component Imports
import VendorStoreView from '@/components/store/vendors/VendorStoreView'

export const generateMetadata = async ({ params }: { params: { slug: string } }): Promise<Metadata> => {
  const vendor = await serverFetch<VendorResponse>(`/api/vendors/slug/${params.slug}`)

  if (!vendor) return { title: 'Vendor' }

  const description = vendor.description || `Shop organic products from ${vendor.storeName} on Organi.`

  return {
    title: vendor.storeName,
    description: description.slice(0, 160),
    openGraph: {
      title: vendor.storeName,
      description: description.slice(0, 160),
      images: vendor.bannerUrl || vendor.logoUrl ? [(vendor.bannerUrl ?? vendor.logoUrl) as string] : undefined
    }
  }
}

const VendorStorePage = ({ params }: { params: { slug: string } }) => <VendorStoreView slug={params.slug} />

export default VendorStorePage
