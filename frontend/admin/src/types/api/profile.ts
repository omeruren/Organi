// Mirrors Organi.Server.Application.Features.Profile.DTOs.ProfileResponse
export interface ProfileResponse {
  id: string
  email: string
  firstName: string
  lastName: string
  phoneNumber: string | null
  dateOfBirth: string | null // DateOnly → "yyyy-MM-dd"
  avatarUrl: string | null
  emailConfirmed: boolean
  lastLoginAt: string | null
  roles: string[]
  createdAt: string
}

// Mirrors UpdateProfileCommand
export interface UpdateProfileRequest {
  firstName: string
  lastName: string
  phoneNumber: string | null
  dateOfBirth: string | null
  avatarUrl: string | null
}
