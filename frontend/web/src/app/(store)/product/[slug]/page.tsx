// Component Imports
import ProductDetailView from '@/components/store/product/ProductDetailView'

const ProductDetailPage = ({ params }: { params: { slug: string } }) => <ProductDetailView slug={params.slug} />

export default ProductDetailPage
