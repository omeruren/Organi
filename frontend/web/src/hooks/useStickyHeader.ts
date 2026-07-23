'use client'

// React Imports
import { useEffect, useState } from 'react'

// Replaces the template's jQuery scroll handler that adds `.sticky` to the header past a threshold.
export const useStickyHeader = (threshold = 122) => {
  const [isSticky, setIsSticky] = useState(false)

  useEffect(() => {
    const onScroll = () => setIsSticky(window.scrollY > threshold)

    onScroll()
    window.addEventListener('scroll', onScroll, { passive: true })

    return () => window.removeEventListener('scroll', onScroll)
  }, [threshold])

  return isSticky
}
