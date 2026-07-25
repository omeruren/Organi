// Component Imports
import Breadcrumb from '@/components/store/ui/Breadcrumb'

// Shared shell for the storefront login/register pages.
const AuthCard = ({
  title,
  breadcrumbLabel,
  footer,
  children
}: {
  title: string
  breadcrumbLabel: string
  footer: React.ReactNode
  children: React.ReactNode
}) => {
  return (
    <>
      <Breadcrumb title={breadcrumbLabel} items={[{ label: breadcrumbLabel }]} />
      <section className='sec_space_large'>
        <div className='container'>
          <div className='row justify-content-center'>
            <div className='col-lg-6 col-md-8'>
              <div className='bg-white rounded-4 shadow p-4 p-md-5'>
                <h3 className='mb-4 text-center' style={{ fontWeight: 800 }}>
                  {title}
                </h3>
                {children}
                <div className='text-center mt-4' style={{ color: '#6b6b6b' }}>
                  {footer}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  )
}

export default AuthCard
