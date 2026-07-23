'use client'

// Replaces the template's jQuery quantity stepper (`.input_number_decrement/increment`).
const QuantityStepper = ({
  value,
  onChange,
  min = 1,
  max = 99
}: {
  value: number
  onChange: (next: number) => void
  min?: number
  max?: number
}) => {
  const clamp = (n: number) => Math.min(max, Math.max(min, n))

  return (
    <div className='quantity_input d-inline-flex align-items-center'>
      <button type='button' className='input_number_decrement' onClick={() => onChange(clamp(value - 1))}>
        –
      </button>
      <input
        className='input_number'
        type='number'
        value={value}
        min={min}
        max={max}
        onChange={e => onChange(clamp(Number(e.target.value) || min))}
      />
      <button type='button' className='input_number_increment' onClick={() => onChange(clamp(value + 1))}>
        +
      </button>
    </div>
  )
}

export default QuantityStepper
