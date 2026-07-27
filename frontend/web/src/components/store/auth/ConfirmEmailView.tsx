'use client'

// React Imports
import { useEffect, useRef, useState } from 'react'

// Next Imports
import Link from 'next/link'
import { useSearchParams } from 'next/navigation'

// Third-party Imports
import { useQueryClient } from '@tanstack/react-query'

// Hook / Lib Imports
import { useConfirmEmail, useResendConfirmation } from '@/hooks/api/useAuthEmail'
import { ApiError } from '@/libs/api-client'

// Component Imports
import AuthCard from '@/components/store/auth/AuthCard'

type Status = 'working' | 'success' | 'failed' | 'missing-token'

const ConfirmEmailView = () => {
  const searchParams = useSearchParams()
  const token = searchParams.get('token')

  const confirmEmail = useConfirmEmail()
  const resendConfirmation = useResendConfirmation()
  const queryClient = useQueryClient()

  const [status, setStatus] = useState<Status>(token ? 'working' : 'missing-token')
  const [message, setMessage] = useState<string | null>(null)
  const [resendEmail, setResendEmail] = useState('')
  const [resent, setResent] = useState(false)

  // React 18 StrictMode mounts effects twice in dev; the token is single-use, so a second
  // call would report "already used" and overwrite a successful result.
  const attempted = useRef(false)

  useEffect(() => {
    if (!token || attempted.current) return
    attempted.current = true

    confirmEmail
      .mutateAsync(token)
      .then(() => {
        setStatus('success')

        // Drop the cached profile so an already-open session stops showing the
        // "confirm your email" banner without needing a reload.
        queryClient.invalidateQueries({ queryKey: ['profile'] })
      })
      .catch(error => {
        setMessage(error instanceof ApiError ? error.message : 'We could not confirm your email address.')
        setStatus('failed')
      })
  }, [token, confirmEmail, queryClient])

  const onResend = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!resendEmail.trim()) return

    try {
      await resendConfirmation.mutateAsync(resendEmail.trim())
    } finally {
      // Always report the same outcome — the endpoint intentionally reveals nothing.
      setResent(true)
    }
  }

  const body = () => {
    if (status === 'working') {
      return <p className='text-center' style={{ color: 'var(--organi-text-muted)' }}>Confirming your email address…</p>
    }

    if (status === 'success') {
      return (
        <div className='text-center'>
          <div style={{ fontSize: 48, color: 'var(--organi-brand-ink)' }}>
            <i className='fas fa-check-circle' aria-hidden='true' />
          </div>
          <p className='mt-3' style={{ color: 'var(--organi-text-muted)' }}>
            Your email address is confirmed. Thanks for verifying your account!
          </p>
          <Link href='/shop' className='btn custom_btn rounded-pill px-4 mt-2'>
            Start Shopping
          </Link>
        </div>
      )
    }

    return (
      <div className='text-center'>
        <div style={{ fontSize: 48, color: 'var(--organi-danger)' }}>
          <i className='fas fa-triangle-exclamation' aria-hidden='true' />
        </div>
        <p className='mt-3' style={{ color: 'var(--organi-text-muted)' }}>
          {status === 'missing-token'
            ? 'This page needs a confirmation link from your email.'
            : message}
        </p>

        {resent ? (
          <p style={{ color: 'var(--organi-text-muted)' }}>
            If that address has an unconfirmed account, a new confirmation link is on its way.
          </p>
        ) : (
          <form onSubmit={onResend} className='d-flex flex-column gap-3 mt-4 text-start'>
            <label className='form-label' htmlFor='resend-email' style={{ color: 'var(--organi-text-muted)' }}>
              Send a new confirmation link
            </label>
            <input
              id='resend-email'
              type='email'
              className='form-control rounded-pill py-3'
              placeholder='Email address'
              value={resendEmail}
              onChange={e => setResendEmail(e.target.value)}
            />
            <button
              type='submit'
              className='btn custom_btn rounded-pill py-3'
              disabled={resendConfirmation.isPending}
            >
              {resendConfirmation.isPending ? 'Sending…' : 'Resend Link'}
            </button>
          </form>
        )}
      </div>
    )
  }

  return (
    <AuthCard
      title='Confirm your email'
      breadcrumbLabel='Confirm Email'
      footer={
        <Link href='/login' className='text-primary'>
          Back to login
        </Link>
      }
    >
      {body()}
    </AuthCard>
  )
}

export default ConfirmEmailView
