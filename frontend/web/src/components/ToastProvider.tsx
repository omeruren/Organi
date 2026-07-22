'use client'

// React Imports
import { createContext, useCallback, useContext, useState } from 'react'
import type { ReactNode } from 'react'

// MUI Imports
import Snackbar from '@mui/material/Snackbar'
import Alert from '@mui/material/Alert'
import type { AlertColor } from '@mui/material/Alert'

interface ToastState {
  open: boolean
  message: string
  severity: AlertColor
}

interface ToastContextValue {
  showToast: (message: string, severity?: AlertColor) => void
}

const ToastContext = createContext<ToastContextValue | null>(null)

export const ToastProvider = ({ children }: { children: ReactNode }) => {
  const [toast, setToast] = useState<ToastState>({ open: false, message: '', severity: 'success' })

  const showToast = useCallback((message: string, severity: AlertColor = 'success') => {
    setToast({ open: true, message, severity })
  }, [])

  const handleClose = () => setToast(prev => ({ ...prev, open: false }))

  return (
    <ToastContext.Provider value={{ showToast }}>
      {children}
      <Snackbar
        open={toast.open}
        autoHideDuration={4000}
        onClose={handleClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert onClose={handleClose} severity={toast.severity} variant='filled'>
          {toast.message}
        </Alert>
      </Snackbar>
    </ToastContext.Provider>
  )
}

export const useToast = (): ToastContextValue => {
  const context = useContext(ToastContext)

  if (!context) throw new Error('useToast must be used within a ToastProvider')

  return context
}
