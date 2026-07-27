'use client'

// React Imports
import { useState } from 'react'

// Third-party Imports
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

// Hook / Lib Imports
import { useSubmitContact } from '@/hooks/api/useContact'
import { ApiError } from '@/libs/api-client'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'

// Mirrors SubmitContactMessageValidator (Features/Contact/Commands/SubmitContactMessage)
const schema = z.object({
  name: z.string().min(1, 'Name is required').max(100),
  email: z.string().min(1, 'Email is required').email('Enter a valid email').max(256),
  subject: z.string().min(1, 'Subject is required').max(200),
  message: z.string().min(1, 'Message is required').max(2000)
})

type ContactForm = z.infer<typeof schema>

const INFO = [
  { icon: 'fas fa-map-marker-alt', title: 'Address', lines: ['1429 Netus Rd, NY 48247'] },
  { icon: 'fas fa-envelope', title: 'Email', lines: ['hello@organi.dev'] },
  { icon: 'fas fa-phone-alt', title: 'Phone', lines: ['(+87) 4886-4174'] }
]

const ContactView = () => {
  const submitContact = useSubmitContact()
  const [formError, setFormError] = useState<string | null>(null)
  const [sent, setSent] = useState(false)

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting }
  } = useForm<ContactForm>({
    resolver: zodResolver(schema),
    defaultValues: { name: '', email: '', subject: '', message: '' }
  })

  const onSubmit = async (values: ContactForm) => {
    setFormError(null)

    try {
      await submitContact.mutateAsync(values)
      reset()
      setSent(true)
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : 'Could not send your message. Please try again.')
    }
  }

  return (
    <>
      <Breadcrumb title='Contact Us' items={[{ label: 'Contact' }]} />
      <section className='sec_space_large'>
        <div className='container'>
          <div className='row g-4'>
            {/* Info */}
            <div className='col-lg-4'>
              <div className='bg-white rounded-4 shadow-sm p-4 h-100'>
                <h5 className='mb-4' style={{ fontWeight: 800 }}>
                  Get in touch
                </h5>
                <p style={{ color: '#6b6b6b', fontSize: 14 }}>
                  Have a question about an order, a product, or becoming a vendor? We would love to hear from you.
                </p>
                <ul className='list-unstyled mt-4'>
                  {INFO.map(info => (
                    <li key={info.title} className='d-flex gap-3 mb-4'>
                      <div
                        className='d-flex align-items-center justify-content-center rounded-circle flex-shrink-0'
                        style={{ width: 44, height: 44, background: '#7cc000' }}
                      >
                        <i className={info.icon} />
                      </div>
                      <div>
                        <strong>{info.title}</strong>
                        {info.lines.map(line => (
                          <p key={line} className='mb-0' style={{ color: '#6b6b6b', fontSize: 14 }}>
                            {line}
                          </p>
                        ))}
                      </div>
                    </li>
                  ))}
                </ul>
              </div>
            </div>

            {/* Form */}
            <div className='col-lg-8'>
              <div className='bg-white rounded-4 shadow-sm p-4 p-md-5'>
                {sent ? (
                  <div className='text-center py-5'>
                    <div style={{ fontSize: 48, color: '#4f7d00' }}>
                      <i className='fas fa-check-circle' />
                    </div>
                    <h3 className='mt-3' style={{ fontWeight: 800 }}>
                      Message sent!
                    </h3>
                    <p style={{ color: '#6b6b6b' }}>Thanks for reaching out — our team will get back to you shortly.</p>
                    <button
                      type='button'
                      className='btn rounded-pill px-4 border mt-2'
                      onClick={() => setSent(false)}
                    >
                      Send another message
                    </button>
                  </div>
                ) : (
                  <form onSubmit={handleSubmit(onSubmit)} className='row g-3' noValidate>
                    <h5 className='mb-1' style={{ fontWeight: 800 }}>
                      Send us a message
                    </h5>
                    {formError && (
                      <div className='col-12'>
                        <div className='alert alert-danger mb-0'>{formError}</div>
                      </div>
                    )}
                    <div className='col-md-6'>
                      <input className='form-control rounded-pill py-3' aria-label='Your name' placeholder='Your name' {...register('name')} />
                      {errors.name && <small className='text-danger'>{errors.name.message}</small>}
                    </div>
                    <div className='col-md-6'>
                      <input
                        type='email'
                        className='form-control rounded-pill py-3'
                        aria-label='Your email'
                        placeholder='Your email'
                        {...register('email')}
                      />
                      {errors.email && <small className='text-danger'>{errors.email.message}</small>}
                    </div>
                    <div className='col-12'>
                      <input className='form-control rounded-pill py-3' aria-label='Subject' placeholder='Subject' {...register('subject')} />
                      {errors.subject && <small className='text-danger'>{errors.subject.message}</small>}
                    </div>
                    <div className='col-12'>
                      <textarea
                        className='form-control rounded-4 p-3'
                        rows={5}
                        aria-label='Your message'
                        placeholder='Your message'
                        {...register('message')}
                      />
                      {errors.message && <small className='text-danger'>{errors.message.message}</small>}
                    </div>
                    <div className='col-12'>
                      <button
                        type='submit'
                        className='btn custom_btn rounded-pill px-4'
                        disabled={isSubmitting}
                      >
                        {isSubmitting ? 'Sending…' : 'Send Message'}
                      </button>
                    </div>
                  </form>
                )}
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  )
}

export default ContactView
