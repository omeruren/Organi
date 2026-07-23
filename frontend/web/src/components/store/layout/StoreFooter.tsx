// Next Imports
import Link from 'next/link'

// Component Imports
import NewsletterForm from '@/components/store/ui/NewsletterForm'

const StoreFooter = () => {
  return (
    <footer className='footer_section position-relative'>
      <div className='footer_section_wrap sec_top_space_50' style={{ backgroundImage: 'url(/store/assets/images/footer/footer.png)' }}>
        <div className='container'>
          <div className='footer_top_content d-flex flex-column flex-lg-row justify-content-between align-items-center'>
            <div className='footer_top_logo'>
              <Link href='/'>
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img src='/store/assets/images/logo/logo2.png' alt='Organi' />
              </Link>
            </div>
            <NewsletterForm />
            <div className='footer_top_social'>
              <ul className='list-unstyled d-flex justify-content-end'>
                <li className='me-3'>
                  <a href='#!'>
                    <i className='fab fa-twitter' />
                  </a>
                </li>
                <li className='me-3'>
                  <a href='#!'>
                    <i className='fab fa-facebook-f' />
                  </a>
                </li>
                <li className='me-3'>
                  <a href='#!'>
                    <i className='fab fa-youtube' />
                  </a>
                </li>
                <li>
                  <a href='#!'>
                    <i className='fab fa-instagram' />
                  </a>
                </li>
              </ul>
            </div>
          </div>
          <div className='footer_inner_content sec_space_xs_70'>
            <div className='footer_inner_content_wrap'>
              <div className='row'>
                <div className='col-md-6 col-lg-3'>
                  <div className='footer_inner_choose_content'>
                    <div className='footer_inner_choose_title'>
                      <h4>
                        <span className='text-white'>Why People Like us</span>
                      </h4>
                    </div>
                    <div className='footer_inner_choose_desc pt-2'>
                      <p>
                        Fresh, organic produce and groceries delivered from local farms and vendors you can trust —
                        quality you can taste, every day.
                      </p>
                    </div>
                    <div className='footer_inner_choose'>
                      <Link href='/about'>
                        <button type='button' className='btn custom_btn rounded-pill px-4 text-white'>
                          View More <i className='fas fa-long-arrow-alt-right' />
                        </button>
                      </Link>
                    </div>
                  </div>
                </div>
                <div className='col-md-6 col-lg-3'>
                  <div className='footer_inner_info_content'>
                    <div className='footer_inner_info_title'>
                      <h4>
                        <span className='text-white'>Information</span>
                      </h4>
                    </div>
                    <div className='footer_inner_info_item pt-2'>
                      <ul className='list-unstyled'>
                        <li>
                          <Link href='/about'>About Us</Link>
                        </li>
                        <li>
                          <Link href='/contact'>Contact Us</Link>
                        </li>
                        <li>
                          <Link href='/faqs'>FAQs</Link>
                        </li>
                        <li>
                          <Link href='/blog'>Blog</Link>
                        </li>
                        <li>
                          <Link href='/vendors'>Vendors</Link>
                        </li>
                      </ul>
                    </div>
                  </div>
                </div>
                <div className='col-md-6 col-lg-3'>
                  <div className='footer_inner_acct_content'>
                    <div className='footer_inner_acct_title'>
                      <h4>
                        <span className='text-white'>My Account</span>
                      </h4>
                    </div>
                    <div className='footer_inner_acct_item pt-2'>
                      <ul className='list-unstyled'>
                        <li>
                          <Link href='/account'>My Account</Link>
                        </li>
                        <li>
                          <Link href='/cart'>Shopping Cart</Link>
                        </li>
                        <li>
                          <Link href='/wishlist'>Wishlist</Link>
                        </li>
                        <li>
                          <Link href='/account'>Order History</Link>
                        </li>
                        <li>
                          <Link href='/compare'>Compare</Link>
                        </li>
                      </ul>
                    </div>
                  </div>
                </div>
                <div className='col-md-6 col-lg-3'>
                  <div className='footer_inner_cotc_content'>
                    <div className='footer_inner_ctc_title'>
                      <h4>
                        <span className='text-white'>Contact Us</span>
                      </h4>
                    </div>
                    <div className='footer_inner_ctc_info pt-2 text-white'>
                      <p>
                        Address: <span>1429 Netus Rd, NY 48247</span>
                      </p>
                      <p>
                        Email: <span>hello@organi.dev</span>
                      </p>
                      <p>
                        Phone: <span>(+87) 4886-4174</span>
                      </p>
                      <div className='footer_inner_payment_ctc'>
                        <div className='footer_inner_payment_title'>
                          <h5 className='text-white'>Payment Accepted</h5>
                        </div>
                        <div className='footer_inner_payment_thumb pt-3'>
                          {/* eslint-disable-next-line @next/next/no-img-element */}
                          <img src='/store/assets/images/payment/payment.png' alt='Payment methods' />
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div className='footer_bootom_content'>
            <div className='footer_bootom_wrap'>
              <div className='container'>
                <div className='row'>
                  <div className='col-md-6'>
                    <div className='footer_bootom_copyright'>
                      <p>
                        Copyright © 2026 <span>ORGANI</span> Inc. All rights reserved.
                      </p>
                    </div>
                  </div>
                  <div className='col-md-6'>
                    <div className='footer_bootom_privicy_cont d-flex justify-content-center align-items-center'>
                      <div className='footer_bootom_privicy pe-5'>
                        <p className='priv position-relative'>Privacy Policy</p>
                      </div>
                      <div className='footer_bootom_terms pe-5'>
                        <p className='position-relative'>Terms of Use</p>
                      </div>
                      <div className='footer_bootom_refunds'>
                        <p>Sales and Refunds</p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </footer>
  )
}

export default StoreFooter
