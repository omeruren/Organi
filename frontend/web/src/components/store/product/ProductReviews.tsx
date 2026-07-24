'use client'

// Next Imports
import Link from 'next/link'

// Hook Imports
import { useProductReviews } from '@/hooks/api/useProductReviews'

// Component Imports
import RatingStars from '@/components/store/ui/RatingStars'

const ProductReviews = ({ productId }: { productId: string }) => {
  const { data, isLoading } = useProductReviews(productId)
  const reviews = data?.items ?? []

  return (
    <div className='review_comment2'>
      <h3 className='title_text'>{`Reviews${data ? ` (${data.totalCount})` : ''}`}</h3>

      {isLoading ? (
        <p>Loading reviews…</p>
      ) : reviews.length === 0 ? (
        <p style={{ color: '#6b6b6b' }}>No reviews yet. Be the first to review this product.</p>
      ) : (
        <ul className='review_comment_list2 ul_li_block'>
          {reviews.map(review => (
            <li key={review.id} className='review_comment_wrap2'>
              <h4 className='admin_name'>
                {review.userFullName}{' '}
                <span className='comment_date'>{new Date(review.createdAt).toLocaleDateString()}</span>
              </h4>
              <RatingStars rating={review.rating} showValue={false} />
              {review.title && <strong className='d-block mt-1'>{review.title}</strong>}
              <p className='mb-0'>{review.comment}</p>
            </li>
          ))}
        </ul>
      )}

      {/* Writing a review requires a signed-in customer who purchased the product — wired in B5. */}
      <div className='comment_form_area mt-4'>
        <p className='mb-0' style={{ color: '#6b6b6b' }}>
          <Link href='/login' className='text-primary'>
            Log in
          </Link>{' '}
          to write a review. Only verified purchasers can review a product.
        </p>
      </div>
    </div>
  )
}

export default ProductReviews
