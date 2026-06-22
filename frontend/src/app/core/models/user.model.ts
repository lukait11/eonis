export type UserRole = 'Admin' | 'Seller' | 'Customer';

export interface ApplicationUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  role: UserRole;
  profilePictureUrl: string;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}
