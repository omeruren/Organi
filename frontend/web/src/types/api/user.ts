// The three seeded system roles (RoleSeedData.cs)
export type AppRoleName = 'Admin' | 'Vendor' | 'Customer'

// Mirrors Organi.Server.Application.Features.Users.DTOs.UserResponse
export interface UserResponse {
  id: string
  email: string
  firstName: string
  lastName: string
  phoneNumber: string | null
  isActive: boolean
  emailConfirmed: boolean
  lastLoginAt: string | null
  roles: string[]
  createdAt: string
}
