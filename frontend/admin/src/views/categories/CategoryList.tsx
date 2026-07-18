'use client'

// React Imports
import { useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import Button from '@mui/material/Button'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridRowParams } from '@mui/x-data-grid'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'
import CategoryFormDialog from '@views/categories/CategoryFormDialog'

// Hook Imports
import { useCategories, useCreateCategory, useUpdateCategory, useDeleteCategory } from '@/hooks/api/useCategories'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Util Imports
import { flattenCategories } from '@/utils/categoryTree'
import type { FlattenedCategory } from '@/utils/categoryTree'

// Type Imports
import type { CategoryResponse, CreateCategoryRequest, UpdateCategoryRequest } from '@/types/api/category'

const CategoryList = () => {
  // States
  const [formOpen, setFormOpen] = useState(false)
  const [editingCategory, setEditingCategory] = useState<CategoryResponse | null>(null)
  const [deletingCategory, setDeletingCategory] = useState<CategoryResponse | null>(null)
  const [serverError, setServerError] = useState<string | null>(null)

  // Hooks
  const { showToast } = useToast()
  const { data, isLoading } = useCategories()

  const createCategory = useCreateCategory()
  const updateCategory = useUpdateCategory()
  const deleteCategory = useDeleteCategory()

  const isMutating = createCategory.isPending || updateCategory.isPending

  // Row order encodes the hierarchy (parents directly above their children),
  // which is why sorting and pagination are disabled on this grid.
  const rows = flattenCategories(data ?? [])

  const handleOpenCreate = () => {
    setEditingCategory(null)
    setServerError(null)
    setFormOpen(true)
  }

  const handleOpenEdit = (category: CategoryResponse) => {
    setEditingCategory(category)
    setServerError(null)
    setFormOpen(true)
  }

  const handleSubmit = async (values: CreateCategoryRequest | UpdateCategoryRequest) => {
    setServerError(null)

    try {
      if (editingCategory) {
        await updateCategory.mutateAsync({ id: editingCategory.id, request: values as UpdateCategoryRequest })
        showToast(`Category ${values.name} updated.`)
      } else {
        await createCategory.mutateAsync(values)
        showToast(`Category ${values.name} created.`)
      }

      setFormOpen(false)
    } catch (error) {
      setServerError(error instanceof ApiError ? error.message : 'Something went wrong. Please try again.')
    }
  }

  const handleDelete = async () => {
    if (!deletingCategory) return

    try {
      await deleteCategory.mutateAsync(deletingCategory.id)
      showToast(`Category ${deletingCategory.name} deleted.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to delete category.', 'error')
    } finally {
      setDeletingCategory(null)
    }
  }

  const columns: GridColDef<FlattenedCategory>[] = [
    {
      field: 'name',
      headerName: 'Name',
      flex: 1,
      minWidth: 220,
      sortable: false,
      renderCell: params => (
        <Typography style={{ paddingInlineStart: params.row.depth * 24 }} color='text.primary'>
          {params.row.depth > 0 && <i className='ri-corner-down-right-line text-base mie-1' />}
          {params.row.name}
        </Typography>
      )
    },
    { field: 'slug', headerName: 'Slug', flex: 1, minWidth: 160, sortable: false },
    { field: 'displayOrder', headerName: 'Order', width: 90, sortable: false },
    {
      field: 'isActive',
      headerName: 'Status',
      width: 110,
      sortable: false,
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
      getActions: (params: GridRowParams<FlattenedCategory>) => [
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
          onClick={() => setDeletingCategory(params.row)}
        />
      ]
    }
  ]

  return (
    <Card>
      <CardHeader
        title='Categories'
        action={
          <Button variant='contained' startIcon={<i className='ri-add-line' />} onClick={handleOpenCreate}>
            New Category
          </Button>
        }
      />
      <DataGrid
        rows={rows}
        columns={columns}
        loading={isLoading}
        autoHeight
        hideFooter
        disableRowSelectionOnClick
        disableColumnMenu
      />
      <CategoryFormDialog
        open={formOpen}
        category={editingCategory}
        categories={rows}
        isPending={isMutating}
        serverError={serverError}
        onSubmit={handleSubmit}
        onClose={() => setFormOpen(false)}
      />
      <ConfirmDialog
        open={deletingCategory !== null}
        title='Delete category?'
        description={`Category "${deletingCategory?.name}" will be deleted. Categories with products or child categories cannot be deleted.`}
        isPending={deleteCategory.isPending}
        onConfirm={handleDelete}
        onCancel={() => setDeletingCategory(null)}
      />
    </Card>
  )
}

export default CategoryList
