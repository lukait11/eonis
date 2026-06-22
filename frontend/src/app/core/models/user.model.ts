export type UserRole = 'Admin' | 'Seller' | 'Customer';

export interface ApplicationUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string | null;
  dateOfBirth: string | null;
  role: UserRole;
  profilePictureUrl: string | null;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}
