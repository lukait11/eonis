import { Component, OnInit, signal, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { StarRating } from '../../shared/components/star-rating/star-rating';
import { ProductCard } from '../../shared/components/product-card/product-card';
import { SellerProfileService } from '../../core/services/seller-profile.service';
import { ProductService } from '../../core/services/product.service';
import { ReviewService } from '../../core/services/review.service';
import { AuthService } from '../../core/services/auth.service';
import { SellerProfile } from '../../core/models/seller-profile.model';
import { Product } from '../../core/models/product.model';
import { SellerReview } from '../../core/models/review.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-seller-profile',
  imports: [ReactiveFormsModule, StarRating, ProductCard, DatePipe],
  templateUrl: './seller-profile.html',
  styleUrl: './seller-profile.css',
})
export class SellerProfilePage implements OnInit {
  private route = inject(ActivatedRoute);
  private sellerService = inject(SellerProfileService);
  private productService = inject(ProductService);
  private reviewService = inject(ReviewService);
  public auth = inject(AuthService);
  private fb = inject(FormBuilder);

  seller = signal<SellerProfile | null>(null);
  products = signal<Product[]>([]);
  reviews = signal<SellerReview[]>([]);
  loading = signal(true);
  reviewRating = signal(0);
  submittingReview = signal(false);

  reviewForm = this.fb.group({ comment: ['', Validators.required] });

  get avgRating(): number {
    const r = this.reviews();
    return r.length ? r.reduce((s, rv) => s + rv.rating, 0) / r.length : 0;
  }

  ngOnInit(): void {
    const sellerId = this.route.snapshot.paramMap.get('id')!;
    this.sellerService.getById(sellerId).subscribe({
      next: s => { this.seller.set(s); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
    this.productService.getAll().subscribe(all =>
      this.products.set(all.filter(p => p.sellerId === sellerId))
    );
    this.reviewService.getSellerReviews(sellerId).subscribe(r => this.reviews.set(r));
  }

  onRated(n: number): void { this.reviewRating.set(n); }

  submitReview(): void {
    if (this.reviewForm.invalid || !this.reviewRating()) return;
    this.submittingReview.set(true);
    this.reviewService.createSellerReview({
      sellerProfileId: this.seller()!.id,
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
