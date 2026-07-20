'use client'

// React Imports
import { useEffect, useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Grid from '@mui/material/Grid'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'

// Component Imports
import ConfirmDialog from '@components/ConfirmDialog'
import { useToast } from '@components/ToastProvider'

// Hook Imports
import { useUsers, useActivateUser, useDeactivateUser, useAssignUserRoles } from '@/hooks/api/useUsers'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// View Imports
import UserRolesDialog from '@views/users/UserRolesDialog'

// Type Imports
import type { UserResponse } from '@/types/api/user'

const UserList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [activeFilter, setActiveFilter] = useState('')
  const [deactivatingUser, setDeactivatingUser] = useState<UserResponse | null>(null)
  const [rolesUser, setRolesUser] = useState<UserResponse | null>(null)
  const [rolesError, setRolesError] = useState<string | null>(null)

  // Hooks
  const { showToast } = useToast()

  // Debounce the search input so each keystroke doesn't hit the API.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setSearch(searchInput)
      setPaginationModel(prev => ({ ...prev, page: 0 }))
    }, 400)

    return () => clearTimeout(timeout)
  }, [searchInput])

  const { data, isLoading } = useUsers({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    search: search || undefined,
    isActive: activeFilter === '' ? undefined : activeFilter === 'active'
  })

  const activateUser = useActivateUser()
  const deactivateUser = useDeactivateUser()
  const assignRoles = useAssignUserRoles()

  const resetToFirstPage = () => setPaginationModel(prev => ({ ...prev, page: 0 }))

  const handleActivate = async (user: UserResponse) => {
    try {
      await activateUser.mutateAsync(user.id)
      showToast(`User ${user.email} activated.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to activate user.', 'error')
    }
  }

  const handleDeactivate = async () => {
    if (!deactivatingUser) return

    try {
      await deactivateUser.mutateAsync(deactivatingUser.id)
      showToast(`User ${deactivatingUser.email} deactivated.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Failed to deactivate user.', 'error')
    } finally {
      setDeactivatingUser(null)
    }
  }

  const handleSaveRoles = async (roleNames: string[]) => {
    if (!rolesUser) return

    setRolesError(null)

    try {
      await assignRoles.mutateAsync({ id: rolesUser.id, roleNames })
      showToast(`Roles updated for ${rolesUser.email}.`)
      setRolesUser(null)
    } catch (error) {
      setRolesError(error instanceof ApiError ? error.message : 'Something went wrong. Please try again.')
    }
  }

  const columns: GridColDef<UserResponse>[] = [
    {
      field: 'name',
      headerName: 'User',
      flex: 1.5,
      minWidth: 220,
      sortable: false,
      renderCell: params => (
        <div className='flex flex-col'>
          <Typography color='text.primary'>{`${params.row.firstName} ${params.row.lastName}`}</Typography>
          <Typography variant='caption' color='text.secondary'>
            {params.row.email}
          </Typography>
        </div>
      )
    },
    {
      field: 'roles',
      headerName: 'Roles',
      flex: 1,
      minWidth: 180,
      sortable: false,
      renderCell: params => (
        <div className='flex flex-wrap gap-1'>
          {params.row.roles.map(role => (
            <Chip key={role} label={role} size='small' variant='tonal' color={role === 'Admin' ? 'primary' : 'default'} />
          ))}
        </div>
      )
    },
    {
      field: 'lastLoginAt',
      headerName: 'Last Login',
      width: 130,
      sortable: false,
      renderCell: params => (
        <Typography>
          {params.row.lastLoginAt ? new Date(params.row.lastLoginAt).toLocaleDateString() : '—'}
        </Typography>
      )
    },
    {
      field: 'createdAt',
      headerName: 'Joined',
      width: 130,
      sortable: false,
      renderCell: params => <Typography>{new Date(params.row.createdAt).toLocaleDateString()}</Typography>
    },
    {
      field: 'isActive',
      headerName: 'Status',
      width: 110,
      sortable: false,
      renderCell: params => (
        <Chip
          label={params.row.isActive ? 'Active' : 'Inactive'}
          color={params.row.isActive ? 'success' : 'default'}
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
      getActions: (params: GridRowParams<UserResponse>) => [
        <GridActionsCellItem
          key='roles'
          icon={<i className='ri-shield-user-line' />}
          label='Manage roles'
          onClick={() => {
            setRolesError(null)
            setRolesUser(params.row)
          }}
        />,
        params.row.isActive ? (
          <GridActionsCellItem
            key='deactivate'
            icon={<i className='ri-user-forbid-line' />}
            label='Deactivate'
            onClick={() => setDeactivatingUser(params.row)}
          />
        ) : (
          <GridActionsCellItem
            key='activate'
            icon={<i className='ri-user-follow-line' />}
            label='Activate'
            onClick={() => handleActivate(params.row)}
          />
        )
      ]
    }
  ]

  return (
    <Card>
      <CardHeader title='Users' />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              size='small'
              label='Search'
              placeholder='Email or name…'
              value={searchInput}
              onChange={e => setSearchInput(e.target.value)}
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              size='small'
              label='Status'
              value={activeFilter}
              onChange={e => {
                setActiveFilter(e.target.value)
                resetToFirstPage()
              }}
            >
              <MenuItem value=''>All Users</MenuItem>
              <MenuItem value='active'>Active</MenuItem>
              <MenuItem value='inactive'>Inactive</MenuItem>
            </TextField>
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
      <UserRolesDialog
        open={rolesUser !== null}
        user={rolesUser}
        isPending={assignRoles.isPending}
        serverError={rolesError}
        onSubmit={handleSaveRoles}
        onClose={() => setRolesUser(null)}
      />
      <ConfirmDialog
        open={deactivatingUser !== null}
        title='Deactivate user?'
        description={`User "${deactivatingUser?.email}" will no longer be able to sign in.`}
        confirmLabel='Deactivate'
        isPending={deactivateUser.isPending}
        onConfirm={handleDeactivate}
        onCancel={() => setDeactivatingUser(null)}
      />
    </Card>
  )
}

export default UserList
