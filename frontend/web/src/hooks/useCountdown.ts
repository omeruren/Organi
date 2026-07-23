'use client'

// React Imports
import { useEffect, useState } from 'react'

export interface Countdown {
  days: number
  hours: number
  minutes: number
  seconds: number
  done: boolean
}

const compute = (target: number): Countdown => {
  const diff = Math.max(0, target - Date.now())
  const totalSeconds = Math.floor(diff / 1000)

  return {
    days: Math.floor(totalSeconds / 86400),
    hours: Math.floor((totalSeconds % 86400) / 3600),
    minutes: Math.floor((totalSeconds % 3600) / 60),
    seconds: totalSeconds % 60,
    done: diff === 0
  }
}

const ZERO: Countdown = { days: 0, hours: 0, minutes: 0, seconds: 0, done: false }

// Replaces the template's jQuery.countdown deal timers. Starts at zero so the SSR and first
// client render match (Date.now() differs between server and client → hydration mismatch);
// the real values fill in after mount.
export const useCountdown = (targetDate: string | number | Date): Countdown => {
  const target = new Date(targetDate).getTime()
  const [state, setState] = useState<Countdown>(ZERO)

  useEffect(() => {
    setState(compute(target))

    const id = setInterval(() => setState(compute(target)), 1000)

    return () => clearInterval(id)
  }, [target])

  return state
}
