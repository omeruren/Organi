// Mirrors Organi.Server.Application.Features.Newsletter.DTOs.NewsletterSubscriberResponse
export interface NewsletterSubscriberResponse {
  id: string
  email: string
  isActive: boolean
  subscribedAt: string
  unsubscribedAt: string | null
}
