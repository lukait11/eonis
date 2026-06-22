import { Component, Input, Output, EventEmitter, inject, computed, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { Product, primaryImage, effectivePrice } from '../../../core/models/product.model';
import { WishlistService } from '../../../core/services/wishlist.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-product-card',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css',
})
export class ProductCard {
  @Input({ required: true }) product!: Product;

  wishlist = inject(WishlistService);
  auth = inject(AuthService);

  get image(): string | null {
    return primaryImage(this.product);
  }

  get price(): number {
    return effectivePrice(this.product);
  }

  get hasDiscount(): boolean {
    return this.product.discount > 0;
  }

  get isWishlisted(): boolean {
    return this.wishlist.isWishlisted(this.product.id);
  }

  toggleWishlist(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    if (!this.auth.isLoggedIn()) return;

    if (this.isWishlisted) {
      const itemId = this.wishlist.getWishlistItemId(this.product.id);
      if (itemId) this.wishlist.removeItem(itemId).subscribe();
    } else {
      const wishlistId = this.wishlist.wishlist()?.id;
      if (wishlistId) this.wishlist.addItem(wishlistId, this.product.id).subscribe();
    }
  }
}
