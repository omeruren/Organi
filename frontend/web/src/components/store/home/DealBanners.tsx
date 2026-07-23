// Next Imports
import Link from 'next/link'

const DEALS = [
  { title: 'Get Every Vegetable You Need', image: '/store/assets/images/deals/deals1.png' },
  { title: 'Fresh Fruits Every Morning', image: '/store/assets/images/deals/deals2.png' },
  { title: 'Daily Groceries & Staples', image: '/store/assets/images/deals/deals3.png' }
]

const DealBanners = () => {
  return (
    <section className='deal_section sec_space_xs_70'>
      <div className='container-fluid'>
        <div className='row justify-content-center align-items-center g-4'>
          {DEALS.map(deal => (
            <div key={deal.title} className='col-md-6 col-lg-4'>
              <div className='deal_item_content_wrap'>
                <div className='deal_item_content position-relative' style={{ backgroundImage: `url(${deal.image})` }}>
                  <div className='deal_item_txt position-absolute'>
                    <div className='deal_title'>
                      <h3>{deal.title}</h3>
                    </div>
                    <div className='deal_btn'>
                      <Link href='/shop'>
                        <button type='button' className='btn'>
                          SHOP NOW{' '}
                          <span>
                            <i className='fas fa-long-arrow-alt-right' />
                          </span>
                        </button>
                      </Link>
                    </div>
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

export default DealBanners
