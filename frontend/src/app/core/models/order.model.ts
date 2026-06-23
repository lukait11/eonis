export type OrderStatus = 'Pending' | 'Paid' | 'Shipped' | 'Delivered' | 'Cancelled';

export interface OrderProductVariant {
  id: string;
  productId: string;
  size: string;
  color: string;
  quantity: number;
}

export interface OrderAddress {
  id: string;
  street: string;
  city: string;
  country: string;
  postalCode: number;
}

export interface Order {
  id: string;
  userId: string;
  sellerProfileId: string;
  addressId: string;
  status: OrderStatus;
  baseAmount: number;
  discount: number;
  createdAt: string;
  items?: OrderItem[];
  address?: OrderAddress;
}

export interface OrderItem {
  id: string;
  orderId: string;
  productVariantId: string;
  quantity: number;
  productVariant?: OrderProductVariant;
}
