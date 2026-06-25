import { Routes } from '@angular/router';
import { authGuard, sellerGuard, adminGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/landing/landing').then(m => m.Landing),
  },
  {
    path: 'explore',
    loadComponent: () => import('./pages/explore/explore').then(m => m.Explore),
  },
  {
    path: 'product/:id',
    loadComponent: () => import('./pages/product/product').then(m => m.ProductPage),
  },
  {
    path: 'seller/:id',
    loadComponent: () => import('./pages/seller-profile/seller-profile').then(m => m.SellerProfilePage),
  },
  {
    path: 'my-store',
    loadComponent: () => import('./pages/my-store/my-store').then(m => m.MyStore),
    canActivate: [sellerGuard],
  },
  {
    path: 'profile',
    loadComponent: () => import('./pages/customer/customer').then(m => m.Customer),
    canActivate: [authGuard],
  },
  {
    path: 'cart',
    loadComponent: () => import('./pages/cart/cart').then(m => m.Cart),
    canActivate: [authGuard],
  },
  {
    path: 'checkout',
    loadComponent: () => import('./pages/checkout/checkout').then(m => m.Checkout),
    canActivate: [authGuard],
  },
  {
    path: 'wishlist',
    loadComponent: () => import('./pages/wishlist/wishlist').then(m => m.Wishlist),
    canActivate: [authGuard],
  },
  {
    path: 'admin',
    loadComponent: () => import('./pages/admin/admin').then(m => m.Admin),
    canActivate: [adminGuard],
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register').then(m => m.Register),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
