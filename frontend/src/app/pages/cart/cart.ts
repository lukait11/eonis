import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { CartService } from '../../core/services/cart.service';
import { ProductService } from '../../core/services/product.service';
import { CartItem } from '../../core/models/cart.model';
import { Product, primaryImage } from '../../core/models/product.model';

interface CartItemView {
  item: CartItem;
  product: Product | null;
}

@Component({
  selector: 'app-cart',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './cart.html',
  styleUrl: './cart.css',
})
export class Cart implements OnInit {
  cartService = inject(CartService);
  private productService = inject(ProductService);

  itemViews = signal<CartItemView[]>([]);
  loading = signal(true);

  total = computed(() =>
    this.itemViews().reduce((sum, v) => {
      if (!v.product) return sum;
      const price = v.product.basePrice * (1 - v.product.discount / 100);
      return sum + price * v.item.quantity;
    }, 0)
  );

  ngOnInit(): void {
    const cart = this.cartService.cart();
    if (!cart) { this.loading.set(false); return; }

    this.cartService.getItems(cart.id).subscribe({
      next: items => {
        if (items.length === 0) { this.loading.set(false); return; }
        let loaded = 0;
        const views: CartItemView[] = items.map(i => ({ item: i, product: null }));

        items.forEach((item, idx) => {
          if (!item.productVariant?.productId) {
            loaded++;
            if (loaded === items.length) { this.itemViews.set(views); this.loading.set(false); }
            return;
          }
          this.productService.getById(item.productVariant.productId).subscribe({
            next: p => { views[idx].product = p; },
            complete: () => {
              loaded++;
              if (loaded === items.length) { this.itemViews.set(views); this.loading.set(false); }
            },
          });
        });
      },
      error: () => this.loading.set(false),
    });
  }

  getImage(product: Product | null): string | null {
    return product ? primaryImage(product) : null;
  }

  updateQty(item: CartItem, delta: number): void {
    const newQty = item.quantity + delta;
    if (newQty < 1) return;
    this.cartService.updateItem({ ...item, quantity: newQty }).subscribe(() =>
      this.itemViews.update(views =>
        views.map(v => v.item.id === item.id ? { ...v, item: { ...item, quantity: newQty } } : v)
      )
    );
  }

  remove(itemId: string): void {
    this.cartService.removeItem(itemId).subscribe(() =>
      this.itemViews.update(views => views.filter(v => v.item.id !== itemId))
    );
  }
}
