'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import { useRouter } from 'next/navigation'

const HEALTHY = 'Healthy'.split('')

const HeroBanner = () => {
  const router = useRouter()
  const [search, setSearch] = useState('')

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    router.push(search.trim() ? `/shop?search=${encodeURIComponent(search.trim())}` : '/shop')
  }

  return (
    <section className='banner_section_main position-relative'>
      <div
        className='banner_section_item sec_space_xxxlarge d-flex justify-content-center align-items-center'
        style={{ backgroundImage: 'url(/store/assets/images/banner/banner1.png)' }}
      >
        <div className='container'>
          <div className='row text-center'>
            <div className='col banner_content'>
              <div className='banner_sub_title position-relative'>
                <h6 className='position-absolute text-white text-uppercase'>100% Organic Foods</h6>
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img className='sub_title_bg' src='/store/assets/images/shapes/shape2.png' alt='' />
              </div>
              <div className='banner_title m-auto'>
                <h1>
                  Organic <span className='first'>Veggies &amp; Foods</span> You Cook{' '}
                  <span className='text-effect'>
                    {HEALTHY.map((ch, i) => (
                      <span key={i}>{ch}</span>
                    ))}
                  </span>
                </h1>
              </div>
              <form className='banner_search_bar d-inline-block position-relative' onSubmit={onSubmit}>
                <input
                  className='d-inline-block rounded-pill border-0 shadow py-3 px-5'
                  type='search'
                  placeholder='Search Product'
                  value={search}
                  onChange={e => setSearch(e.target.value)}
                />
              </form>
            </div>
          </div>
        </div>
      </div>
      {/* eslint-disable-next-line @next/next/no-img-element */}
      <img className='banner_right_thumb img_moving_anim1 position-absolute' src='/store/assets/images/banner/banner3.png' alt='' />
      {/* eslint-disable-next-line @next/next/no-img-element */}
      <img className='banner_left_thumb img_moving_anim2 position-absolute' src='/store/assets/images/banner/banner4.png' alt='' />
    </section>
  )
}

export default HeroBanner
