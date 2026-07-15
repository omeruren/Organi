// MUI Imports
import { useTheme } from '@mui/material/styles'

// Third-party Imports
import PerfectScrollbar from 'react-perfect-scrollbar'

// Type Imports
import type { VerticalMenuContextProps } from '@menu/components/vertical-menu/Menu'

// Component Imports
import { Menu, MenuItem, MenuSection } from '@menu/vertical-menu'

// Hook Imports
import useVerticalNav from '@menu/hooks/useVerticalNav'

// Style Imports
import StyledVerticalNavExpandIcon from '@menu/styles/vertical/StyledVerticalNavExpandIcon'
import menuItemStyles from '@core/styles/vertical/menuItemStyles'
import menuSectionStyles from '@core/styles/vertical/menuSectionStyles'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Config Imports
import { navigationConfig } from '@/configs/navigation'
import type { AppRole } from '@/configs/navigation'

type RenderExpandIconProps = {
  open?: boolean
  transitionDuration?: VerticalMenuContextProps['transitionDuration']
}

const RenderExpandIcon = ({ open, transitionDuration }: RenderExpandIconProps) => (
  <StyledVerticalNavExpandIcon open={open} transitionDuration={transitionDuration}>
    <i className='ri-arrow-right-s-line' />
  </StyledVerticalNavExpandIcon>
)

const VerticalMenu = ({ scrollMenu }: { scrollMenu: (container: any, isPerfectScrollbar: boolean) => void }) => {
  // Hooks
  const theme = useTheme()
  const { isBreakpointReached, transitionDuration } = useVerticalNav()
  const { user } = useAuth()

  const ScrollWrapper = isBreakpointReached ? 'div' : PerfectScrollbar

  // A section renders only if at least one of its items is visible to the current user's roles —
  // ownership of *which* rows within a page (e.g. a Vendor's own orders) is still enforced by the
  // backend regardless of what's shown here. See ai/09-AI-Frontend-Skills.md §10.
  const hasAccess = (roles: AppRole[]) => roles.some(role => user?.roles.includes(role))

  return (
    // eslint-disable-next-line lines-around-comment
    /* Custom scrollbar instead of browser scroll, remove if you want browser scroll only */
    <ScrollWrapper
      {...(isBreakpointReached
        ? {
            className: 'bs-full overflow-y-auto overflow-x-hidden',
            onScroll: container => scrollMenu(container, false)
          }
        : {
            options: { wheelPropagation: false, suppressScrollX: true },
            onScrollY: container => scrollMenu(container, true)
          })}
    >
      <Menu
        menuItemStyles={menuItemStyles(theme)}
        renderExpandIcon={({ open }) => <RenderExpandIcon open={open} transitionDuration={transitionDuration} />}
        renderExpandedMenuItemIcon={{ icon: <i className='ri-circle-line' /> }}
        menuSectionStyles={menuSectionStyles(theme)}
      >
        {navigationConfig.map(section => {
          const visibleItems = section.items.filter(item => hasAccess(item.roles))

          if (visibleItems.length === 0) return null

          return (
            <MenuSection key={section.label} label={section.label}>
              {visibleItems.map(item => (
                <MenuItem key={item.href} href={item.href} icon={<i className={item.icon} />}>
                  {item.label}
                </MenuItem>
              ))}
            </MenuSection>
          )
        })}
      </Menu>
    </ScrollWrapper>
  )
}

export default VerticalMenu
