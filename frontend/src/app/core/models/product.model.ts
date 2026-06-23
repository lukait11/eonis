import { Category } from './category.model';
import { SellerProfile } from './seller-profile.model';

export type ProductStatus = 'Available' | 'OutOfStock' | 'Discontinued';

export interface ProductVariant {
  id: string;
  productId: string;
  size: string;
  color: string;
  quantity: number;
}

export interface ProductImage {
  id: string;
  productId: string;
  imageUrl: string;
  isPrimary: boolean;
}

export interface Product {
  id: string;
  sellerId: string;
  name: string;
  description: string;
  basePrice: number;
  discount: number;
  material: string;
  status: ProductStatus;
  createdAt: string;
  updatedAt: string;
  seller?: SellerProfile;
  categories?: Category[];
  variants?: ProductVariant[];
  images?: ProductImage[];
}

export function effectivePrice(product: Product | null | undefined): number {
  if (!product) return 0;
  return product.basePrice * (1 - product.discount / 100);
}

export function primaryImage(product: Product | null | undefined): string | null {
  if (!product || !product.images || product.images.length === 0) return null;
  const primary = product.images.find(i => i.isPrimary);
  return primary?.imageUrl ?? product.images[0]?.imageUrl ?? null;
}
