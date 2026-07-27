'use client'

// React Imports
import { useEffect } from 'react'

// Third-party Imports
import { useForm } from 'react-hook-form'

// Hook / Context Imports
import { useProfile, useUpdateProfile } from '@/hooks/api/useProfile'
import { useStoreToast } from '@/components/store/StoreToast'
import { ApiError } from '@/libs/api-client'

interface ProfileForm {
  firstName: string
  lastName: string
  phoneNumber: string
  dateOfBirth: string
  avatarUrl: string
}

const AccountProfilePage = () => {
  const { data: profile, isLoading } = useProfile()
  const updateProfile = useUpdateProfile()
  const { showToast } = useStoreToast()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting }
  } = useForm<ProfileForm>({
    defaultValues: { firstName: '', lastName: '', phoneNumber: '', dateOfBirth: '', avatarUrl: '' }
  })

  useEffect(() => {
    if (profile) {
      reset({
        firstName: profile.firstName,
        lastName: profile.lastName,
        phoneNumber: profile.phoneNumber ?? '',
        dateOfBirth: profile.dateOfBirth ?? '',
        avatarUrl: profile.avatarUrl ?? ''
      })
    }
  }, [profile, reset])

  const onSubmit = async (values: ProfileForm) => {
    try {
      await updateProfile.mutateAsync({
        firstName: values.firstName,
        lastName: values.lastName,
        phoneNumber: values.phoneNumber || null,
        dateOfBirth: values.dateOfBirth || null,
        avatarUrl: values.avatarUrl || null
      })
      showToast('Profile updated.')
    } catch (e) {
      showToast(e instanceof ApiError ? e.message : 'Could not update profile.', 'error')
    }
  }

  if (isLoading) return <p>Loading profile…</p>

  return (
    <div className='bg-white rounded-4 shadow-sm p-4'>
      <h5 className='mb-4' style={{ fontWeight: 800 }}>
        My Profile
      </h5>
      <form onSubmit={handleSubmit(onSubmit)} className='row g-3' noValidate>
        <div className='col-md-6'>
          <label className='form-label'>First name</label>
          <input
            className='form-control rounded-pill py-2'
            {...register('firstName', { required: 'First name is required' })}
          />
          {errors.firstName && <small className='text-danger'>{errors.firstName.message}</small>}
        </div>
        <div className='col-md-6'>
          <label className='form-label'>Last name</label>
          <input
            className='form-control rounded-pill py-2'
            {...register('lastName', { required: 'Last name is required' })}
          />
          {errors.lastName && <small className='text-danger'>{errors.lastName.message}</small>}
        </div>
        <div className='col-md-6'>
          <label className='form-label'>Email</label>
          <input className='form-control rounded-pill py-2' value={profile?.email ?? ''} disabled />
        </div>
        <div className='col-md-6'>
          <label className='form-label'>Phone number</label>
          <input className='form-control rounded-pill py-2' {...register('phoneNumber')} />
        </div>
        <div className='col-md-6'>
          <label className='form-label'>Date of birth</label>
          <input type='date' className='form-control rounded-pill py-2' {...register('dateOfBirth')} />
        </div>
        <div className='col-md-6'>
          <label className='form-label'>Avatar URL</label>
          <input className='form-control rounded-pill py-2' {...register('avatarUrl')} />
        </div>
        <div className='col-12'>
          <button type='submit' className='btn custom_btn rounded-pill px-5 py-2' disabled={isSubmitting}>
            {isSubmitting ? 'Saving…' : 'Save Changes'}
          </button>
        </div>
      </form>
    </div>
  )
}

export default AccountProfilePage
