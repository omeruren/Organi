'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'

// Hook / Context Imports
import { useProductReviews, useCreateProductReview } from '@/hooks/api/useProductReviews'
import { useAuth } from '@/contexts/AuthContext'
import { useStoreToast } from '@/components/store/StoreToast'
import { ApiError } from '@/libs/api-client'

// Component Imports
import RatingStars from '@/components/store/ui/RatingStars'

const ReviewForm = ({ productId }: { productId: string }) => {
  const create = useCreateProductReview(productId)
  const { showToast } = useStoreToast()

  const [rating, setRating] = useState(5)
  const [hover, setHover] = useState(0)
  const [title, setTitle] = useState('')
  const [comment, setComment] = useState('')

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!comment.trim()) {
      showToast('Please write a comment.', 'error')

      return
    }

    try {
      await create.mutateAsync({ rating, title: title.trim() || null, comment: comment.trim() })
      showToast('Thanks — your review was submitted.')
      setTitle('')
      setComment('')
      setRating(5)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Could not submit your review.', 'error')
    }
  }

  return (
    <div className='comment_form_area mt-4'>
      <h4 className='comment_title'>Write a Review</h4>
      <form onSubmit={onSubmit} className='d-flex flex-column gap-3'>
        <div className='d-flex align-items-center gap-1' style={{ fontSize: 22, color: '#f0ad4e' }}>
          {[1, 2, 3, 4, 5].map(star => (
            <button
              key={star}
              type='button'
              className='btn p-0 border-0'
              style={{ color: '#f0ad4e', lineHeight: 1 }}
              onMouseEnter={() => setHover(star)}
              onMouseLeave={() => setHover(0)}
              onClick={() => setRating(star)}
              aria-label={`${star} star`}
            >
              <i className={`fa${(hover || rating) >= star ? 's' : 'r'} fa-star`} />
            </button>
          ))}
        </div>
        <label className='form-label mb-0' htmlFor='review-title'>
          Review title<span style={{ color: '#6b6b6b', fontWeight: 400 }}> (optional)</span>
        </label>
        <input
          id='review-title'
          className='form-control rounded-pill py-2'
          value={title}
          onChange={e => setTitle(e.target.value)}
          maxLength={200}
        />
        <label className='form-label mb-0' htmlFor='review-comment'>
          Your review
        </label>
        <textarea
          id='review-comment'
          className='form-control rounded-4 p-3'
          rows={3}
          value={comment}
          onChange={e => setComment(e.target.value)}
          maxLength={1000}
        />
        <button type='submit' className='btn custom_btn rounded-pill py-2 px-4 align-self-start' disabled={create.isPending}>
          {create.isPending ? 'Submitting…' : 'Post Review'}
        </button>
      </form>
    </div>
  )
}

const ProductReviews = ({ productId }: { productId: string }) => {
  const { data, isLoading } = useProductReviews(productId)
  const { user } = useAuth()
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

      {user ? (
        <ReviewForm productId={productId} />
      ) : (
        <div className='comment_form_area mt-4'>
          <p className='mb-0' style={{ color: '#6b6b6b' }}>
            <Link href='/login' className='text-primary'>
              Log in
            </Link>{' '}
            to write a review. Only verified purchasers can review a product.
          </p>
        </div>
      )}
    </div>
  )
}

export default ProductReviews
