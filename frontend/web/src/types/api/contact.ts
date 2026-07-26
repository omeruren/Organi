// Mirrors Organi.Server.Application.Features.Contact.Commands.SubmitContactMessage.SubmitContactMessageCommand
export interface ContactMessageRequest {
  name: string
  email: string
  subject: string
  message: string
}

// Mirrors Organi.Server.Application.Features.Contact.DTOs.ContactMessageResponse
export interface ContactMessageResponse {
  id: string
  name: string
  email: string
  subject: string
  message: string
  isHandled: boolean
  createdAt: string
}
