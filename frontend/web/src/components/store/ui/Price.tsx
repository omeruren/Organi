// Sale price + struck-through original, matching the template's `.product_price` markup.
const Price = ({ price, salePrice }: { price: number; salePrice?: number | null }) => {
  const hasSale = salePrice != null && salePrice < price

  return (
    <div className='product_price'>
      <span className='sale_price pe-1'>{`$${(hasSale ? salePrice : price).toFixed(2)}`}</span>
      {hasSale && <del>{`$${price.toFixed(2)}`}</del>}
    </div>
  )
}

export default Price
