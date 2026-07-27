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
          <input
            className='form-control rounded-pill py-3'
            placeholder='First name'
            {...register('firstName', { required: 'First name is required' })}
          />
          {errors.firstName && <small className='text-danger'>{errors.firstName.message}</small>}
        </div>
        <div className='col-md-6'>
          <input
            className='form-control rounded-pill py-3'
            placeholder='Last name'
            {...register('lastName', { required: 'Last name is required' })}
          />
          {errors.lastName && <small className='text-danger'>{errors.lastName.message}</small>}
        </div>
        <div className='col-12'>
          <input
            type='email'
            className='form-control rounded-pill py-3'
            placeholder='Email address'
            {...register('email', { required: 'Email is required' })}
          />
          {errors.email && <small className='text-danger'>{errors.email.message}</small>}
        </div>
        <div className='col-12'>
          <input
            className='form-control rounded-pill py-3'
            placeholder='Phone number (optional)'
            {...register('phoneNumber')}
          />
        </div>
        <div className='col-12'>
          <input
            type='password'
            className='form-control rounded-pill py-3'
            placeholder='Password'
            {...register('password', { required: 'Password is required' })}
          />
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
