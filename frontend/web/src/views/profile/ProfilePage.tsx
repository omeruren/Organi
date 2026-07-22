'use client'

// React Imports
import { useEffect } from 'react'

// Third-party Imports
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import Avatar from '@mui/material/Avatar'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import Button from '@mui/material/Button'
import Divider from '@mui/material/Divider'
import CircularProgress from '@mui/material/CircularProgress'

// Component Imports
import { useToast } from '@components/ToastProvider'

// Hook Imports
import { useProfile, useUpdateProfile } from '@/hooks/api/useProfile'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Mirrors UpdateProfileValidator — Features/Profile/Commands/UpdateProfile/UpdateProfileValidator.cs
const profileSchema = z.object({
  firstName: z.string().min(1, 'First name is required.').max(100, 'First name must not exceed 100 characters.'),
  lastName: z.string().min(1, 'Last name is required.').max(100, 'Last name must not exceed 100 characters.'),
  phoneNumber: z.string().max(20, 'Phone number must not exceed 20 characters.'),
  dateOfBirth: z.string(),
  avatarUrl: z.string().max(500, 'Avatar URL must not exceed 500 characters.')
})

type ProfileFormValues = z.infer<typeof profileSchema>

const emptyValues: ProfileFormValues = {
  firstName: '',
  lastName: '',
  phoneNumber: '',
  dateOfBirth: '',
  avatarUrl: ''
}

const ProfilePage = () => {
  const { showToast } = useToast()
  const { data: profile, isLoading } = useProfile()
  const updateProfile = useUpdateProfile()

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<ProfileFormValues>({
    resolver: zodResolver(profileSchema),
    defaultValues: emptyValues
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

  const onSubmit = async (values: ProfileFormValues) => {
    try {
      await updateProfile.mutateAsync({
        firstName: values.firstName,
        lastName: values.lastName,
        phoneNumber: values.phoneNumber || null,
        dateOfBirth: values.dateOfBirth || null,
        avatarUrl: values.avatarUrl || null
      })
      showToast('Profile updated.')
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to update profile.', 'error')
    }
  }

  if (isLoading || !profile) {
    return (
      <div className='flex justify-center plb-12'>
        <CircularProgress />
      </div>
    )
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12} md={4}>
        <Card>
          <CardContent className='flex flex-col items-center text-center gap-4'>
            <Avatar src={profile.avatarUrl ?? undefined} className='bs-[96px] is-[96px]'>
              {profile.firstName.charAt(0)}
            </Avatar>
            <div>
              <Typography variant='h5'>{`${profile.firstName} ${profile.lastName}`}</Typography>
              <Typography color='text.secondary'>{profile.email}</Typography>
            </div>
            <div className='flex flex-wrap justify-center gap-2'>
              {profile.roles.map(role => (
                <Chip key={role} label={role} size='small' variant='tonal' color={role === 'Admin' ? 'primary' : 'default'} />
              ))}
            </div>
            <Divider className='is-full' />
            <div className='flex flex-col gap-2 is-full text-left'>
              <div className='flex justify-between'>
                <Typography variant='body2' color='text.secondary'>
                  Email confirmed
                </Typography>
                <Chip
                  label={profile.emailConfirmed ? 'Yes' : 'No'}
                  size='small'
                  variant='tonal'
                  color={profile.emailConfirmed ? 'success' : 'warning'}
                />
              </div>
              <div className='flex justify-between'>
                <Typography variant='body2' color='text.secondary'>
                  Last login
                </Typography>
                <Typography variant='body2'>
                  {profile.lastLoginAt ? new Date(profile.lastLoginAt).toLocaleDateString() : '—'}
                </Typography>
              </div>
              <div className='flex justify-between'>
                <Typography variant='body2' color='text.secondary'>
                  Member since
                </Typography>
                <Typography variant='body2'>{new Date(profile.createdAt).toLocaleDateString()}</Typography>
              </div>
            </div>
          </CardContent>
        </Card>
      </Grid>
      <Grid item xs={12} md={8}>
        <Card>
          <CardHeader title='Edit Profile' />
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} noValidate>
              <Grid container spacing={5}>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name='firstName'
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth label='First Name' error={!!errors.firstName} helperText={errors.firstName?.message} />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name='lastName'
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth label='Last Name' error={!!errors.lastName} helperText={errors.lastName?.message} />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField fullWidth label='Email' value={profile.email} disabled />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name='phoneNumber'
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth label='Phone Number' error={!!errors.phoneNumber} helperText={errors.phoneNumber?.message} />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name='dateOfBirth'
                    control={control}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        type='date'
                        label='Date of Birth'
                        InputLabelProps={{ shrink: true }}
                        error={!!errors.dateOfBirth}
                        helperText={errors.dateOfBirth?.message}
                      />
                    )}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name='avatarUrl'
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} fullWidth label='Avatar URL' error={!!errors.avatarUrl} helperText={errors.avatarUrl?.message} />
                    )}
                  />
                </Grid>
                <Grid item xs={12} className='flex gap-4'>
                  <Button type='submit' variant='contained' disabled={updateProfile.isPending}>
                    {updateProfile.isPending ? 'Saving…' : 'Save Changes'}
                  </Button>
                  <Button type='button' variant='outlined' color='secondary' onClick={() => reset()} disabled={updateProfile.isPending}>
                    Reset
                  </Button>
                </Grid>
              </Grid>
            </form>
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  )
}

export default ProfilePage
