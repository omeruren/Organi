'use client'

// React Imports
import { useEffect } from 'react'

// Third-party Imports
import { useForm, useFieldArray, Controller } from 'react-hook-form'
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
import Checkbox from '@mui/material/Checkbox'
import FormControlLabel from '@mui/material/FormControlLabel'
import Grid from '@mui/material/Grid'
import Alert from '@mui/material/Alert'
import Typography from '@mui/material/Typography'
import IconButton from '@mui/material/IconButton'
import CircularProgress from '@mui/material/CircularProgress'
import Divider from '@mui/material/Divider'

// Hook Imports
import { useProduct } from '@/hooks/api/useProducts'
import { useCategories } from '@/hooks/api/useCategories'

// Util Imports
import { flattenCategories } from '@/utils/categoryTree'

// Type Imports
import type { CreateProductRequest, UpdateProductRequest, ProductStatus } from '@/types/api/product'

const PRODUCT_STATUSES: ProductStatus[] = ['Draft', 'Active', 'OutOfStock', 'Discontinued', 'PendingApproval']

// Mirrors CreateProductValidator / UpdateProductValidator — see
// Features/Products/Commands/{Create,Update}Product/*Validator.cs
const productSchema = z
  .object({
    name: z.string().min(1, 'Product name is required.').max(200, 'Product name must not exceed 200 characters.'),
    description: z.string().max(2000, 'Description must not exceed 2000 characters.'),
    shortDescription: z.string().max(500, 'Short description must not exceed 500 characters.'),
    price: z.coerce.number().gt(0, 'Price must be greater than zero.'),
    salePrice: z.union([z.literal(''), z.coerce.number().gt(0, 'Sale price must be greater than zero.')]),
    sku: z.string().min(1, 'SKU is required.').max(50, 'SKU must not exceed 50 characters.'),
    stockQuantity: z.coerce
      .number()
      .int('Stock quantity must be a whole number.')
      .min(0, 'Stock quantity must be zero or greater.'),
    unit: z.string().min(1, 'Unit is required.').max(20, 'Unit must not exceed 20 characters.'),
    weight: z.union([z.literal(''), z.coerce.number().gt(0, 'Weight must be greater than zero.')]),
    isOrganic: z.boolean(),
    isFeatured: z.boolean(),
    categoryId: z.string().min(1, 'Category is required.'),
    status: z.enum(['Draft', 'Active', 'OutOfStock', 'Discontinued', 'PendingApproval']),
    images: z.array(
      z.object({
        imageUrl: z.string().min(1, 'Image URL is required.').max(500, 'Image URL must not exceed 500 characters.'),
        altText: z.string().max(200, 'Alt text must not exceed 200 characters.'),
        displayOrder: z.coerce.number().int().min(0),
        isPrimary: z.boolean()
      })
    )
  })
  .refine(data => data.salePrice === '' || data.salePrice < data.price, {
    message: 'Sale price must be less than the regular price.',
    path: ['salePrice']
  })
  .refine(data => data.images.filter(image => image.isPrimary).length <= 1, {
    message: 'Only one image can be marked as primary.',
    path: ['images']
  })
  .refine(data => !data.isFeatured || data.images.length >= 1, {
    message: 'Featured products need at least one image.',
    path: ['isFeatured']
  })

// zod's coerce fields make the schema's input type (raw strings from the DOM) differ from its
// output type (parsed numbers) — react-hook-form needs both generics to bridge that.
type ProductFormInput = z.input<typeof productSchema>
type ProductFormValues = z.output<typeof productSchema>

const emptyValues: ProductFormInput = {
  name: '',
  description: '',
  shortDescription: '',
  price: '' as unknown as number,
  salePrice: '',
  sku: '',
  stockQuantity: 0,
  unit: '',
  weight: '',
  isOrganic: true,
  isFeatured: false,
  categoryId: '',
  status: 'Draft',
  images: []
}

interface ProductFormDialogProps {
  open: boolean
  productId: string | null // null = create mode
  isPending: boolean
  serverError: string | null
  onSubmit: (values: CreateProductRequest | UpdateProductRequest) => void
  onClose: () => void
}

const ProductFormDialog = ({ open, productId, isPending, serverError, onSubmit, onClose }: ProductFormDialogProps) => {
  const isEdit = productId !== null

  // Hooks
  const { data: product, isLoading: isProductLoading } = useProduct(open ? productId : null)
  const { data: categoryTree } = useCategories()

  const categoryOptions = flattenCategories(categoryTree ?? [])

  const {
    control,
    handleSubmit,
    reset,
    setValue,
    getValues,
    formState: { errors }
  } = useForm<ProductFormInput, unknown, ProductFormValues>({
    resolver: zodResolver(productSchema),
    defaultValues: emptyValues
  })

  const { fields, append, remove } = useFieldArray({ control, name: 'images' })

  useEffect(() => {
    if (!open) return

    if (!isEdit) {
      reset(emptyValues)

      return
    }

    // Edit mode: only seed the form from the FULL detail response — a summary row lacks
    // sku/stock/images, and images use replace-all PUT semantics.
    if (product) {
      reset({
        name: product.name,
        description: product.description ?? '',
        shortDescription: product.shortDescription ?? '',
        price: product.price,
        salePrice: product.salePrice ?? '',
        sku: product.sku,
        stockQuantity: product.stockQuantity,
        unit: product.unit,
        weight: product.weight ?? '',
        isOrganic: product.isOrganic,
        isFeatured: product.isFeatured,
        categoryId: product.categoryId,
        status: product.status,
        images: product.images.map(image => ({
          imageUrl: image.imageUrl,
          altText: image.altText ?? '',
          displayOrder: image.displayOrder,
          isPrimary: image.isPrimary
        }))
      })
    }
  }, [open, isEdit, product, reset])

  const handlePrimaryToggle = (index: number, checked: boolean) => {
    if (checked) {
      // Backend allows at most one primary image — flipping one on clears the others.
      getValues('images').forEach((_, i) => setValue(`images.${i}.isPrimary`, i === index))
    } else {
      setValue(`images.${index}.isPrimary`, false)
    }
  }

  const handleAddImage = () => {
    append({ imageUrl: '', altText: '', displayOrder: fields.length, isPrimary: fields.length === 0 })
  }

  const submitForm = (values: ProductFormValues) => {
    const base: CreateProductRequest = {
      name: values.name,
      description: values.description || null,
      shortDescription: values.shortDescription || null,
      price: values.price,
      salePrice: values.salePrice === '' ? null : values.salePrice,
      sku: values.sku,
      stockQuantity: values.stockQuantity,
      unit: values.unit,
      weight: values.weight === '' ? null : values.weight,
      isOrganic: values.isOrganic,
      isFeatured: values.isFeatured,
      categoryId: values.categoryId,

      // The form always renders the complete list, so always send it (replace-all on update).
      images: values.images.map(image => ({
        imageUrl: image.imageUrl,
        altText: image.altText || null,
        displayOrder: image.displayOrder,
        isPrimary: image.isPrimary
      }))
    }

    onSubmit(isEdit ? { ...base, status: values.status } : base)
  }

  const showLoader = isEdit && isProductLoading

  return (
    <Dialog open={open} onClose={onClose} maxWidth='md' fullWidth>
      <DialogTitle>{isEdit ? `Edit Product — ${product?.name ?? ''}` : 'New Product'}</DialogTitle>
      <form onSubmit={handleSubmit(submitForm)} noValidate>
        <DialogContent className='flex flex-col gap-5 pbs-2'>
          {serverError && <Alert severity='error'>{serverError}</Alert>}
          {showLoader ? (
            <div className='flex justify-center plb-12'>
              <CircularProgress />
            </div>
          ) : (
            <Grid container spacing={5}>
              <Grid item xs={12} sm={8}>
                <Controller
                  name='name'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label='Name'
                      error={!!errors.name}
                      helperText={errors.name?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <Controller
                  name='sku'
                  control={control}
                  render={({ field }) => (
                    <TextField {...field} fullWidth label='SKU' error={!!errors.sku} helperText={errors.sku?.message} />
                  )}
                />
              </Grid>
              <Grid item xs={12}>
                <Controller
                  name='shortDescription'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label='Short Description'
                      error={!!errors.shortDescription}
                      helperText={errors.shortDescription?.message}
                    />
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
                      multiline
                      minRows={3}
                      label='Description'
                      error={!!errors.description}
                      helperText={errors.description?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6} sm={3}>
                <Controller
                  name='price'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type='number'
                      label='Price'
                      error={!!errors.price}
                      helperText={errors.price?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6} sm={3}>
                <Controller
                  name='salePrice'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type='number'
                      label='Sale Price'
                      error={!!errors.salePrice}
                      helperText={errors.salePrice?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6} sm={3}>
                <Controller
                  name='stockQuantity'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type='number'
                      label='Stock Quantity'
                      error={!!errors.stockQuantity}
                      helperText={errors.stockQuantity?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6} sm={3}>
                <Controller
                  name='unit'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label='Unit'
                      placeholder='kg, piece, jar…'
                      error={!!errors.unit}
                      helperText={errors.unit?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6} sm={3}>
                <Controller
                  name='weight'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      type='number'
                      label='Weight (g)'
                      error={!!errors.weight}
                      helperText={errors.weight?.message}
                    />
                  )}
                />
              </Grid>
              <Grid item xs={6} sm={isEdit ? 3 : 5}>
                <Controller
                  name='categoryId'
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      select
                      fullWidth
                      label='Category'
                      error={!!errors.categoryId}
                      helperText={errors.categoryId?.message}
                    >
                      {categoryOptions.map(option => (
                        <MenuItem key={option.id} value={option.id}>
                          {' '.repeat(option.depth * 4) + option.name}
                        </MenuItem>
                      ))}
                    </TextField>
                  )}
                />
              </Grid>
              {isEdit && (
                <Grid item xs={6} sm={3}>
                  <Controller
                    name='status'
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} select fullWidth label='Status'>
                        {PRODUCT_STATUSES.map(status => (
                          <MenuItem key={status} value={status}>
                            {status}
                          </MenuItem>
                        ))}
                      </TextField>
                    )}
                  />
                </Grid>
              )}
              <Grid item xs={12} sm={isEdit ? 3 : 4} className='flex items-center gap-4'>
                <Controller
                  name='isOrganic'
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Switch checked={field.value} onChange={field.onChange} />}
                      label='Organic'
                    />
                  )}
                />
                <Controller
                  name='isFeatured'
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Switch checked={field.value} onChange={field.onChange} />}
                      label='Featured'
                    />
                  )}
                />
              </Grid>
              {errors.isFeatured && (
                <Grid item xs={12}>
                  <Alert severity='warning'>{errors.isFeatured.message}</Alert>
                </Grid>
              )}
              <Grid item xs={12}>
                <Divider />
                <div className='flex items-center justify-between mbs-4'>
                  <div>
                    <Typography variant='h6'>Images</Typography>
                    <Typography variant='caption' color='text.secondary'>
                      On save, this list replaces all existing images for the product.
                    </Typography>
                  </div>
                  <Button size='small' startIcon={<i className='ri-add-line' />} onClick={handleAddImage}>
                    Add Image
                  </Button>
                </div>
              </Grid>
              {fields.map((field, index) => (
                <Grid item xs={12} key={field.id}>
                  <div className='flex items-start gap-3 flex-wrap sm:flex-nowrap'>
                    <Controller
                      name={`images.${index}.imageUrl`}
                      control={control}
                      render={({ field: urlField }) => (
                        <TextField
                          {...urlField}
                          fullWidth
                          size='small'
                          label='Image URL'
                          error={!!errors.images?.[index]?.imageUrl}
                          helperText={errors.images?.[index]?.imageUrl?.message}
                        />
                      )}
                    />
                    <Controller
                      name={`images.${index}.altText`}
                      control={control}
                      render={({ field: altField }) => (
                        <TextField {...altField} size='small' label='Alt Text' className='is-[160px] shrink-0' />
                      )}
                    />
                    <Controller
                      name={`images.${index}.displayOrder`}
                      control={control}
                      render={({ field: orderField }) => (
                        <TextField {...orderField} size='small' type='number' label='Order' className='is-[80px] shrink-0' />
                      )}
                    />
                    <Controller
                      name={`images.${index}.isPrimary`}
                      control={control}
                      render={({ field: primaryField }) => (
                        <FormControlLabel
                          className='shrink-0'
                          control={
                            <Checkbox
                              checked={primaryField.value}
                              onChange={e => handlePrimaryToggle(index, e.target.checked)}
                            />
                          }
                          label='Primary'
                        />
                      )}
                    />
                    <IconButton size='small' color='error' onClick={() => remove(index)} className='shrink-0 mbs-1'>
                      <i className='ri-delete-bin-7-line' />
                    </IconButton>
                  </div>
                </Grid>
              ))}
              {errors.images?.root && (
                <Grid item xs={12}>
                  <Alert severity='warning'>{errors.images.root.message}</Alert>
                </Grid>
              )}
            </Grid>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} color='secondary' disabled={isPending}>
            Cancel
          </Button>
          <Button type='submit' variant='contained' disabled={isPending || showLoader}>
            {isPending ? 'Saving…' : isEdit ? 'Save Changes' : 'Create Product'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  )
}

export default ProductFormDialog
