'use client'

// Next Imports
import { useRouter, usePathname } from 'next/navigation'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'
import { useStoreToast } from '@/components/store/StoreToast'

// Hook / Lib Imports
import { useAddToCart } from '@/hooks/api/useCart'
import { ApiError } from '@/libs/api-client'

// Shared add-to-cart action used by ProductCard, the PDP, and the wishlist — auth-gated
// (redirects to login when signed out) with storefront toast feedback.
export const useCartActions = () => {
  const { user } = useAuth()
  const router = useRouter()
  const pathname = usePathname()
  const { showToast } = useStoreToast()
  const addMutation = useAddToCart()

  const addToCart = async (productId: string, quantity = 1, productName?: string) => {
    if (!user) {
      router.push(`/login?redirectTo=${encodeURIComponent(pathname)}`)

      return
    }

    try {
      await addMutation.mutateAsync({ productId, quantity })
      showToast(`${productName ?? 'Product'} added to cart.`)
    } catch (error) {
      showToast(error instanceof ApiError ? error.message : 'Could not add to cart.', 'error')
    }
  }

  return { addToCart, isAdding: addMutation.isPending }
}
