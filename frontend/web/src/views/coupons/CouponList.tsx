'use client'

// React Imports
import { useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import Button from '@mui/material/Button'
import Chip from '@mui/material/Chip'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'
import CouponFormDialog from '@views/coupons/CouponFormDialog'

// Hook Imports
import { useCoupons, useCreateCoupon, useUpdateCoupon, useDeleteCoupon } from '@/hooks/api/useCoupons'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Type Imports
import type { CouponResponse, CreateCouponRequest, UpdateCouponRequest } from '@/types/api/coupon'

const formatDate = (iso: string) => new Date(iso).toLocaleDateString()

const CouponList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [formOpen, setFormOpen] = useState(false)
  const [editingCoupon, setEditingCoupon] = useState<CouponResponse | null>(null)
  const [deletingCoupon, setDeletingCoupon] = useState<CouponResponse | null>(null)
  const [serverError, setServerError] = useState<string | null>(null)

  // Hooks
  const { showToast } = useToast()

  // Backend pages are 1-indexed, DataGrid is 0-indexed — convert at this boundary only.
  const { data, isLoading } = useCoupons({ page: paginationModel.page + 1, pageSize: paginationModel.pageSize })

  const createCoupon = useCreateCoupon()
  const updateCoupon = useUpdateCoupon()
  const deleteCoupon = useDeleteCoupon()

  const isMutating = createCoupon.isPending || updateCoupon.isPending

  const handleOpenCreate = () => {
    setEditingCoupon(null)
    setServerError(null)
    setFormOpen(true)
  }

  const handleOpenEdit = (coupon: CouponResponse) => {
    setEditingCoupon(coupon)
    setServerError(null)
    setFormOpen(true)
  }

  const handleSubmit = async (values: CreateCouponRequest | UpdateCouponRequest) => {
    setServerError(null)

    try {
      if (editingCoupon) {
        await updateCoupon.mutateAsync({ id: editingCoupon.id, request: values as UpdateCouponRequest })
        showToast(`Coupon ${values.code} updated.`)
      } else {
        await createCoupon.mutateAsync(values)
        showToast(`Coupon ${values.code} created.`)
      }

      setFormOpen(false)
    } catch (error) {
      // 409 business-rule conflicts (e.g. duplicate code) surface inside the dialog; the backend's
      // detail messages are written to be user-facing. Validation 400s shouldn't happen since the
      // zod schema mirrors the backend validator, but the same path covers them if they do.
      setServerError(error instanceof ApiError ? error.message : 'Something went wrong. Please try again.')
    }
  }

  const handleDelete = async () => {
    if (!deletingCoupon) return

    try {
      await deleteCoupon.mutateAsync(deletingCoupon.id)
      showToast(`Coupon ${deletingCoupon.code} deleted.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to delete coupon.', 'error')
    } finally {
      setDeletingCoupon(null)
    }
  }

  const columns: GridColDef<CouponResponse>[] = [
    { field: 'code', headerName: 'Code', flex: 1, minWidth: 140 },
    {
      field: 'discountValue',
      headerName: 'Discount',
      width: 130,
      valueGetter: params =>
        params.row.discountType === 'Percentage' ? `${params.row.discountValue}%` : `$${params.row.discountValue}`
    },
    {
      field: 'usage',
      headerName: 'Usage',
      width: 110,
      sortable: false,
      valueGetter: params => `${params.row.currentUsageCount}${params.row.maxUsageCount ? ` / ${params.row.maxUsageCount}` : ''}`
    },
    { field: 'startDate', headerName: 'Starts', width: 120, valueFormatter: params => formatDate(params.value) },
    { field: 'endDate', headerName: 'Ends', width: 120, valueFormatter: params => formatDate(params.value) },
    {
      field: 'isActive',
      headerName: 'Status',
      width: 110,
      renderCell: params => (
        <Chip
          label={params.value ? 'Active' : 'Inactive'}
          color={params.value ? 'success' : 'default'}
          size='small'
          variant='tonal'
        />
      )
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: '',
      width: 100,
      getActions: (params: GridRowParams<CouponResponse>) => [
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
          onClick={() => setDeletingCoupon(params.row)}
        />
      ]
    }
  ]

  return (
    <Card>
      <CardHeader
        title='Coupons'
        action={
          <Button variant='contained' startIcon={<i className='ri-add-line' />} onClick={handleOpenCreate}>
            New Coupon
          </Button>
        }
      />
      <DataGrid
        rows={data?.items ?? []}
        columns={columns}
        loading={isLoading}
        autoHeight
        paginationMode='server'
        rowCount={data?.totalCount ?? 0}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
        pageSizeOptions={[10, 25, 50]}
        disableRowSelectionOnClick
        disableColumnMenu
      />
      <CouponFormDialog
        open={formOpen}
        coupon={editingCoupon}
        isPending={isMutating}
        serverError={serverError}
        onSubmit={handleSubmit}
        onClose={() => setFormOpen(false)}
      />
      <ConfirmDialog
        open={deletingCoupon !== null}
        title='Delete coupon?'
        description={`Coupon "${deletingCoupon?.code}" will be deleted. This action cannot be undone from this screen.`}
        isPending={deleteCoupon.isPending}
        onConfirm={handleDelete}
        onCancel={() => setDeletingCoupon(null)}
      />
    </Card>
  )
}

export default CouponList
