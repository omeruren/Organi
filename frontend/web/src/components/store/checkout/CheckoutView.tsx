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
import { ApiError } from '@/libs/api-client'

// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'

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
  const createOrder = useCreateOrder()

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
                  <div style={{ fontSize: 48, color: '#7cc000' }}>
                    <i className='fas fa-check-circle' />
                  </div>
                  <h3 className='mt-3' style={{ fontWeight: 800 }}>
                    Thank you for your order!
                  </h3>
                  <p style={{ color: '#6b6b6b' }}>
                    Your order <strong>{placed.orderNumber}</strong> has been placed and is <strong>{placed.status}</strong>.
                  </p>
                  <p style={{ color: '#6b6b6b' }}>
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
                    <Link href='/shop' className='btn custom_btn rounded-pill px-4 text-white'>
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
          <Link href='/login?redirectTo=/checkout' className='btn custom_btn rounded-pill px-4 text-white'>
            Login
          </Link>
        </div>
      )
    }

    if (emptyCart) {
      return (
        <div className='text-center py-5'>
          <p style={{ color: '#6b6b6b' }}>Your cart is empty.</p>
          <Link href='/shop' className='btn custom_btn rounded-pill px-4 text-white'>
            Browse Products
          </Link>
        </div>
      )
    }

    const field = (name: keyof CheckoutForm, placeholder: string, type = 'text') => (
      <div className='col-12'>
        <input type={type} className='form-control rounded-pill py-3' placeholder={placeholder} {...register(name)} />
        {errors[name] && <small className='text-danger'>{errors[name]?.message}</small>}
      </div>
    )

    return (
      <form onSubmit={handleSubmit(onSubmit)} className='row g-4' noValidate>
        <div className='col-lg-7'>
          <div className='bg-white rounded-4 shadow-sm p-4'>
            <h5 className='mb-4' style={{ fontWeight: 800 }}>
              Shipping Details
            </h5>
            {formError && <div className='alert alert-danger'>{formError}</div>}
            <div className='row g-3'>
              <div className='col-md-6'>
                <input className='form-control rounded-pill py-3' placeholder='First name' {...register('shippingFirstName')} />
                {errors.shippingFirstName && <small className='text-danger'>{errors.shippingFirstName.message}</small>}
              </div>
              <div className='col-md-6'>
                <input className='form-control rounded-pill py-3' placeholder='Last name' {...register('shippingLastName')} />
                {errors.shippingLastName && <small className='text-danger'>{errors.shippingLastName.message}</small>}
              </div>
              {field('shippingAddress', 'Street address')}
              <div className='col-md-6'>
                <input className='form-control rounded-pill py-3' placeholder='City' {...register('shippingCity')} />
                {errors.shippingCity && <small className='text-danger'>{errors.shippingCity.message}</small>}
              </div>
              <div className='col-md-6'>
                <input className='form-control rounded-pill py-3' placeholder='Postal code (optional)' {...register('shippingPostalCode')} />
              </div>
              <div className='col-md-6'>
                <input className='form-control rounded-pill py-3' placeholder='Phone' {...register('shippingPhone')} />
                {errors.shippingPhone && <small className='text-danger'>{errors.shippingPhone.message}</small>}
              </div>
              <div className='col-md-6'>
                <input type='email' className='form-control rounded-pill py-3' placeholder='Email' {...register('shippingEmail')} />
                {errors.shippingEmail && <small className='text-danger'>{errors.shippingEmail.message}</small>}
              </div>
              <div className='col-12'>
                <textarea className='form-control rounded-4 p-3' rows={3} placeholder='Order notes (optional)' {...register('notes')} />
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
                <span style={{ color: '#6b6b6b' }}>
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
              <input className='form-control rounded-pill py-2' placeholder='Coupon code (optional)' {...register('couponCode')} />
            </div>
            <p className='mt-2' style={{ color: '#6b6b6b', fontSize: 13 }}>
              Shipping, tax and any discount are applied when the order is placed.
            </p>
            <button type='submit' className='btn custom_btn rounded-pill py-3 text-white w-100 mt-2' disabled={isSubmitting}>
              {isSubmitting ? 'Placing order…' : 'Place Order'}
            </button>
          </div>
        </div>
      </form>
    )
  }

  return (
    <>
      <Breadcrumb title='Checkout' items={[{ label: 'Cart', href: '/cart' }, { label: 'Checkout' }]} />
      <section className='sec_space_large'>
        <div className='container'>{body()}</div>
      </section>
    </>
  )
}

export default CheckoutView
