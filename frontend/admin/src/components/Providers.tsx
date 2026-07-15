// Type Imports
import type { ChildrenType, Direction } from '@core/types'

// Context Imports
import { VerticalNavProvider } from '@menu/contexts/verticalNavContext'
import { SettingsProvider } from '@core/contexts/settingsContext'
import ThemeProvider from '@components/theme'

// Lib Imports
import QueryProvider from '@/libs/QueryProvider'

// Context Imports
import { AuthProvider } from '@/contexts/AuthContext'
import { ToastProvider } from '@components/ToastProvider'

// Util Imports
import { getMode, getSettingsFromCookie } from '@core/utils/serverHelpers'

type Props = ChildrenType & {
  direction: Direction
}

const Providers = (props: Props) => {
  // Props
  const { children, direction } = props

  // Vars
  const mode = getMode()
  const settingsCookie = getSettingsFromCookie()

  return (
    <QueryProvider>
      <AuthProvider>
        <VerticalNavProvider>
          <SettingsProvider settingsCookie={settingsCookie} mode={mode}>
            <ThemeProvider direction={direction}>
            <ToastProvider>{children}</ToastProvider>
          </ThemeProvider>
          </SettingsProvider>
        </VerticalNavProvider>
      </AuthProvider>
    </QueryProvider>
  )
}

export default Providers
