import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Address } from '../models/address.model';

@Injectable({ providedIn: 'root' })
export class AddressService {
  private readonly api = `${environment.apiUrl}/address`;

  constructor(private http: HttpClient) {}

  getByUser(userId: string): Observable<Address[]> {
    return this.http.get<Address[]>(`${this.api}/user/${userId}`);
  }

  create(address: Partial<Address>): Observable<Address> {
    return this.http.post<Address>(this.api, address);
  }

  update(address: Address): Observable<Address> {
    return this.http.put<Address>(this.api, address);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }
}
