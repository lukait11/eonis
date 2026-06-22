import { ProductVariant } from './product.model';

export interface Cart {
  id: string;
  userId: string;
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
  items?: CartItem[];
}

export interface CartItem {
  id: string;
  cartId: string;
  productVariantId: string;
  quantity: number;
  productVariant?: ProductVariant;
}
