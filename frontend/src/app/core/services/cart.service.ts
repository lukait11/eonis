import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, of, switchMap, catchError, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Cart, CartItem } from '../models/cart.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly cartApi = `${environment.apiUrl}/cart`;
  private readonly itemApi = `${environment.apiUrl}/cartitem`;

  private _cart = signal<Cart | null>(null);

  readonly cart = this._cart.asReadonly();
  readonly itemCount = computed(() => this._cart()?.items?.length ?? 0);
  readonly total = computed(() => this._cart()?.totalAmount ?? 0);

  constructor(private http: HttpClient) {}

  loadCart(userId: string): Observable<Cart> {
    return this.http.get<Cart>(`${this.cartApi}/user/${userId}`).pipe(
      tap(cart => this._cart.set(cart)),
      catchError(() => {
        return this.http
          .post<Cart>(this.cartApi, { userId, totalAmount: 0 })
          .pipe(tap(cart => this._cart.set(cart)));
      })
    );
  }

  getItems(cartId: string): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(`${this.itemApi}/cart/${cartId}`).pipe(map(r => r ?? []));
  }

  addItem(cartId: string, productVariantId: string, quantity: number): Observable<CartItem> {
    return this.http
      .post<CartItem>(this.itemApi, { cartId, productVariantId, quantity })
      .pipe(
        tap(() => {
          const cart = this._cart();
          if (cart) this.loadCart(cart.userId).subscribe();
        })
      );
  }

  updateItem(item: CartItem): Observable<CartItem> {
    const body = { cartId: item.cartId, productVariantId: item.productVariantId, quantity: item.quantity };
    return this.http.put<CartItem>(`${this.itemApi}/${item.id}`, body).pipe(
      tap(() => {
        const cart = this._cart();
        if (cart) this.loadCart(cart.userId).subscribe();
      })
    );
  }

  removeItem(itemId: string): Observable<void> {
    return this.http.delete<void>(`${this.itemApi}/${itemId}`).pipe(
      tap(() => {
        const cart = this._cart();
        if (cart) this.loadCart(cart.userId).subscribe();
      })
    );
  }

  clear(): void {
    this._cart.set(null);
  }
}
