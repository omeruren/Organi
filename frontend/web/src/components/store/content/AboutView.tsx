// Next Imports
import Link from 'next/link'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import SectionTitle from '@/components/store/ui/SectionTitle'

const VALUES = [
  { icon: 'fas fa-leaf', title: '100% Organic', text: 'Every product is certified organic, grown without synthetic pesticides or fertilizers.' },
  { icon: 'fas fa-truck', title: 'Fresh Delivery', text: 'From local farms straight to your door — harvested and delivered at peak freshness.' },
  { icon: 'fas fa-hand-holding-heart', title: 'Trusted Vendors', text: 'We partner only with growers and makers who share our commitment to quality.' },
  { icon: 'fas fa-recycle', title: 'Sustainable', text: 'Eco-friendly packaging and responsible sourcing at every step of the journey.' }
]

const TEAM = [
  { name: 'Sarah Mitchell', role: 'Founder & CEO', image: '/store/assets/images/team/team3.png' },
  { name: 'David Chen', role: 'Head of Sourcing', image: '/store/assets/images/team/team4.png' },
  { name: 'Emma Rodriguez', role: 'Quality Lead', image: '/store/assets/images/team/team5.png' },
  { name: 'James Okafor', role: 'Vendor Relations', image: '/store/assets/images/team/team6.png' }
]

const AboutView = () => {
  return (
    <>
      <Breadcrumb title='About Us' items={[{ label: 'About' }]} />

      {/* Intro */}
      <section className='sec_space_large'>
        <div className='container'>
          <div className='row align-items-center g-5'>
            <div className='col-lg-6'>
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img
                src='/store/assets/images/quality/qlty1.png'
                alt='Organi'
                className='w-100 rounded-4'
                style={{ objectFit: 'cover' }}
              />
            </div>
            <div className='col-lg-6'>
              <SectionTitle eyebrow='Who We Are' title='Fresh, organic food for a healthier life' />
              <p style={{ color: '#6b6b6b', lineHeight: 1.9 }}>
                Organi started with a simple belief: everyone deserves access to fresh, honest food. We connect you
                directly with local farms and trusted vendors, bringing organic produce and groceries from the soil to
                your table with nothing artificial in between.
              </p>
              <p style={{ color: '#6b6b6b', lineHeight: 1.9 }}>
                Today we host hundreds of independent vendors, each carefully vetted for quality and sustainability — so
                every order supports growers who farm the right way.
              </p>
              <Link href='/shop' className='btn custom_btn rounded-pill px-4 text-white mt-2'>
                Start Shopping <i className='fas fa-long-arrow-alt-right ms-1' />
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* Values */}
      <section className='sec_space_mid_small' style={{ background: '#f7f8f3' }}>
        <div className='container'>
          <div className='text-center mb-5'>
            <h2 style={{ fontWeight: 800 }}>Why choose Organi</h2>
            <p style={{ color: '#6b6b6b' }}>The principles that guide everything we do.</p>
          </div>
          <div className='row g-4'>
            {VALUES.map(value => (
              <div key={value.title} className='col-sm-6 col-lg-3'>
                <div className='bg-white rounded-4 shadow-sm p-4 h-100 text-center'>
                  <div
                    className='d-inline-flex align-items-center justify-content-center rounded-circle mb-3 text-white'
                    style={{ width: 64, height: 64, background: '#7cc000', fontSize: 24 }}
                  >
                    <i className={value.icon} />
                  </div>
                  <h5 style={{ fontWeight: 700 }}>{value.title}</h5>
                  <p className='mb-0' style={{ color: '#6b6b6b', fontSize: 14 }}>
                    {value.text}
                  </p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Team */}
      <section className='sec_space_large'>
        <div className='container'>
          <div className='text-center mb-5'>
            <h2 style={{ fontWeight: 800 }}>Meet the team</h2>
            <p style={{ color: '#6b6b6b' }}>The people behind your daily fresh.</p>
          </div>
          <div className='row g-4'>
            {TEAM.map(member => (
              <div key={member.name} className='col-sm-6 col-lg-3'>
                <div className='bg-white rounded-4 shadow-sm overflow-hidden h-100 text-center'>
                  <div style={{ height: 220, overflow: 'hidden' }}>
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img src={member.image} alt={member.name} className='w-100 h-100' style={{ objectFit: 'cover' }} />
                  </div>
                  <div className='p-3'>
                    <h5 className='mb-0' style={{ fontWeight: 700 }}>
                      {member.name}
                    </h5>
                    <p className='mb-0' style={{ color: '#7cc000', fontSize: 14 }}>
                      {member.role}
                    </p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>
    </>
  )
}

export default AboutView
