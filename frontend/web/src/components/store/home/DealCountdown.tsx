'use client'

// Next Imports
import Link from 'next/link'

// Hook Imports
import { useCountdown } from '@/hooks/useCountdown'

// A future target for the "deal of the week" timer (7 days out from first render).
const TARGET = Date.now() + 7 * 24 * 60 * 60 * 1000

const box: React.CSSProperties = {
  minWidth: 68,
  padding: '12px 8px',
  background: '#fff',
  borderRadius: 12,
  textAlign: 'center',
  boxShadow: '0 4px 14px rgba(0,0,0,.08)'
}

const DealCountdown = () => {
  const { days, hours, minutes, seconds } = useCountdown(TARGET)

  const units: [string, number][] = [
    ['Days', days],
    ['Hours', hours],
    ['Mins', minutes],
    ['Secs', seconds]
  ]

  return (
    <section className='sec_space_xs_70'>
      <div className='container'>
        <div
          className='row align-items-center g-4 p-4 p-lg-5'
          style={{ background: 'linear-gradient(90deg,#f3f9e8,#eef6dd)', borderRadius: 24 }}
        >
          <div className='col-lg-7'>
            <h6 className='text-uppercase' style={{ color: '#7cc000', fontWeight: 700, letterSpacing: 1 }}>
              Deal of the Week
            </h6>
            <h2 style={{ fontWeight: 900, color: '#292929' }}>Save up to 30% on fresh organic bundles</h2>
            <div className='d-flex gap-3 my-4'>
              {units.map(([label, value]) => (
                <div key={label} style={box}>
                  <div style={{ fontSize: 26, fontWeight: 900, color: '#292929', lineHeight: 1 }}>
                    {String(value).padStart(2, '0')}
                  </div>
                  <div style={{ fontSize: 12, color: '#6b6b6b', marginTop: 4 }}>{label}</div>
                </div>
              ))}
            </div>
            <Link href='/shop'>
              <button type='button' className='btn custom_btn rounded-pill px-4 text-white'>
                Shop the Deal <i className='fas fa-long-arrow-alt-right' />
              </button>
            </Link>
          </div>
          <div className='col-lg-5 text-center'>
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src='/store/assets/images/offer/offer1.png' alt='Weekly deal' className='img-fluid' style={{ maxHeight: 300 }} />
          </div>
        </div>
      </div>
    </section>
  )
}

export default DealCountdown
