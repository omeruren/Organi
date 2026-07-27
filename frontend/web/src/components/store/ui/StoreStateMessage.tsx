// Next Imports
import Link from 'next/link'

interface StoreStateMessageProps {
  icon?: string
  title?: string
  message: string
  cta?: { href: string; label: string }
  onRetry?: () => void
}

// Shared centered state block for storefront loading/empty/error states, so a failed fetch
// reads as an error (with a retry) rather than masquerading as an empty result.
const StoreStateMessage = ({ icon, title, message, cta, onRetry }: StoreStateMessageProps) => (
  <div className='text-center py-5'>
    {icon && (
      <div style={{ fontSize: 40, color: 'var(--organi-brand-ink)' }} className='mb-3'>
        <i className={icon} aria-hidden='true' />
      </div>
    )}
    {title && (
      <h5 style={{ fontWeight: 800 }} className='mb-2'>
        {title}
      </h5>
    )}
    <p style={{ color: 'var(--organi-text-muted)' }}>{message}</p>
    {cta && (
      <Link href={cta.href} className='btn custom_btn rounded-pill px-4'>
        {cta.label}
      </Link>
    )}
    {onRetry && (
      <button type='button' className='btn rounded-pill px-4 border ms-2' onClick={onRetry}>
        Try Again
      </button>
    )}
  </div>
)

export default StoreStateMessage
