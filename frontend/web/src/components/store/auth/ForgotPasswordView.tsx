'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'

// Third-party Imports
import { useForm } from 'react-hook-form'

// Hook / Lib Imports
import { useForgotPassword } from '@/hooks/api/useAuthEmail'
import { ApiError } from '@/libs/api-client'

// Component Imports
import AuthCard from '@/components/store/auth/AuthCard'

interface ForgotPasswordForm {
  email: string
}

const ForgotPasswordView = () => {
  const forgotPassword = useForgotPassword()
  const [formError, setFormError] = useState<string | null>(null)
  const [sentTo, setSentTo] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<ForgotPasswordForm>({ defaultValues: { email: '' } })

  const onSubmit = async (values: ForgotPasswordForm) => {
    setFormError(null)

    try {
      await forgotPassword.mutateAsync(values.email)
      setSentTo(values.email)
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : 'Something went wrong. Please try again.')
    }
  }

  // The backend deliberately responds the same way whether or not the address has an account,
  // so this screen must not imply the address was found.
  if (sentTo) {
    return (
      <AuthCard
        title='Check your inbox'
        breadcrumbLabel='Forgot Password'
        footer={
          <Link href='/login' className='text-primary'>
            Back to login
          </Link>
        }
      >
        <div className='text-center'>
          <div style={{ fontSize: 48, color: '#4f7d00' }}>
            <i className='fas fa-envelope-circle-check' aria-hidden='true' />
          </div>
          <p className='mt-3' style={{ color: '#6b6b6b' }}>
            If an account exists for <strong>{sentTo}</strong>, we&apos;ve sent it a 6-digit reset code. The code
            expires in 15 minutes.
          </p>
          <Link href={`/reset-password?email=${encodeURIComponent(sentTo)}`} className='btn custom_btn rounded-pill px-4 mt-2'>
            Enter Code
          </Link>
        </div>
      </AuthCard>
    )
  }

  return (
    <AuthCard
      title='Forgot your password?'
      breadcrumbLabel='Forgot Password'
      footer={
        <>
          Remembered it?{' '}
          <Link href='/login' className='text-primary'>
            Back to login
          </Link>
        </>
      }
    >
      <p className='text-center mb-4' style={{ color: '#6b6b6b' }}>
        Enter your email address and we&apos;ll send you a code to reset your password.
      </p>
      {formError && <div className='alert alert-danger'>{formError}</div>}
      <form onSubmit={handleSubmit(onSubmit)} className='d-flex flex-column gap-3' noValidate>
        <div className='form_item'>
          <label className='form-label' htmlFor='forgot-email'>
            Email address
          </label>
          <input
            id='forgot-email'
            type='email'
            className='form-control rounded-pill py-3'
            autoComplete='email'
            {...register('email', { required: 'Email is required' })}
          />
          {errors.email && <small className='text-danger'>{errors.email.message}</small>}
        </div>
        <button type='submit' className='btn custom_btn rounded-pill py-3' disabled={isSubmitting}>
          {isSubmitting ? 'Sending…' : 'Send Reset Code'}
        </button>
      </form>
    </AuthCard>
  )
}

export default ForgotPasswordView
