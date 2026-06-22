import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Order, OrderItem } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly api = `${environment.apiUrl}/order`;
  private readonly itemApi = `${environment.apiUrl}/orderitem`;

  constructor(private http: HttpClient) {}

  getByUser(userId: string): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.api}/user/${userId}`);
  }

  getBySeller(sellerId: string): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.api}/seller/${sellerId}`);
  }

  getById(id: string): Observable<Order> {
    return this.http.get<Order>(`${this.api}/${id}`);
  }

  getItems(orderId: string): Observable<OrderItem[]> {
    return this.http.get<OrderItem[]>(`${this.itemApi}/order/${orderId}`);
  }

  updateStatus(order: Order): Observable<Order> {
    return this.http.put<Order>(this.api, order);
  }
}
