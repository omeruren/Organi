'use client'

// React Imports
import { createContext, useCallback, useContext, useState } from 'react'
import type { ReactNode } from 'react'

type ToastType = 'success' | 'error' | 'info'

interface Toast {
  id: number
  message: string
  type: ToastType
}

interface StoreToastValue {
  showToast: (message: string, type?: ToastType) => void
}

const StoreToastContext = createContext<StoreToastValue | null>(null)

const COLORS: Record<ToastType, string> = {
  success: 'var(--organi-brand)',
  error: 'var(--organi-danger)',
  info: 'var(--organi-text)'
}

// Lightweight, MUI-free toast for the storefront (the admin's ToastProvider is MUI-based).
export const StoreToastProvider = ({ children }: { children: ReactNode }) => {
  const [toasts, setToasts] = useState<Toast[]>([])

  const showToast = useCallback((message: string, type: ToastType = 'success') => {
    const id = Date.now() + Math.random()

    setToasts(prev => [...prev, { id, message, type }])
    window.setTimeout(() => setToasts(prev => prev.filter(t => t.id !== id)), 3000)
  }, [])

  return (
    <StoreToastContext.Provider value={{ showToast }}>
      {children}
      <div
        style={{
          position: 'fixed',
          bottom: 24,
          right: 24,
          zIndex: 2000,
          display: 'flex',
          flexDirection: 'column',
          gap: 10
        }}
      >
        {toasts.map(toast => (
          <div
            key={toast.id}
            style={{
              background: COLORS[toast.type],
              color: '#fff',
              padding: '12px 20px',
              borderRadius: 10,
              boxShadow: '0 8px 24px rgba(0,0,0,.18)',
              fontWeight: 600,
              maxWidth: 320,
              fontFamily: 'Roboto, sans-serif'
            }}
          >
            {toast.message}
          </div>
        ))}
      </div>
    </StoreToastContext.Provider>
  )
}

export const useStoreToast = (): StoreToastValue => {
  const context = useContext(StoreToastContext)

  if (!context) throw new Error('useStoreToast must be used within a StoreToastProvider')

  return context
}
