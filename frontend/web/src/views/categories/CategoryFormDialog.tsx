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
import Alert from '@mui/material/Alert'

// Type Imports
import type { CategoryResponse, CreateCategoryRequest, UpdateCategoryRequest } from '@/types/api/category'
import type { FlattenedCategory } from '@/utils/categoryTree'

// Mirrors CreateCategoryValidator / UpdateCategoryValidator — see
// Features/Categories/Commands/{Create,Update}Category/*Validator.cs
const categorySchema = z.object({
  name: z.string().min(1, 'Category name is required.').max(100, 'Category name must not exceed 100 characters.'),
  description: z.string().max(500, 'Description must not exceed 500 characters.'),
  imageUrl: z.string().max(500, 'Image URL must not exceed 500 characters.'),
  displayOrder: z.coerce.number().int('Display order must be a whole number.').min(0, 'Display order must be zero or greater.'),
  parentCategoryId: z.string(),
  isActive: z.boolean()
})

// zod's coerce fields make the schema's input type (raw strings from the DOM) differ from its
// output type (parsed numbers) — react-hook-form needs both generics to bridge that.
type CategoryFormInput = z.input<typeof categorySchema>
type CategoryFormValues = z.output<typeof categorySchema>

const emptyValues: CategoryFormInput = {
  name: '',
  description: '',
  imageUrl: '',
  displayOrder: 0,
  parentCategoryId: '',
  isActive: true
}

interface CategoryFormDialogProps {
  open: boolean
  category: CategoryResponse | null // null = create mode
  categories: FlattenedCategory[] // for the parent select
  isPending: boolean
  serverError: string | null
  onSubmit: (values: CreateCategoryRequest | UpdateCategoryRequest) => void
  onClose: () => void
}

const CategoryFormDialog = ({
  open,
  category,
  categories,
  isPending,
  serverError,
  onSubmit,
  onClose
}: CategoryFormDialogProps) => {
  const isEdit = category !== null

  // Backend rejects self-parenting with a 409; excluding self client-side avoids the round trip.
  // Deeper cases (max depth 3, duplicate sibling name) still surface via serverError.
  const parentOptions = categories.filter(c => c.id !== category?.id)

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<CategoryFormInput, unknown, CategoryFormValues>({
    resolver: zodResolver(categorySchema),
    defaultValues: emptyValues
  })

  useEffect(() => {
    if (!open) return

    reset(
      category
        ? {
            name: category.name,
            description: category.description ?? '',
            imageUrl: category.imageUrl ?? '',
            displayOrder: category.displayOrder,
            parentCategoryId: category.parentCategoryId ?? '',
            isActive: category.isActive
          }
        : emptyValues
    )
  }, [open, category, reset])

  const submitForm = (values: CategoryFormValues) => {
    const base: CreateCategoryRequest = {
      name: values.name,
      description: values.description || null,
      imageUrl: values.imageUrl || null,
      displayOrder: values.displayOrder,
      parentCategoryId: values.parentCategoryId === '' ? null : values.parentCategoryId
    }

    onSubmit(isEdit ? { ...base, isActive: values.isActive } : base)
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth='sm' fullWidth>
      <DialogTitle>{isEdit ? `Edit Category — ${category.name}` : 'New Category'}</DialogTitle>
      <form onSubmit={handleSubmit(submitForm)} noValidate>
        <DialogContent className='flex flex-col gap-5 pbs-2'>
          {serverError && <Alert severity='error'>{serverError}</Alert>}
          <Grid container spacing={5}>
            <Grid item xs={12} sm={6}>
              <Controller
                name='name'
                control={control}
                render={({ field }) => (
                  <TextField {...field} fullWidth label='Name' error={!!errors.name} helperText={errors.name?.message} />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <Controller
                name='parentCategoryId'
                control={control}
                render={({ field }) => (
                  <TextField {...field} select fullWidth label='Parent Category'>
                    <MenuItem value=''>None (top level)</MenuItem>
                    {parentOptions.map(option => (
                      <MenuItem key={option.id} value={option.id}>
                        {' '.repeat(option.depth * 4) + option.name}
                      </MenuItem>
                    ))}
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
            <Grid item xs={12} sm={8}>
              <Controller
                name='imageUrl'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    label='Image URL'
                    error={!!errors.imageUrl}
                    helperText={errors.imageUrl?.message}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <Controller
                name='displayOrder'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    type='number'
                    label='Display Order'
                    error={!!errors.displayOrder}
                    helperText={errors.displayOrder?.message}
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
            {isPending ? 'Saving…' : isEdit ? 'Save Changes' : 'Create Category'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  )
}

export default CategoryFormDialog
