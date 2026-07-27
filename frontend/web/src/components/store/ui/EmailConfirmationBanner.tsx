'use client'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook Imports
import { useProfile } from '@/hooks/api/useProfile'
import { useResendConfirmation } from '@/hooks/api/useAuthEmail'

// Lib Imports
import { ApiError } from '@/libs/api-client'

// Component Imports
import { useStoreToast } from '@/components/store/StoreToast'

// Shown to signed-in users who have not confirmed their email. Checkout, reviews, blog comments
// and vendor registration are gated on confirmation server-side, so this explains the block up
// front rather than letting the user discover it on submit.
const EmailConfirmationBanner = ({ className = '' }: { className?: string }) => {
  const { user } = useAuth()
  const { data: profile } = useProfile(!!user)
  const resend = useResendConfirmation()
  const { showToast } = useStoreToast()

  if (!user || !profile || profile.emailConfirmed) return null

  const onResend = async () => {
    try {
      await resend.mutateAsync(profile.email)
      showToast('Confirmation email sent. Check your inbox.', 'success')
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Could not send the email. Please try again.', 'error')
    }
  }

  return (
    <div className={`alert alert-warning d-flex flex-column flex-sm-row align-items-sm-center gap-3 ${className}`} role='alert'>
      <i className='fas fa-envelope-open-text' style={{ fontSize: 22 }} aria-hidden='true' />
      <div className='flex-grow-1'>
        <strong className='d-block'>Please confirm your email address</strong>
        <span style={{ fontSize: 14 }}>
          We sent a confirmation link to <strong>{profile.email}</strong>. Confirm it to place orders, post reviews and
          leave comments.
        </span>
      </div>
      <button
        type='button'
        className='btn custom_btn rounded-pill px-4 flex-shrink-0'
        disabled={resend.isPending}
        onClick={onResend}
      >
        {resend.isPending ? 'Sending…' : 'Resend email'}
      </button>
    </div>
  )
}

export default EmailConfirmationBanner
