// The template's small triple-dot eyebrow + section heading.
const SectionTitle = ({ eyebrow, title }: { eyebrow: string; title: string }) => {
  return (
    <div className='category_top_content_text'>
      <div className='category_sub_title d-flex align-items-center'>
        <i className='far fa-circle' />
        <i className='far fa-circle' />
        <i className='far fa-circle' />
        <span className='ps-2'>{eyebrow}</span>
      </div>
      <div className='category_title pb-3'>
        <h2>{title}</h2>
      </div>
    </div>
  )
}

export default SectionTitle
