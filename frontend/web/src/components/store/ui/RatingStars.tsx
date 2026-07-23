// Renders the template's 5-star rating (filled fa-star for the rounded value).
const RatingStars = ({ rating, showValue = true }: { rating: number; showValue?: boolean }) => {
  const rounded = Math.round(rating)

  return (
    <div className='rating_wrap d-flex'>
      <ul className='rating_star ul_li'>
        {Array.from({ length: 5 }).map((_, i) => (
          <li key={i} className={i < rounded ? 'active' : ''}>
            <i className={i < rounded ? 'fas fa-star' : 'far fa-star'} />
          </li>
        ))}
      </ul>
      {showValue && <span className='shop_review_text'>{`( ${rating.toFixed(1)} )`}</span>}
    </div>
  )
}

export default RatingStars
