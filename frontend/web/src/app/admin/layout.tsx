// Third-party Imports
import 'react-perfect-scrollbar/dist/css/styles.css'

// Style Imports — scoped to /admin/* only, so the MUI/Tailwind reset never loads on the storefront.
import '@/app/globals.css'

// Generated Icon CSS Imports (remix icons used by the admin)
import '@assets/iconify-icons/generated-icons.css'

// Type Imports
import type { ChildrenType } from '@core/types'

export const metadata = {
  title: 'Organi Admin',
  description: 'Admin dashboard for the Organi organic marketplace.'
}

const AdminLayout = ({ children }: ChildrenType) => {
  return <>{children}</>
}

export default AdminLayout
