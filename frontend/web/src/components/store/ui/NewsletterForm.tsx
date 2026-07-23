'use client'

// React Imports
import { useState } from 'react'

// Lib Imports
import { apiFetch, ApiError } from '@/libs/api-client'

// Footer newsletter subscribe, wired to the public POST /api/newsletter/subscribe.
const NewsletterForm = () => {
  const [email, setEmail] = useState('')
  const [status, setStatus] = useState<'idle' | 'loading' | 'ok' | 'error'>('idle')
  const [message, setMessage] = useState('')

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!email.trim()) return

    setStatus('loading')

    try {
      await apiFetch('/api/newsletter/subscribe', { method: 'POST', body: { email: email.trim() } })
      setStatus('ok')
      setMessage('Thanks for subscribing!')
      setEmail('')
    } catch (error) {
      setStatus('error')
      setMessage(error instanceof ApiError ? error.message : 'Subscription failed. Please try again.')
    }
  }

  return (
    <form className='footer_top_subs position-relative' onSubmit={onSubmit}>
      <input
        className='rounded-pill'
        type='email'
        name='email'
        placeholder='Your Email Address'
        value={email}
        onChange={e => setEmail(e.target.value)}
      />
      <button type='submit' className='btn custom_btn rounded-pill text-white position-absolute' disabled={status === 'loading'}>
        {status === 'loading' ? 'Subscribing…' : 'Subscribe Now'} <i className='fas fa-long-arrow-alt-right' />
      </button>
      {message && (
        <p className='mt-2 mb-0' style={{ color: status === 'error' ? '#dc3545' : '#fff', fontSize: 14 }}>
          {message}
        </p>
      )}
    </form>
  )
}

export default NewsletterForm
