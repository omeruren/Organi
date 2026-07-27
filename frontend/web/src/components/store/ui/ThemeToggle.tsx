'use client'

// React Imports
import { useEffect, useState } from 'react'

type Theme = 'light' | 'dark'

export const THEME_STORAGE_KEY = 'organi-theme'

const readStored = (): Theme | null => {
  try {
    const value = window.localStorage.getItem(THEME_STORAGE_KEY)

    return value === 'light' || value === 'dark' ? value : null
  } catch {
    // Private browsing can throw on localStorage access; fall back to the OS preference.
    return null
  }
}

const systemTheme = (): Theme =>
  window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'

const ThemeToggle = () => {
  // Rendered only after mount: the server has no way to know the visitor's stored
  // choice, so committing to an icon during SSR guarantees a hydration mismatch.
  const [theme, setTheme] = useState<Theme | null>(null)

  useEffect(() => {
    setTheme(readStored() ?? systemTheme())
  }, [])

  // Track the OS while the visitor has not made an explicit choice.
  useEffect(() => {
    const media = window.matchMedia('(prefers-color-scheme: dark)')

    const onChange = () => {
      if (readStored() === null) {
        const next = media.matches ? 'dark' : 'light'

        document.documentElement.setAttribute('data-theme', next)
        setTheme(next)
      }
    }

    media.addEventListener('change', onChange)

    return () => media.removeEventListener('change', onChange)
  }, [])

  const toggle = () => {
    const next: Theme = theme === 'dark' ? 'light' : 'dark'

    document.documentElement.setAttribute('data-theme', next)

    try {
      window.localStorage.setItem(THEME_STORAGE_KEY, next)
    } catch {
      // Preference just won't persist; the current page still switches.
    }

    setTheme(next)
  }

  const isDark = theme === 'dark'

  return (
    <button
      type='button'
      className='main_search_btn'
      onClick={toggle}
      aria-label={isDark ? 'Switch to light theme' : 'Switch to dark theme'}
      aria-pressed={isDark}
      title={isDark ? 'Light mode' : 'Dark mode'}
    >
      {/* Reserve the slot before mount so the header doesn't shift when the icon appears. */}
      {theme === null ? (
        <i className='far fa-sun' style={{ visibility: 'hidden' }} aria-hidden='true' />
      ) : (
        <i className={isDark ? 'fas fa-sun' : 'far fa-moon'} aria-hidden='true' />
      )}
    </button>
  )
}

export default ThemeToggle
