// Next Imports
import type { Metadata } from 'next'

// Type Imports
import type { ProductResponse } from '@/types/api/product'

// Lib Imports
import { serverFetch } from '@/libs/server-fetch'

// Component Imports
import ProductDetailView from '@/components/store/product/ProductDetailView'

export const generateMetadata = async ({ params }: { params: { slug: string } }): Promise<Metadata> => {
  const product = await serverFetch<ProductResponse>(`/api/products/slug/${params.slug}`)

  if (!product) return { title: 'Product' }

  const description = product.shortDescription || product.description || `Buy ${product.name} — fresh and organic on Organi.`

  return {
    title: product.name,
    description: description.slice(0, 160),
    openGraph: {
      title: product.name,
      description: description.slice(0, 160),
      images: product.primaryImageUrl ? [product.primaryImageUrl] : undefined
    }
  }
}

const ProductDetailPage = ({ params }: { params: { slug: string } }) => <ProductDetailView slug={params.slug} />

export default ProductDetailPage
