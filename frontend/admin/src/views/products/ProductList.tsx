'use client'

// React Imports
import { useEffect, useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Button from '@mui/material/Button'
import Chip from '@mui/material/Chip'
import Avatar from '@mui/material/Avatar'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Checkbox from '@mui/material/Checkbox'
import FormControlLabel from '@mui/material/FormControlLabel'
import Grid from '@mui/material/Grid'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'
import type { ChipProps } from '@mui/material/Chip'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'
import ProductFormDialog from '@views/products/ProductFormDialog'

// Hook Imports
import { useProducts, useCreateProduct, useUpdateProduct, useDeleteProduct } from '@/hooks/api/useProducts'
import { useCategories } from '@/hooks/api/useCategories'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Util Imports
import { flattenCategories } from '@/utils/categoryTree'

// Type Imports
import type {
  ProductSummaryResponse,
  ProductStatus,
  CreateProductRequest,
  UpdateProductRequest
} from '@/types/api/product'

const PRODUCT_STATUSES: ProductStatus[] = ['Draft', 'Active', 'OutOfStock', 'Discontinued', 'PendingApproval']

const statusColorMap: Record<ProductStatus, ChipProps['color']> = {
  Draft: 'default',
  Active: 'success',
  OutOfStock: 'warning',
  Discontinued: 'error',
  PendingApproval: 'info'
}

const ProductList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [status, setStatus] = useState('')
  const [organicOnly, setOrganicOnly] = useState(false)
  const [formOpen, setFormOpen] = useState(false)
  const [editingProductId, setEditingProductId] = useState<string | null>(null)
  const [deletingProduct, setDeletingProduct] = useState<ProductSummaryResponse | null>(null)
  const [serverError, setServerError] = useState<string | null>(null)

  // Hooks
  const { showToast } = useToast()
  const { user } = useAuth()

  // The backend does NOT auto-scope vendors to their own products — a vendor-only user
  // gets their list pre-filtered here; admins see everything.
  const isVendorOnly = (user?.roles.includes('Vendor') ?? false) && !(user?.roles.includes('Admin') ?? false)
  const canCreate = user?.vendorId != null

  // Debounce the search input so each keystroke doesn't hit the API.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setSearch(searchInput)
      setPaginationModel(prev => ({ ...prev, page: 0 }))
    }, 400)

    return () => clearTimeout(timeout)
  }, [searchInput])

  const { data, isLoading } = useProducts({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    search: search || undefined,
    categoryId: categoryId || undefined,
    status: status || undefined,
    isOrganic: organicOnly ? true : undefined,
    vendorId: isVendorOnly ? (user?.vendorId ?? undefined) : undefined
  })

  const { data: categoryTree } = useCategories()
  const categoryOptions = flattenCategories(categoryTree ?? [])

  const createProduct = useCreateProduct()
  const updateProduct = useUpdateProduct()
  const deleteProduct = useDeleteProduct()

  const isMutating = createProduct.isPending || updateProduct.isPending

  const resetToFirstPage = () => setPaginationModel(prev => ({ ...prev, page: 0 }))

  const handleOpenCreate = () => {
    setEditingProductId(null)
    setServerError(null)
    setFormOpen(true)
  }

  const handleOpenEdit = (product: ProductSummaryResponse) => {
    setEditingProductId(product.id)
    setServerError(null)
    setFormOpen(true)
  }

  const handleSubmit = async (values: CreateProductRequest | UpdateProductRequest) => {
    setServerError(null)

    try {
      if (editingProductId) {
        await updateProduct.mutateAsync({ id: editingProductId, request: values as UpdateProductRequest })
        showToast(`Product ${values.name} updated.`)
      } else {
        await createProduct.mutateAsync(values)
        showToast(`Product ${values.name} created as Draft.`)
      }

      setFormOpen(false)
    } catch (error) {
      setServerError(error instanceof ApiError ? error.message : 'Something went wrong. Please try again.')
    }
  }

  const handleDelete = async () => {
    if (!deletingProduct) return

    try {
      await deleteProduct.mutateAsync(deletingProduct.id)
      showToast(`Product ${deletingProduct.name} deleted.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to delete product.', 'error')
    } finally {
      setDeletingProduct(null)
    }
  }

  const columns: GridColDef<ProductSummaryResponse>[] = [
    {
      field: 'name',
      headerName: 'Product',
      flex: 1.5,
      minWidth: 220,
      sortable: false,
      renderCell: params => (
        <div className='flex items-center gap-3'>
          <Avatar variant='rounded' src={params.row.primaryImageUrl ?? undefined}>
            {params.row.name.charAt(0)}
          </Avatar>
          <div className='flex flex-col'>
            <Typography color='text.primary'>{params.row.name}</Typography>
            {params.row.isOrganic && (
              <Typography variant='caption' color='success.main'>
                <i className='ri-leaf-line text-sm' /> Organic
              </Typography>
            )}
          </div>
        </div>
      )
    },
    { field: 'categoryName', headerName: 'Category', flex: 1, minWidth: 130, sortable: false },
    { field: 'vendorName', headerName: 'Vendor', flex: 1, minWidth: 140, sortable: false },
    {
      field: 'price',
      headerName: 'Price',
      width: 140,
      sortable: false,
      renderCell: params =>
        params.row.salePrice != null ? (
          <div className='flex items-baseline gap-1'>
            <Typography color='text.primary'>${params.row.salePrice}</Typography>
            <Typography variant='caption' color='text.disabled' className='line-through'>
              ${params.row.price}
            </Typography>
            <Typography variant='caption'>/{params.row.unit}</Typography>
          </div>
        ) : (
          <Typography color='text.primary'>{`$${params.row.price}/${params.row.unit}`}</Typography>
        )
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 140,
      sortable: false,
      renderCell: params => (
        <Chip label={params.value} color={statusColorMap[params.row.status]} size='small' variant='tonal' />
      )
    },
    {
      field: 'averageRating',
      headerName: 'Rating',
      width: 100,
      sortable: false,
      renderCell: params => (
        <div className='flex items-center gap-1'>
          <i className='ri-star-fill text-warning text-base' />
          <Typography>{params.row.averageRating.toFixed(1)}</Typography>
        </div>
      )
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: '',
      width: 100,
      getActions: (params: GridRowParams<ProductSummaryResponse>) => [
        <GridActionsCellItem
          key='edit'
          icon={<i className='ri-pencil-line' />}
          label='Edit'
          onClick={() => handleOpenEdit(params.row)}
        />,
        <GridActionsCellItem
          key='delete'
          icon={<i className='ri-delete-bin-7-line' />}
          label='Delete'
          onClick={() => setDeletingProduct(params.row)}
        />
      ]
    }
  ]

  return (
    <Card>
      <CardHeader
        title='Products'
        action={
          canCreate ? (
            <Button variant='contained' startIcon={<i className='ri-add-line' />} onClick={handleOpenCreate}>
              New Product
            </Button>
          ) : (
            <Typography variant='caption' color='text.secondary'>
              Products are created by vendor accounts.
            </Typography>
          )
        }
      />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              size='small'
              label='Search'
              placeholder='Name or description…'
              value={searchInput}
              onChange={e => setSearchInput(e.target.value)}
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              size='small'
              label='Category'
              value={categoryId}
              onChange={e => {
                setCategoryId(e.target.value)
                resetToFirstPage()
              }}
            >
              <MenuItem value=''>All Categories</MenuItem>
              {categoryOptions.map(option => (
                <MenuItem key={option.id} value={option.id}>
                  {' '.repeat(option.depth * 4) + option.name}
                </MenuItem>
              ))}
            </TextField>
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              size='small'
              label='Status'
              value={status}
              onChange={e => {
                setStatus(e.target.value)
                resetToFirstPage()
              }}
            >
              <MenuItem value=''>All Statuses</MenuItem>
              {PRODUCT_STATUSES.map(option => (
                <MenuItem key={option} value={option}>
                  {option}
                </MenuItem>
              ))}
            </TextField>
          </Grid>
          <Grid item xs={12} sm={2} className='flex items-center'>
            <FormControlLabel
              control={
                <Checkbox
                  checked={organicOnly}
                  onChange={e => {
                    setOrganicOnly(e.target.checked)
                    resetToFirstPage()
                  }}
                />
              }
              label='Organic only'
            />
          </Grid>
        </Grid>
      </CardContent>
      <DataGrid
        rows={data?.items ?? []}
        columns={columns}
        loading={isLoading}
        autoHeight
        rowHeight={64}
        paginationMode='server'
        rowCount={data?.totalCount ?? 0}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
        pageSizeOptions={[10, 25, 50]}
        disableRowSelectionOnClick
        disableColumnMenu
      />
      <ProductFormDialog
        open={formOpen}
        productId={editingProductId}
        isPending={isMutating}
        serverError={serverError}
        onSubmit={handleSubmit}
        onClose={() => setFormOpen(false)}
      />
      <ConfirmDialog
        open={deletingProduct !== null}
        title='Delete product?'
        description={`Product "${deletingProduct?.name}" will be deleted. Products with pending orders cannot be deleted.`}
        isPending={deleteProduct.isPending}
        onConfirm={handleDelete}
        onCancel={() => setDeletingProduct(null)}
      />
    </Card>
  )
}

export default ProductList
