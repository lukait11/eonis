import { Product } from './product.model';

export interface Wishlist {
  id: string;
  userId: string;
  items?: WishlistItem[];
}

export interface WishlistItem {
  id: string;
  wishlistId: string;
  productId: string;
  addedAt: string;
  product?: Product;
}
