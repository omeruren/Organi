// Component Imports
import AccountOrderDetail from '@/components/store/account/AccountOrderDetail'

const AccountOrderDetailPage = ({ params }: { params: { id: string } }) => <AccountOrderDetail orderId={params.id} />

export default AccountOrderDetailPage
