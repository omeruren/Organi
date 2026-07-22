'use client'

// React Imports
import { useEffect, useState } from 'react'

// MUI Imports
import Dialog from '@mui/material/Dialog'
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import Checkbox from '@mui/material/Checkbox'
import FormControlLabel from '@mui/material/FormControlLabel'
import FormGroup from '@mui/material/FormGroup'
import Alert from '@mui/material/Alert'

// Type Imports
import type { UserResponse, AppRoleName } from '@/types/api/user'

// The three seeded system roles (RoleSeedData.cs) — the backend validates
// that every requested role exists, so this list must match the seed data.
const ALL_ROLES: AppRoleName[] = ['Admin', 'Vendor', 'Customer']

interface UserRolesDialogProps {
  open: boolean
  user: UserResponse | null
  isPending: boolean
  serverError: string | null
  onSubmit: (roleNames: string[]) => void
  onClose: () => void
}

const UserRolesDialog = ({ open, user, isPending, serverError, onSubmit, onClose }: UserRolesDialogProps) => {
  const [selected, setSelected] = useState<string[]>([])

  useEffect(() => {
    if (open) setSelected(user?.roles ?? [])
  }, [open, user])

  const toggleRole = (role: string) =>
    setSelected(prev => (prev.includes(role) ? prev.filter(r => r !== role) : [...prev, role]))

  return (
    <Dialog open={open} onClose={onClose} maxWidth='xs' fullWidth>
      <DialogTitle>Manage roles</DialogTitle>
      <DialogContent>
        <DialogContentText className='mbe-2'>
          {user ? `Roles for ${user.firstName} ${user.lastName} (${user.email})` : ''}
        </DialogContentText>
        <FormGroup>
          {ALL_ROLES.map(role => (
            <FormControlLabel
              key={role}
              control={<Checkbox checked={selected.includes(role)} onChange={() => toggleRole(role)} />}
              label={role}
            />
          ))}
        </FormGroup>
        {serverError && (
          <Alert severity='error' className='mbs-2'>
            {serverError}
          </Alert>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color='secondary' disabled={isPending}>
          Cancel
        </Button>
        <Button
          onClick={() => onSubmit(selected)}
          variant='contained'
          disabled={isPending || selected.length === 0}
        >
          {isPending ? 'Saving…' : 'Save Roles'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default UserRolesDialog
