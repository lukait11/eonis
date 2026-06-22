export interface ProductReview {
  id: string;
  productId: string;
  userId: string;
  rating: number;
  comment: string;
  createdAt: string;
}

export interface SellerReview {
  id: string;
  sellerProfileId: string;
  userId: string;
  rating: number;
  comment: string;
  createdAt: string;
}
