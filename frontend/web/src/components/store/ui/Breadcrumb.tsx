// Next Imports
import Link from 'next/link'

export interface BreadcrumbItem {
  label: string
  href?: string
}

// Template `.breadcrumb_sec_1` hero with centered title + trail.
const Breadcrumb = ({ title, items }: { title: string; items: BreadcrumbItem[] }) => {
  return (
    <section className='breadcrumb_sec_1 position-relative'>
      <div
        className='breadcrumb_wrap sec_space_mid_small'
        style={{ backgroundImage: 'url(/store/assets/images/breadcrumb/breadcrumb1.png)' }}
      >
        <div className='breadcrumb_cont text-center'>
          <div className='breadcrumb_title'>
            <h2 className='text-white'>{title}</h2>
          </div>
          <ul className='list-unstyled breadcrumb_item d-flex justify-content-center align-items-center text-white'>
            <li>
              <Link href='/'>
                <i className='fas fa-home active' /> Home
              </Link>
            </li>
            {items.map(item => (
              <li key={item.label}>
                <i className='fas fa-chevron-right' />
                {item.href ? <Link href={item.href}>{item.label}</Link> : item.label}
              </li>
            ))}
          </ul>
        </div>
      </div>
    </section>
  )
}

export default Breadcrumb
