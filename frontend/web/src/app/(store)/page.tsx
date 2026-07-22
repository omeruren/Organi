// Placeholder storefront home — replaced by the ported template in B2.
// Inline styles only, so we can confirm no admin (MUI/Tailwind) or Bootstrap CSS leaks onto "/".
const HomePage = () => {
  return (
    <main
      style={{
        minHeight: '100dvh',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        gap: '1rem',
        fontFamily: 'system-ui, sans-serif',
        color: '#292929',
        textAlign: 'center',
        padding: '2rem'
      }}
    >
      <div style={{ fontSize: '2.5rem' }}>🌿</div>
      <h1 style={{ fontSize: '2rem', fontWeight: 800, margin: 0 }}>Organi storefront</h1>
      <p style={{ maxWidth: 420, color: '#6b6b6b', margin: 0 }}>
        The customer storefront is coming soon. The admin dashboard is live.
      </p>
      <a
        href='/admin'
        style={{
          marginTop: '0.5rem',
          background: '#7cc000',
          color: '#fff',
          padding: '0.75rem 1.5rem',
          borderRadius: '9999px',
          textDecoration: 'none',
          fontWeight: 600
        }}
      >
        Go to Admin
      </a>
    </main>
  )
}

export default HomePage
