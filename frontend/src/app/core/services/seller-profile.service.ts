import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { SellerProfile } from '../models/seller-profile.model';

@Injectable({ providedIn: 'root' })
export class SellerProfileService {
  private readonly api = `${environment.apiUrl}/sellerprofile`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<SellerProfile[]> {
    return this.http.get<SellerProfile[]>(this.api).pipe(map(r => r ?? []));
  }

  getById(id: string): Observable<SellerProfile> {
    return this.http.get<SellerProfile>(`${this.api}/${id}`);
  }

  update(profile: SellerProfile): Observable<SellerProfile> {
    return this.http.put<SellerProfile>(this.api, profile);
  }
}
