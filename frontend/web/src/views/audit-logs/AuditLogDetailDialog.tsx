'use client'

// MUI Imports
import Dialog from '@mui/material/Dialog'
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import Grid from '@mui/material/Grid'
import Divider from '@mui/material/Divider'

// View Imports
import { auditActionColorMap } from '@views/audit-logs/auditAction'

// Type Imports
import type { AuditLogResponse } from '@/types/api/auditLog'

// Pretty-print a JSON string, falling back to the raw value if it isn't valid JSON.
const formatJson = (raw: string | null): string => {
  if (!raw) return '—'

  try {
    return JSON.stringify(JSON.parse(raw), null, 2)
  } catch {
    return raw
  }
}

const AuditLogDetailDialog = ({ log, onClose }: { log: AuditLogResponse | null; onClose: () => void }) => {
  return (
    <Dialog open={log !== null} onClose={onClose} maxWidth='md' fullWidth>
      {log && (
        <>
          <DialogTitle className='flex items-center gap-3'>
            <span>{log.entityName}</span>
            <Chip label={log.action} color={auditActionColorMap[log.action]} size='small' variant='tonal' />
          </DialogTitle>
          <DialogContent className='flex flex-col gap-5'>
            <Grid container spacing={3}>
              <Grid item xs={12} sm={6}>
                <Typography variant='caption' color='text.secondary'>
                  Entity ID
                </Typography>
                <Typography className='break-all'>{log.entityId}</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant='caption' color='text.secondary'>
                  Timestamp
                </Typography>
                <Typography>{new Date(log.timestamp).toLocaleString()}</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant='caption' color='text.secondary'>
                  User
                </Typography>
                <Typography>{log.userEmail ?? '—'}</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant='caption' color='text.secondary'>
                  IP Address
                </Typography>
                <Typography>{log.ipAddress ?? '—'}</Typography>
              </Grid>
            </Grid>
            <Divider />
            <div>
              <Typography variant='subtitle2' className='mbe-2'>
                Old Values
              </Typography>
              <pre className='overflow-auto rounded bg-actionHover p-3 text-sm'>{formatJson(log.oldValues)}</pre>
            </div>
            <div>
              <Typography variant='subtitle2' className='mbe-2'>
                New Values
              </Typography>
              <pre className='overflow-auto rounded bg-actionHover p-3 text-sm'>{formatJson(log.newValues)}</pre>
            </div>
          </DialogContent>
          <DialogActions>
            <Button onClick={onClose} color='secondary'>
              Close
            </Button>
          </DialogActions>
        </>
      )}
    </Dialog>
  )
}

export default AuditLogDetailDialog
