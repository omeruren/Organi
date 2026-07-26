'use client'

// React Imports
import { useState } from 'react'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'

const FAQS = [
  {
    q: 'How does delivery work?',
    a: 'We deliver fresh, organic groceries directly from local farms and vendors to your door. Orders placed before 2pm are eligible for next-day delivery in most areas. You can track your order status any time from your account.'
  },
  {
    q: 'Are all products certified organic?',
    a: 'Yes. Every product on Organi is certified organic and sourced from vendors we vet for quality and sustainable growing practices. Look for the “Organic” badge on each product card.'
  },
  {
    q: 'How do I become a vendor?',
    a: 'We love welcoming new growers and makers! Register for an account, then apply through the vendor registration flow. Our team reviews each application to make sure it meets our quality and sustainability standards.'
  },
  {
    q: 'What is your return policy?',
    a: 'If you are not happy with the freshness or quality of any item, contact us within 48 hours of delivery and we will refund or replace it — no questions asked.'
  },
  {
    q: 'How can I track my order?',
    a: 'Once your order is placed, you can follow its status from the Orders section of your account. You will also receive updates as it moves from confirmed to out-for-delivery.'
  },
  {
    q: 'Which payment methods do you accept?',
    a: 'We accept all major credit and debit cards. Your payment details are processed securely and are never stored on our servers.'
  }
]

const FaqView = () => {
  const [open, setOpen] = useState<number | null>(0)

  return (
    <>
      <Breadcrumb title='FAQs' items={[{ label: 'FAQs' }]} />
      <section className='sec_space_large'>
        <div className='container'>
          <div className='text-center mb-5'>
            <h2 style={{ fontWeight: 800 }}>Frequently asked questions</h2>
            <p style={{ color: '#6b6b6b' }}>Everything you need to know about shopping with Organi.</p>
          </div>
          <div className='row justify-content-center'>
            <div className='col-lg-8'>
              <div className='accordion'>
                {FAQS.map((faq, index) => {
                  const isOpen = open === index

                  return (
                    <div key={faq.q} className='bg-white rounded-4 shadow-sm mb-3 overflow-hidden'>
                      <button
                        type='button'
                        className='btn w-100 d-flex justify-content-between align-items-center text-start p-4'
                        style={{ fontWeight: 700, fontSize: 16 }}
                        onClick={() => setOpen(isOpen ? null : index)}
                        aria-expanded={isOpen}
                      >
                        {faq.q}
                        <i
                          className={`fas ${isOpen ? 'fa-minus' : 'fa-plus'}`}
                          style={{ color: '#7cc000', flexShrink: 0, marginLeft: 12 }}
                        />
                      </button>
                      {isOpen && (
                        <div className='px-4 pb-4' style={{ color: '#6b6b6b', lineHeight: 1.8 }}>
                          {faq.a}
                        </div>
                      )}
                    </div>
                  )
                })}
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  )
}

export default FaqView
