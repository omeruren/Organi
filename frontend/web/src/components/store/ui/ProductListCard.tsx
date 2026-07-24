// Next Imports
import Link from 'next/link'

// Component Imports
import RatingStars from '@/components/store/ui/RatingStars'
import Price from '@/components/store/ui/Price'

// Type Imports
import type { ProductSummaryResponse } from '@/types/api/product'

const FALLBACK_IMAGE = '/store/assets/images/product/product1.png'

// Template `.list_layout_wrap` card — the shop "list" view alternative to ProductCard.
const ProductListCard = ({ product }: { product: ProductSummaryResponse }) => {
  const href = `/product/${product.slug}`
  const image = product.primaryImageUrl || FALLBACK_IMAGE
  const hasSale = product.salePrice != null && product.salePrice < product.price
  const discount = hasSale ? Math.round((1 - (product.salePrice as number) / product.price) * 100) : 0

  return (
    <div className='list_layout_wrap overflow-hidden'>
      <div className='list_layout_content bg-white d-flex justify-content-between align-items-center'>
        <div className='col-lg-5'>
          <Link className='list_layout_thumb d-flex justify-content-start' href={href}>
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src={image} alt={product.name} />
          </Link>
        </div>
        <div className='col-lg-7'>
          <ul className='list_layout_bade list-unstyled d-flex'>
            {product.isOrganic && (
              <li>
                <span className='badge_meats rounded-pill text-uppercase'>Organic</span>
              </li>
            )}
            {hasSale && (
              <li>
                <span className='badge_discount text-white rounded-pill'>{`-${discount}%`}</span>
              </li>
            )}
          </ul>
          <RatingStars rating={product.averageRating} />
          <div className='product_content'>
            <h3 className='product_title py-2'>
              <Link href={href}>{product.name}</Link>
            </h3>
            <Price price={product.price} salePrice={product.salePrice} />
            <p className='list_layout_desc'>
              Fresh, organic {product.categoryName.toLowerCase()} from {product.vendorName}. Sold per {product.unit}.
            </p>
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
        </div>
      </div>
    </div>
  )
}

export default ProductListCard
