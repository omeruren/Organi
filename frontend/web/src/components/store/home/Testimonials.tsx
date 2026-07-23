// Component Imports
import SectionTitle from '@/components/store/ui/SectionTitle'

const REVIEWS = [
  {
    name: 'Sarah Johnson',
    role: 'Home Cook',
    avatar: '/store/assets/images/testimonials/test-1.jpg',
    text: 'The produce is always fresh and arrives fast. Organi has completely changed how I shop for groceries.'
  },
  {
    name: 'Michael Lee',
    role: 'Chef',
    avatar: '/store/assets/images/testimonials/test-2.jpg',
    text: 'Great selection of organic ingredients from local vendors. Quality I can rely on for my restaurant.'
  },
  {
    name: 'Emma Davis',
    role: 'Nutritionist',
    avatar: '/store/assets/images/testimonials/test-3.jpg',
    text: 'I love that everything is clearly labeled organic and sourced responsibly. Highly recommend Organi.'
  }
]

const card: React.CSSProperties = {
  background: '#fff',
  borderRadius: 20,
  padding: '28px 26px',
  boxShadow: '0 6px 20px rgba(0,0,0,.06)',
  height: '100%'
}

const Testimonials = () => {
  return (
    <section className='sec_space_xxs_50'>
      <div className='container'>
        <div className='text-center mb-5 d-flex flex-column align-items-center'>
          <SectionTitle eyebrow='WHAT CUSTOMERS SAY' title='Loved by Thousands' />
        </div>
        <div className='row g-4'>
          {REVIEWS.map(review => (
            <div key={review.name} className='col-md-4'>
              <div style={card}>
                <div className='mb-3' style={{ color: '#7cc000' }}>
                  {Array.from({ length: 5 }).map((_, i) => (
                    <i key={i} className='fas fa-star' style={{ marginRight: 2 }} />
                  ))}
                </div>
                <p style={{ color: '#4a4a4a' }}>&ldquo;{review.text}&rdquo;</p>
                <div className='d-flex align-items-center gap-3 mt-4'>
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img
                    src={review.avatar}
                    alt={review.name}
                    width={52}
                    height={52}
                    style={{ borderRadius: '50%', objectFit: 'cover' }}
                  />
                  <div>
                    <h6 className='mb-0' style={{ fontWeight: 800 }}>
                      {review.name}
                    </h6>
                    <span style={{ color: '#6b6b6b', fontSize: 14 }}>{review.role}</span>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

export default Testimonials
