'use client'

// React Imports
import { useState } from 'react'

// Next Imports
import Link from 'next/link'
import { useRouter } from 'next/navigation'

// Third-party Imports
import { useForm } from 'react-hook-form'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Component Imports
import AuthCard from '@/components/store/auth/AuthCard'

interface RegisterForm {
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  password: string
}

const StoreRegisterView = () => {
  const { register: registerUser } = useAuth()
  const router = useRouter()
  const [formError, setFormError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<RegisterForm>({
    defaultValues: { firstName: '', lastName: '', email: '', phoneNumber: '', password: '' }
  })

  const onSubmit = async (values: RegisterForm) => {
    setFormError(null)

    try {
      await registerUser({
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        password: values.password,
        phoneNumber: values.phoneNumber || null
      })
      router.push('/')
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : 'Could not create your account. Please try again.')
    }
  }

  return (
    <AuthCard
      title='Create your account'
      breadcrumbLabel='Register'
      footer={
        <>
          Already have an account?{' '}
          <Link href='/login' className='text-primary'>
            Login
          </Link>
        </>
      }
    >
      {formError && <div className='alert alert-danger'>{formError}</div>}
      <form onSubmit={handleSubmit(onSubmit)} className='row g-3' noValidate>
        <div className='col-md-6'>
          <label className='form-label' htmlFor='register-firstName'>
            First name
          </label>
          <input
            id='register-firstName'
            className='form-control rounded-pill py-3'
            autoComplete='given-name'
            {...register('firstName', { required: 'First name is required' })}
          />
          {errors.firstName && <small className='text-danger'>{errors.firstName.message}</small>}
        </div>
        <div className='col-md-6'>
          <label className='form-label' htmlFor='register-lastName'>
            Last name
          </label>
          <input
            id='register-lastName'
            className='form-control rounded-pill py-3'
            autoComplete='family-name'
            {...register('lastName', { required: 'Last name is required' })}
          />
          {errors.lastName && <small className='text-danger'>{errors.lastName.message}</small>}
        </div>
        <div className='col-12'>
          <label className='form-label' htmlFor='register-email'>
            Email address
          </label>
          <input
            id='register-email'
            type='email'
            className='form-control rounded-pill py-3'
            autoComplete='email'
            {...register('email', { required: 'Email is required' })}
          />
          {errors.email && <small className='text-danger'>{errors.email.message}</small>}
        </div>
        <div className='col-12'>
          <label className='form-label' htmlFor='register-phoneNumber'>
            Phone number<span style={{ color: '#6b6b6b', fontWeight: 400 }}> (optional)</span>
          </label>
          <input
            id='register-phoneNumber'
            type='tel'
            className='form-control rounded-pill py-3'
            autoComplete='tel'
            {...register('phoneNumber')}
          />
        </div>
        <div className='col-12'>
          <label className='form-label' htmlFor='register-password'>
            Password
          </label>
          <input
            id='register-password'
            type='password'
            className='form-control rounded-pill py-3'
            autoComplete='new-password'
            aria-describedby='register-password-hint'
            {...register('password', { required: 'Password is required' })}
          />
          <small id='register-password-hint' className='d-block mt-1' style={{ color: '#6b6b6b' }}>
            At least 8 characters, with an uppercase letter, a lowercase letter, a number and a special character.
          </small>
          {errors.password && <small className='text-danger'>{errors.password.message}</small>}
        </div>
        <div className='col-12'>
          <button type='submit' className='btn custom_btn rounded-pill py-3 w-100' disabled={isSubmitting}>
            {isSubmitting ? 'Creating account…' : 'Create Account'}
          </button>
        </div>
      </form>
    </AuthCard>
  )
}

export default StoreRegisterView
