import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Wishlist, WishlistItem } from '../models/wishlist.model';

@Injectable({ providedIn: 'root' })
export class WishlistService {
  private readonly wishlistApi = `${environment.apiUrl}/wishlist`;
  private readonly itemApi = `${environment.apiUrl}/wishlistitem`;

  private _wishlist = signal<Wishlist | null>(null);

  readonly wishlist = this._wishlist.asReadonly();
  readonly itemCount = computed(() => this._wishlist()?.items?.length ?? 0);

  constructor(private http: HttpClient) {}

  loadWishlist(userId: string): Observable<Wishlist> {
    return this.http.get<Wishlist>(`${this.wishlistApi}/user/${userId}`).pipe(
      tap(w => this._wishlist.set(w)),
      catchError(() =>
        this.http
          .post<Wishlist>(this.wishlistApi, { userId })
          .pipe(tap(w => this._wishlist.set(w)))
      )
    );
  }

  getItems(wishlistId: string): Observable<WishlistItem[]> {
    return this.http.get<WishlistItem[]>(`${this.itemApi}/wishlist/${wishlistId}`);
  }

  addItem(wishlistId: string, productId: string): Observable<WishlistItem> {
    return this.http
      .post<WishlistItem>(this.itemApi, { wishlistId, productId })
      .pipe(
        tap(() => {
          const w = this._wishlist();
          if (w) this.loadWishlist(w.userId).subscribe();
        })
      );
  }

  removeItem(itemId: string): Observable<void> {
    return this.http.delete<void>(`${this.itemApi}/${itemId}`).pipe(
      tap(() => {
        const w = this._wishlist();
        if (w) this.loadWishlist(w.userId).subscribe();
      })
    );
  }

  isWishlisted(productId: string): boolean {
    return this._wishlist()?.items?.some(i => i.productId === productId) ?? false;
  }

  getWishlistItemId(productId: string): string | null {
    return this._wishlist()?.items?.find(i => i.productId === productId)?.id ?? null;
  }

  clear(): void {
    this._wishlist.set(null);
  }
}
