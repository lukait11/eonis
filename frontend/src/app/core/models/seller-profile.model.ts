export interface SellerProfile {
  id: string;
  userId: string;
  storeName: string;
  description: string;
  logoUrl: string;
  profilePictureUrl?: string;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}
