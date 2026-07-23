// Next Imports
import Link from 'next/link'

// Component Imports
import RatingStars from '@/components/store/ui/RatingStars'
import Price from '@/components/store/ui/Price'

// Type Imports
import type { ProductSummaryResponse } from '@/types/api/product'

const FALLBACK_IMAGE = '/store/assets/images/product/product1.png'

// Faithful port of the template's `.product_layout_1` card, wired to live product data.
const ProductCard = ({ product }: { product: ProductSummaryResponse }) => {
  const href = `/product/${product.slug}`
  const image = product.primaryImageUrl || FALLBACK_IMAGE
  const hasSale = product.salePrice != null && product.salePrice < product.price
  const discount = hasSale ? Math.round((1 - (product.salePrice as number) / product.price) * 100) : 0

  return (
    <div className='product_layout_1 overflow-hidden position-relative'>
      <div className='product_layout_content bg-white position-relative'>
        <div className='product_image_wrap'>
          <Link className='product_image d-flex justify-content-center align-items-center' href={href}>
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img className='pic-1' src={image} alt={product.name} />
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img className='pic-2' src={image} alt={product.name} />
          </Link>
          <ul className='product_badge_group ul_li_block'>
            {product.isOrganic && (
              <li>
                <span className='product_badge badge_meats position-absolute rounded-pill text-uppercase'>Organic</span>
              </li>
            )}
            {hasSale && (
              <li>
                <span className='product_badge badge_discount position-absolute rounded-pill'>{`-${discount}%`}</span>
              </li>
            )}
          </ul>
          <ul className='product_action_btns ul_li_block d-flex'>
            <li>
              <Link className='tooltips' title='View Product' href={href}>
                <i className='fas fa-search' />
              </Link>
            </li>
            <li>
              <Link className='tooltips' title='View Product' href={href}>
                <i className='fas fa-shopping-bag' />
              </Link>
            </li>
          </ul>
        </div>
        <RatingStars rating={product.averageRating} />
        <div className='product_content'>
          <h3 className='product_title'>
            <Link href={href}>{product.name}</Link>
          </h3>
          <Price price={product.price} salePrice={product.salePrice} />
        </div>
      </div>
    </div>
  )
}

export default ProductCard
