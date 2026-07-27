'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'
import { useRouter, useSearchParams } from 'next/navigation'

// Third-party Imports
import { useForm } from 'react-hook-form'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Component Imports
import AuthCard from '@/components/store/auth/AuthCard'

interface LoginForm {
  email: string
  password: string
}

const StoreLoginView = () => {
  const { login } = useAuth()
  const router = useRouter()
  const searchParams = useSearchParams()
  const redirectTo = searchParams.get('redirectTo') || '/'
  const [formError, setFormError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<LoginForm>({ defaultValues: { email: '', password: '' } })

  const onSubmit = async (values: LoginForm) => {
    setFormError(null)

    try {
      await login(values.email, values.password)
      router.push(redirectTo)
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : 'Unable to log in. Please try again.')
    }
  }

  return (
    <AuthCard
      title='Login to your account'
      breadcrumbLabel='Login'
      footer={
        <>
          New to Organi?{' '}
          <Link href='/register' className='text-primary'>
            Create an account
          </Link>
        </>
      }
    >
      {formError && <div className='alert alert-danger'>{formError}</div>}
      <form onSubmit={handleSubmit(onSubmit)} className='d-flex flex-column gap-3' noValidate>
        <div className='form_item'>
          <label className='form-label' htmlFor='login-email'>
            Email address
          </label>
          <input
            id='login-email'
            type='email'
            className='form-control rounded-pill py-3'
            autoComplete='email'
            {...register('email', { required: 'Email is required' })}
          />
          {errors.email && <small className='text-danger'>{errors.email.message}</small>}
        </div>
        <div className='form_item'>
          <label className='form-label' htmlFor='login-password'>
            Password
          </label>
          <input
            id='login-password'
            type='password'
            className='form-control rounded-pill py-3'
            autoComplete='current-password'
            {...register('password', { required: 'Password is required' })}
          />
          {errors.password && <small className='text-danger'>{errors.password.message}</small>}
        </div>
        <div className='text-end'>
          <Link href='/forgot-password' className='text-primary' style={{ fontSize: 14 }}>
            Forgot password?
          </Link>
        </div>
        <button type='submit' className='btn custom_btn rounded-pill py-3' disabled={isSubmitting}>
          {isSubmitting ? 'Signing in…' : 'Login'}
        </button>
      </form>
    </AuthCard>
  )
}

export default StoreLoginView
