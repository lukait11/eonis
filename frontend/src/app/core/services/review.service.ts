import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProductReview, SellerReview } from '../models/review.model';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private readonly productApi = `${environment.apiUrl}/productreview`;
  private readonly sellerApi = `${environment.apiUrl}/sellerreview`;

  constructor(private http: HttpClient) {}

  getProductReviews(productId: string): Observable<ProductReview[]> {
    return this.http.get<ProductReview[]>(`${this.productApi}/product/${productId}`);
  }

  getSellerReviews(sellerId: string): Observable<SellerReview[]> {
    return this.http.get<SellerReview[]>(`${this.sellerApi}/seller/${sellerId}`);
  }

  createProductReview(review: Partial<ProductReview>): Observable<ProductReview> {
    return this.http.post<ProductReview>(this.productApi, review);
  }

  createSellerReview(review: Partial<SellerReview>): Observable<SellerReview> {
    return this.http.post<SellerReview>(this.sellerApi, review);
  }

  deleteProductReview(id: string): Observable<void> {
    return this.http.delete<void>(`${this.productApi}/${id}`);
  }

  deleteSellerReview(id: string): Observable<void> {
    return this.http.delete<void>(`${this.sellerApi}/${id}`);
  }
}
