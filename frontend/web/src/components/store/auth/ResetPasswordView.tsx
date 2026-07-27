'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'
import { useSearchParams } from 'next/navigation'

// Third-party Imports
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

// Hook / Lib Imports
import { useResetPassword } from '@/hooks/api/useAuthEmail'
import { ApiError } from '@/libs/api-client'

// Component Imports
import AuthCard from '@/components/store/auth/AuthCard'

// Mirrors PasswordRules.cs + ResetPasswordValidator.cs so the user sees rule failures
// before a round trip.
const schema = z
  .object({
    email: z.string().min(1, 'Email is required').email('Enter a valid email'),
    code: z.string().regex(/^[0-9]{6}$/, 'Enter the 6-digit code from your email'),
    newPassword: z
      .string()
      .min(8, 'Password must be at least 8 characters')
      .max(128, 'Password must not exceed 128 characters')
      .regex(/[A-Z]/, 'Password must contain an uppercase letter')
      .regex(/[a-z]/, 'Password must contain a lowercase letter')
      .regex(/[0-9]/, 'Password must contain a digit')
      .regex(/[^a-zA-Z0-9]/, 'Password must contain a special character'),
    confirmPassword: z.string().min(1, 'Please confirm your password')
  })
  .refine(values => values.newPassword === values.confirmPassword, {
    message: 'Passwords do not match',
    path: ['confirmPassword']
  })

type ResetPasswordForm = z.infer<typeof schema>

const ResetPasswordView = () => {
  const searchParams = useSearchParams()
  const resetPassword = useResetPassword()

  const [formError, setFormError] = useState<string | null>(null)
  const [done, setDone] = useState(false)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<ResetPasswordForm>({
    resolver: zodResolver(schema),
    defaultValues: {
      email: searchParams.get('email') ?? '',
      code: '',
      newPassword: '',
      confirmPassword: ''
    }
  })

  const onSubmit = async (values: ResetPasswordForm) => {
    setFormError(null)

    try {
      await resetPassword.mutateAsync({
        email: values.email,
        code: values.code,
        newPassword: values.newPassword
      })
      setDone(true)
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : 'Could not reset your password. Please try again.')
    }
  }

  if (done) {
    return (
      <AuthCard title='Password updated' breadcrumbLabel='Reset Password' footer={null}>
        <div className='text-center'>
          <div style={{ fontSize: 48, color: '#4f7d00' }}>
            <i className='fas fa-check-circle' aria-hidden='true' />
          </div>
          <p className='mt-3' style={{ color: '#6b6b6b' }}>
            Your password has been changed and you&apos;ve been signed out everywhere else.
          </p>
          <Link href='/login' className='btn custom_btn rounded-pill px-4 mt-2'>
            Login
          </Link>
        </div>
      </AuthCard>
    )
  }

  return (
    <AuthCard
      title='Set a new password'
      breadcrumbLabel='Reset Password'
      footer={
        <>
          Didn&apos;t get a code?{' '}
          <Link href='/forgot-password' className='text-primary'>
            Request a new one
          </Link>
        </>
      }
    >
      {formError && <div className='alert alert-danger'>{formError}</div>}
      <form onSubmit={handleSubmit(onSubmit)} className='d-flex flex-column gap-3' noValidate>
        <div className='form_item'>
          <input
            type='email'
            className='form-control rounded-pill py-3'
            aria-label='Email address'
            placeholder='Email address'
            {...register('email')}
          />
          {errors.email && <small className='text-danger'>{errors.email.message}</small>}
        </div>
        <div className='form_item'>
          <input
            type='text'
            inputMode='numeric'
            maxLength={6}
            className='form-control rounded-pill py-3 text-center'
            aria-label='6-digit reset code'
            placeholder='6-digit code'
            style={{ letterSpacing: 8, fontWeight: 700 }}
            {...register('code')}
          />
          {errors.code && <small className='text-danger'>{errors.code.message}</small>}
        </div>
        <div className='form_item'>
          <input
            type='password'
            className='form-control rounded-pill py-3'
            aria-label='New password'
            placeholder='New password'
            {...register('newPassword')}
          />
          {errors.newPassword && <small className='text-danger'>{errors.newPassword.message}</small>}
        </div>
        <div className='form_item'>
          <input
            type='password'
            className='form-control rounded-pill py-3'
            aria-label='Confirm new password'
            placeholder='Confirm new password'
            {...register('confirmPassword')}
          />
          {errors.confirmPassword && <small className='text-danger'>{errors.confirmPassword.message}</small>}
        </div>
        <button type='submit' className='btn custom_btn rounded-pill py-3' disabled={isSubmitting}>
          {isSubmitting ? 'Updating…' : 'Reset Password'}
        </button>
      </form>
    </AuthCard>
  )
}

export default ResetPasswordView
