// View Imports
import OrderDetail from '@views/orders/OrderDetail'

const OrderDetailPage = ({ params }: { params: { id: string } }) => <OrderDetail orderId={params.id} />

export default OrderDetailPage
