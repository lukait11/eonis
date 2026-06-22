import { Component, OnInit, signal, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { StarRating } from '../../shared/components/star-rating/star-rating';
import { ProductService } from '../../core/services/product.service';
import { ReviewService } from '../../core/services/review.service';
import { CartService } from '../../core/services/cart.service';
import { WishlistService } from '../../core/services/wishlist.service';
import { AuthService } from '../../core/services/auth.service';
import { Product, ProductVariant, primaryImage, effectivePrice } from '../../core/models/product.model';
import { ProductReview } from '../../core/models/review.model';

@Component({
  selector: 'app-product',
  imports: [RouterLink, DecimalPipe, DatePipe, ReactiveFormsModule, StarRating],
  templateUrl: './product.html',
  styleUrl: './product.css',
})
export class ProductPage implements OnInit {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private reviewService = inject(ReviewService);
  public cart = inject(CartService);
  public wishlist = inject(WishlistService);
  public auth = inject(AuthService);
  private fb = inject(FormBuilder);

  product = signal<Product | null>(null);
  reviews = signal<ProductReview[]>([]);
  loading = signal(true);
  selectedImage = signal<string | null>(null);
  selectedVariant = signal<ProductVariant | null>(null);
  addingToCart = signal(false);
  cartMessage = signal('');
  reviewRating = signal(0);
  submittingReview = signal(false);

  reviewForm = this.fb.group({
    comment: ['', Validators.required],
  });

  get effectivePrice(): number {
    return this.product() ? effectivePrice(this.product()!) : 0;
  }

  get isWishlisted(): boolean {
    return this.product() ? this.wishlist.isWishlisted(this.product()!.id) : false;
  }

  get avgRating(): number {
    const r = this.reviews();
    return r.length ? r.reduce((s, rv) => s + rv.rating, 0) / r.length : 0;
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.productService.getById(id).subscribe({
      next: p => {
        this.product.set(p);
        this.selectedImage.set(primaryImage(p));
        this.loading.set(false);
        if (p.variants?.length) this.selectedVariant.set(p.variants[0]);
      },
      error: () => this.loading.set(false),
    });
    this.reviewService.getProductReviews(id).subscribe(r => this.reviews.set(r));
  }

  selectImage(url: string): void { this.selectedImage.set(url); }
  selectVariant(v: ProductVariant): void { this.selectedVariant.set(v); }

  addToCart(): void {
    const cartId = this.cart.cart()?.id;
    const variantId = this.selectedVariant()?.id;
    if (!cartId || !variantId) return;
    this.addingToCart.set(true);
    this.cart.addItem(cartId, variantId, 1).subscribe({
      next: () => {
        this.cartMessage.set('Added to cart!');
        this.addingToCart.set(false);
        setTimeout(() => this.cartMessage.set(''), 2500);
      },
      error: () => {
        this.cartMessage.set('Failed to add.');
        this.addingToCart.set(false);
      },
    });
  }

  toggleWishlist(): void {
    if (!this.product()) return;
    if (this.isWishlisted) {
      const id = this.wishlist.getWishlistItemId(this.product()!.id);
      if (id) this.wishlist.removeItem(id).subscribe();
    } else {
      const wId = this.wishlist.wishlist()?.id;
      if (wId) this.wishlist.addItem(wId, this.product()!.id).subscribe();
    }
  }

  onRated(n: number): void { this.reviewRating.set(n); }

  submitReview(): void {
    if (this.reviewForm.invalid || !this.reviewRating()) return;
    this.submittingReview.set(true);
    this.reviewService.createProductReview({
      productId: this.product()!.id,
      userId: this.auth.currentUserId()!,
      rating: this.reviewRating(),
      comment: this.reviewForm.value.comment!,
    }).subscribe({
      next: rv => {
        this.reviews.update(r => [rv, ...r]);
        this.reviewForm.reset();
        this.reviewRating.set(0);
        this.submittingReview.set(false);
      },
      error: () => this.submittingReview.set(false),
    });
  }
}
