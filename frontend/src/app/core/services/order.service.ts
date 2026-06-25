import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Order, OrderItem } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly api = `${environment.apiUrl}/order`;
  private readonly itemApi = `${environment.apiUrl}/orderitem`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Order[]> {
    return this.http.get<Order[]>(this.api).pipe(map(r => r ?? []));
  }

  getByUser(userId: string): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.api}/user/${userId}`).pipe(map(r => r ?? []));
  }

  getBySeller(sellerId: string): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.api}/seller/${sellerId}`).pipe(map(r => r ?? []));
  }

  getById(id: string): Observable<Order> {
    return this.http.get<Order>(`${this.api}/${id}`);
  }

  getItems(orderId: string): Observable<OrderItem[]> {
    return this.http.get<OrderItem[]>(`${this.itemApi}/order/${orderId}`).pipe(map(r => r ?? []));
  }

  create(body: Partial<Order>): Observable<Order> {
    return this.http.post<Order>(this.api, body);
  }

  createItem(body: { orderId: string; productVariantId: string; quantity: number }): Observable<OrderItem> {
    return this.http.post<OrderItem>(this.itemApi, body);
  }

  updateStatus(orderId: string, status: string): Observable<Order> {
    return this.http.put<Order>(`${this.api}/${orderId}/status`, { id: orderId, status });
  }
}
