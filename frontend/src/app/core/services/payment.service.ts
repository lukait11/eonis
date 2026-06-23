import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PaymentIntentResponse {
  clientSecret: string;
  paymentIntentId: string;
}

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private readonly api = `${environment.apiUrl}/payment`;

  constructor(private http: HttpClient) {}

  createIntent(orderId: string): Observable<PaymentIntentResponse> {
    return this.http.post<PaymentIntentResponse>(`${this.api}/create-intent`, { orderId });
  }
}
