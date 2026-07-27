'use client'

// React Imports
import { useEffect, useRef, useState } from 'react'

// Next Imports
import Link from 'next/link'

// Third-party Imports
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

// Context Imports
import { useAuth } from '@/contexts/AuthContext'

// Hook / Lib Imports
import { useCart } from '@/hooks/api/useCart'
import { useCreateOrder } from '@/hooks/api/useCheckout'
import { useProfile } from '@/hooks/api/useProfile'
import { ApiError, EMAIL_NOT_CONFIRMED } from '@/libs/api-client'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'
import EmailConfirmationBanner from '@/components/store/ui/EmailConfirmationBanner'

// Type Imports
import type { OrderResponse } from '@/types/api/order'

// Mirrors CreateOrderValidator (Features/Orders/Commands/CreateOrder/CreateOrderValidator.cs)
const schema = z.object({
  shippingFirstName: z.string().min(1, 'First name is required').max(100),
  shippingLastName: z.string().min(1, 'Last name is required').max(100),
  shippingAddress: z.string().min(1, 'Address is required').max(500),
  shippingCity: z.string().min(1, 'City is required').max(100),
  shippingPostalCode: z.string().max(20),
  shippingPhone: z.string().min(1, 'Phone is required').max(20),
  shippingEmail: z.string().min(1, 'Email is required').email('Enter a valid email').max(256),
  notes: z.string().max(500),
  couponCode: z.string()
})

type CheckoutForm = z.infer<typeof schema>

const CheckoutView = () => {
  const { user, isLoading: authLoading } = useAuth()
  const { data: cart, isLoading: cartLoading } = useCart(!!user)
  const { data: profile } = useProfile(!!user)
  const createOrder = useCreateOrder()

  // Checkout is gated on a confirmed email server-side (RequireConfirmedEmailFilter), so surface
  // it before the user fills the whole form rather than failing them on submit.
  const emailUnconfirmed = profile != null && !profile.emailConfirmed

  const [formError, setFormError] = useState<string | null>(null)
  const [placed, setPlaced] = useState<OrderResponse | null>(null)

  const [firstName, ...rest] = (user?.name ?? '').split(' ')

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors, isSubmitting }
  } = useForm<CheckoutForm>({
    resolver: zodResolver(schema),
    defaultValues: {
      shippingFirstName: firstName ?? '',
      shippingLastName: rest.join(' '),
      shippingAddress: '',
      shippingCity: '',
      shippingPostalCode: '',
      shippingPhone: '',
      shippingEmail: user?.email ?? '',
      notes: '',
      couponCode: ''
    }
  })

  // Auth resolves after the form initializes, so prefill name/email once the user is available.
  const prefilled = useRef(false)

  useEffect(() => {
    if (!user || prefilled.current) return
    prefilled.current = true

    const [first, ...last] = (user.name ?? '').split(' ')

    if (first) setValue('shippingFirstName', first)
    if (last.length) setValue('shippingLastName', last.join(' '))
    if (user.email) setValue('shippingEmail', user.email)
  }, [user, setValue])

  const onSubmit = async (values: CheckoutForm) => {
    setFormError(null)

    try {
      const order = await createOrder.mutateAsync({
        shippingFirstName: values.shippingFirstName,
        shippingLastName: values.shippingLastName,
        shippingAddress: values.shippingAddress,
        shippingCity: values.shippingCity,
        shippingPostalCode: values.shippingPostalCode || null,
        shippingPhone: values.shippingPhone,
        shippingEmail: values.shippingEmail,
        notes: values.notes || null,
        couponCode: values.couponCode || null
      })

      setPlaced(order)
    } catch (error) {
      // The banner above already explains the unconfirmed case; keep the inline error generic so
      // the two don't contradict each other.
      if (error instanceof ApiError && error.code === EMAIL_NOT_CONFIRMED) {
        setFormError('Please confirm your email address before placing an order — see the notice above.')

        return
      }

      setFormError(error instanceof ApiError ? error.message : 'Could not place your order. Please try again.')
    }
  }

  // Order confirmation
  if (placed) {
    return (
      <>
        <Breadcrumb title='Order Confirmed' items={[{ label: 'Checkout' }]} />
        <section className='sec_space_large'>
          <div className='container'>
            <div className='row justify-content-center'>
              <div className='col-lg-7 text-center'>
                <div className='bg-white rounded-4 shadow-sm p-5'>
                  <div style={{ fontSize: 48, color: 'var(--organi-brand-ink)' }}>
                    <i className='fas fa-check-circle' />
                  </div>
                  <h3 className='mt-3' style={{ fontWeight: 800 }}>
                    Thank you for your order!
                  </h3>
                  <p style={{ color: 'var(--organi-text-muted)' }}>
                    Your order <strong>{placed.orderNumber}</strong> has been placed and is <strong>{placed.status}</strong>.
                  </p>
                  <p style={{ color: 'var(--organi-text-muted)' }}>
                    A confirmation email is on its way to <strong>{placed.shippingEmail}</strong>.
                  </p>
                  <ul className='list-unstyled text-start mx-auto my-4' style={{ maxWidth: 320 }}>
                    <li className='d-flex justify-content-between'>
                      <span>Subtotal</span>
                      <span>${placed.subTotal.toFixed(2)}</span>
                    </li>
                    {placed.discountAmount > 0 && (
                      <li className='d-flex justify-content-between text-success'>
                        <span>Discount</span>
                        <span>-${placed.discountAmount.toFixed(2)}</span>
                      </li>
                    )}
                    <li className='d-flex justify-content-between'>
                      <span>Shipping</span>
                      <span>${placed.shippingCost.toFixed(2)}</span>
                    </li>
                    <li className='d-flex justify-content-between'>
                      <span>Tax</span>
                      <span>${placed.taxAmount.toFixed(2)}</span>
                    </li>
                    <li className='d-flex justify-content-between fw-bold border-top pt-2 mt-2'>
                      <span>Total</span>
                      <span>${placed.totalAmount.toFixed(2)}</span>
                    </li>
                  </ul>
                  <div className='d-flex gap-3 justify-content-center'>
                    <Link href='/account' className='btn rounded-pill px-4 border'>
                      My Orders
                    </Link>
                    <Link href='/shop' className='btn custom_btn rounded-pill px-4'>
                      Continue Shopping
                    </Link>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>
      </>
    )
  }

  const emptyCart = !cart || cart.items.length === 0

  const body = () => {
    if (authLoading || cartLoading) return <p className='text-center py-5'>Loading…</p>

    if (!user) {
      return (
        <div className='text-center py-5'>
          <p>Please log in to check out.</p>
          <Link href='/login?redirectTo=/checkout' className='btn custom_btn rounded-pill px-4'>
            Login
          </Link>
        </div>
      )
    }

    if (emptyCart) {
      return (
        <div className='text-center py-5'>
          <p style={{ color: 'var(--organi-text-muted)' }}>Your cart is empty.</p>
          <Link href='/shop' className='btn custom_btn rounded-pill px-4'>
            Browse Products
          </Link>
        </div>
      )
    }

    // The cart is deliberately left intact — an unconfirmed user can still review it and confirm
    // their email without losing what they added.

    // Every field gets a visible <label> tied to its input by id. A placeholder alone disappears
    // the moment you type, which is exactly when a checkout form is easiest to get lost in.
    const field = (
      name: keyof CheckoutForm,
      label: string,
      { type = 'text', col = 'col-12', optional = false }: { type?: string; col?: string; optional?: boolean } = {}
    ) => {
      const id = `checkout-${name}`

      return (
        <div className={col}>
          <label className='form-label' htmlFor={id}>
            {label}
            {optional && <span style={{ color: 'var(--organi-text-muted)', fontWeight: 400 }}> (optional)</span>}
          </label>
          <input id={id} type={type} className='form-control rounded-pill py-3' {...register(name)} />
          {errors[name] && <small className='text-danger'>{errors[name]?.message}</small>}
        </div>
      )
    }

    return (
      <form onSubmit={handleSubmit(onSubmit)} className='row g-4' noValidate>
        <div className='col-lg-7'>
          <div className='bg-white rounded-4 shadow-sm p-4'>
            <h5 className='mb-4' style={{ fontWeight: 800 }}>
              Shipping Details
            </h5>
            {formError && <div className='alert alert-danger'>{formError}</div>}
            <div className='row g-3'>
              {field('shippingFirstName', 'First name', { col: 'col-md-6' })}
              {field('shippingLastName', 'Last name', { col: 'col-md-6' })}
              {field('shippingAddress', 'Street address')}
              {field('shippingCity', 'City', { col: 'col-md-6' })}
              {field('shippingPostalCode', 'Postal code', { col: 'col-md-6', optional: true })}
              {field('shippingPhone', 'Phone', { col: 'col-md-6', type: 'tel' })}
              {field('shippingEmail', 'Email', { col: 'col-md-6', type: 'email' })}
              <div className='col-12'>
                <label className='form-label' htmlFor='checkout-notes'>
                  Order notes<span style={{ color: 'var(--organi-text-muted)', fontWeight: 400 }}> (optional)</span>
                </label>
                <textarea id='checkout-notes' className='form-control rounded-4 p-3' rows={3} {...register('notes')} />
              </div>
            </div>
          </div>
        </div>

        <div className='col-lg-5'>
          <div className='bg-white rounded-4 shadow-sm p-4'>
            <h5 className='mb-3' style={{ fontWeight: 800 }}>
              Order Summary
            </h5>
            {cart!.items.map(item => (
              <div key={item.id} className='d-flex justify-content-between mb-2'>
                <span style={{ color: 'var(--organi-text-muted)' }}>
                  {item.productName} × {item.quantity}
                </span>
                <span>${item.lineTotal.toFixed(2)}</span>
              </div>
            ))}
            <div className='d-flex justify-content-between border-top pt-2 mt-2 fw-bold'>
              <span>Subtotal</span>
              <span>${cart!.subTotal.toFixed(2)}</span>
            </div>
            <div className='mt-3'>
              <label className='form-label' htmlFor='checkout-couponCode'>
                Coupon code<span style={{ color: 'var(--organi-text-muted)', fontWeight: 400 }}> (optional)</span>
              </label>
              <input id='checkout-couponCode' className='form-control rounded-pill py-2' {...register('couponCode')} />
            </div>
            <p className='mt-2' style={{ color: 'var(--organi-text-muted)', fontSize: 13 }}>
              Shipping, tax and any discount are applied when the order is placed.
            </p>
            <button
              type='submit'
              className='btn custom_btn rounded-pill py-3 w-100 mt-2'
              disabled={isSubmitting || emailUnconfirmed}
            >
              {isSubmitting ? 'Placing order…' : 'Place Order'}
            </button>
            {emailUnconfirmed && (
              <p className='mt-2 mb-0 text-center' style={{ color: 'var(--organi-text-muted)', fontSize: 13 }}>
                Confirm your email address to place this order.
              </p>
            )}
          </div>
        </div>
      </form>
    )
  }

  return (
    <>
      <Breadcrumb title='Checkout' items={[{ label: 'Cart', href: '/cart' }, { label: 'Checkout' }]} />
      <section className='sec_space_large'>
        <div className='container'>
          <EmailConfirmationBanner className='mb-4' />
          {body()}
        </div>
      </section>
    </>
  )
}

export default CheckoutView
