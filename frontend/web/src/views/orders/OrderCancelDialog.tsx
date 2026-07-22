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
import TextField from '@mui/material/TextField'

interface OrderCancelDialogProps {
  open: boolean
  isPending: boolean
  onConfirm: (reason: string | null) => void
  onCancel: () => void
}

const OrderCancelDialog = ({ open, isPending, onConfirm, onCancel }: OrderCancelDialogProps) => {
  const [reason, setReason] = useState('')

  useEffect(() => {
    if (open) setReason('')
  }, [open])

  return (
    <Dialog open={open} onClose={onCancel} maxWidth='xs' fullWidth>
      <DialogTitle>Cancel order?</DialogTitle>
      <DialogContent>
        <DialogContentText className='mbe-4'>
          The order will be cancelled. If it was already confirmed or shipped, its stock is restored.
        </DialogContentText>
        <TextField
          fullWidth
          multiline
          minRows={2}
          label='Reason (optional)'
          value={reason}
          onChange={e => setReason(e.target.value)}
          inputProps={{ maxLength: 500 }}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel} color='secondary' disabled={isPending}>
          Keep Order
        </Button>
        <Button
          onClick={() => onConfirm(reason.trim() || null)}
          color='error'
          variant='contained'
          disabled={isPending}
        >
          {isPending ? 'Working…' : 'Cancel Order'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default OrderCancelDialog
