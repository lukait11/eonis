import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { WishlistService } from '../../core/services/wishlist.service';
import { ProductService } from '../../core/services/product.service';
import { CartService } from '../../core/services/cart.service';
import { AuthService } from '../../core/services/auth.service';
import { WishlistItem } from '../../core/models/wishlist.model';
import { Product, primaryImage, effectivePrice } from '../../core/models/product.model';
import { DecimalPipe } from '@angular/common';

interface WishlistView {
  item: WishlistItem;
  product: Product | null;
}

@Component({
  selector: 'app-wishlist',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './wishlist.html',
  styleUrl: './wishlist.css',
})
export class Wishlist implements OnInit {
  wishlistService = inject(WishlistService);
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private auth = inject(AuthService);

  views = signal<WishlistView[]>([]);
  loading = signal(true);
  addingToCart = signal<string | null>(null);

  ngOnInit(): void {
    const w = this.wishlistService.wishlist();
    if (!w) { this.loading.set(false); return; }

    this.wishlistService.getItems(w.id).subscribe({
      next: items => {
        if (items.length === 0) { this.loading.set(false); return; }
        let loaded = 0;
        const views: WishlistView[] = items.map(i => ({ item: i, product: null }));
        items.forEach((item, idx) => {
          this.productService.getById(item.productId).subscribe({
            next: p => { views[idx].product = p; },
            complete: () => {
              loaded++;
              if (loaded === items.length) { this.views.set(views); this.loading.set(false); }
            },
          });
        });
      },
      error: () => this.loading.set(false),
    });
  }

  removeItem(itemId: string): void {
    this.wishlistService.removeItem(itemId).subscribe(() =>
      this.views.update(v => v.filter(x => x.item.id !== itemId))
    );
  }

  addToCart(productId: string, view: WishlistView): void {
    if (!view.product?.variants?.length) return;
    const cartId = this.cartService.cart()?.id;
    const variantId = view.product.variants[0].id;
    if (!cartId) return;
    this.addingToCart.set(productId);
    this.cartService.addItem(cartId, variantId, 1).subscribe({
      next: () => this.addingToCart.set(null),
      error: () => this.addingToCart.set(null),
    });
  }

  getImage = primaryImage;
  getPrice = effectivePrice;
}
