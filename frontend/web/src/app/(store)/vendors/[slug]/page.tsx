// Component Imports
import VendorStoreView from '@/components/store/vendors/VendorStoreView'

const VendorStorePage = ({ params }: { params: { slug: string } }) => <VendorStoreView slug={params.slug} />

export default VendorStorePage
