'use client'

// React Imports
import { useEffect } from 'react'

// Third-party Imports
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

// MUI Imports
import Dialog from '@mui/material/Dialog'
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Switch from '@mui/material/Switch'
import FormControlLabel from '@mui/material/FormControlLabel'
import Grid from '@mui/material/Grid'

// Type Imports
import type { CouponResponse, CreateCouponRequest, UpdateCouponRequest } from '@/types/api/coupon'

// Mirrors CreateCouponValidator / UpdateCouponValidator on the backend — see
// Features/Coupons/Commands/{Create,Update}Coupon/*Validator.cs
const couponSchema = z
  .object({
    code: z.string().min(1, 'Coupon code is required.').max(50, 'Coupon code must not exceed 50 characters.'),
    description: z.string().max(256, 'Description must not exceed 256 characters.'),
    discountType: z.enum(['Percentage', 'FixedAmount']),
    discountValue: z.coerce.number().gt(0, 'Discount value must be greater than zero.'),
    minimumOrderAmount: z.union([z.literal(''), z.coerce.number().min(0, 'Minimum order amount must be zero or greater.')]),
    maxUsageCount: z.union([z.literal(''), z.coerce.number().int().gt(0, 'Maximum usage count must be greater than zero.')]),
    startDate: z.string().min(1, 'Start date is required.'),
    endDate: z.string().min(1, 'End date is required.'),
    isActive: z.boolean()
  })
  .refine(data => data.discountType !== 'Percentage' || data.discountValue <= 100, {
    message: 'Percentage discount cannot exceed 100.',
    path: ['discountValue']
  })
  .refine(data => !data.startDate || !data.endDate || new Date(data.endDate) > new Date(data.startDate), {
    message: 'End date must be after the start date.',
    path: ['endDate']
  })

// zod's coerce fields make the schema's input type (raw strings from the DOM) differ from its
// output type (parsed numbers) — react-hook-form needs both generics to bridge that.
type CouponFormInput = z.input<typeof couponSchema>
type CouponFormValues = z.output<typeof couponSchema>

const toDateInput = (iso: string): string => iso.slice(0, 10)

const emptyValues: CouponFormInput = {
  code: '',
  description: '',
  discountType: 'Percentage',
  discountValue: 10,
  minimumOrderAmount: '',
  maxUsageCount: '',
  startDate: '',
  endDate: '',
  isActive: true
}

interface CouponFormDialogProps {
  open: boolean
  coupon: CouponResponse | null // null = create mode
  isPending: boolean
  serverError: string | null
  onSubmit: (values: CreateCouponRequest | UpdateCouponRequest) => void
  onClose: () => void
}

const CouponFormDialog = ({ open, coupon, isPending, serverError, onSubmit, onClose }: CouponFormDialogProps) => {
  const isEdit = coupon !== null

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<CouponFormInput, unknown, CouponFormValues>({
    resolver: zodResolver(couponSchema),
    defaultValues: emptyValues
  })

  useEffect(() => {
    if (!open) return

    reset(
      coupon
        ? {
            code: coupon.code,
            description: coupon.description ?? '',
            discountType: coupon.discountType,
            discountValue: coupon.discountValue,
            minimumOrderAmount: coupon.minimumOrderAmount ?? '',
            maxUsageCount: coupon.maxUsageCount ?? '',
            startDate: toDateInput(coupon.startDate),
            endDate: toDateInput(coupon.endDate),
            isActive: coupon.isActive
          }
        : emptyValues
    )
  }, [open, coupon, reset])

  const submitForm = (values: CouponFormValues) => {
    const base: CreateCouponRequest = {
      code: values.code,
      description: values.description || null,
      discountType: values.discountType,
      discountValue: values.discountValue,
      minimumOrderAmount: values.minimumOrderAmount === '' ? null : values.minimumOrderAmount,
      maxUsageCount: values.maxUsageCount === '' ? null : values.maxUsageCount,
      startDate: values.startDate,
      endDate: values.endDate
    }

    onSubmit(isEdit ? { ...base, isActive: values.isActive } : base)
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth='sm' fullWidth>
      <DialogTitle>{isEdit ? `Edit Coupon — ${coupon.code}` : 'New Coupon'}</DialogTitle>
      <form onSubmit={handleSubmit(submitForm)} noValidate>
        <DialogContent className='flex flex-col gap-5 pbs-2'>
          {serverError && (
            <TextField
              value={serverError}
              error
              fullWidth
              multiline
              InputProps={{ readOnly: true }}
              variant='standard'
              label='Server error'
            />
          )}
          <Grid container spacing={5}>
            <Grid item xs={12} sm={6}>
              <Controller
                name='code'
                control={control}
                render={({ field }) => (
                  <TextField {...field} fullWidth label='Code' error={!!errors.code} helperText={errors.code?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <Controller
                name='discountType'
                control={control}
                render={({ field }) => (
                  <TextField {...field} select fullWidth label='Discount Type'>
                    <MenuItem value='Percentage'>Percentage</MenuItem>
                    <MenuItem value='FixedAmount'>Fixed Amount</MenuItem>
                  </TextField>
                )}
              />
            </Grid>
            <Grid item xs={12}>
              <Controller
                name='description'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    label='Description'
                    error={!!errors.description}
                    helperText={errors.description?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <Controller
                name='discountValue'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    type='number'
                    label='Discount Value'
                    error={!!errors.discountValue}
                    helperText={errors.discountValue?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <Controller
                name='minimumOrderAmount'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    type='number'
                    label='Min. Order Amount'
                    error={!!errors.minimumOrderAmount}
                    helperText={errors.minimumOrderAmount?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <Controller
                name='maxUsageCount'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    type='number'
                    label='Max Usage Count'
                    error={!!errors.maxUsageCount}
                    helperText={errors.maxUsageCount?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <Controller
                name='startDate'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    type='date'
                    label='Start Date'
                    InputLabelProps={{ shrink: true }}
                    error={!!errors.startDate}
                    helperText={errors.startDate?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <Controller
                name='endDate'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    type='date'
                    label='End Date'
                    InputLabelProps={{ shrink: true }}
                    error={!!errors.endDate}
                    helperText={errors.endDate?.message}
                  />
                )}
              />
            </Grid>
            {isEdit && (
              <Grid item xs={12}>
                <Controller
                  name='isActive'
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Switch checked={field.value} onChange={field.onChange} />}
                      label='Active'
                    />
                  )}
                />
              </Grid>
            )}
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} color='secondary' disabled={isPending}>
            Cancel
          </Button>
          <Button type='submit' variant='contained' disabled={isPending}>
            {isPending ? 'Saving…' : isEdit ? 'Save Changes' : 'Create Coupon'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  )
}

export default CouponFormDialog
