import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Product, ProductVariant } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly api = `${environment.apiUrl}/product`;
  private readonly variantApi = `${environment.apiUrl}/productvariant`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.api).pipe(map(r => r ?? []));
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.api}/${id}`);
  }

  getByCategory(categoryId: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.api}/category/${categoryId}`).pipe(map(r => r ?? []));
  }

  create(product: Partial<Product>): Observable<Product> {
    return this.http.post<Product>(this.api, product);
  }

  update(product: Product): Observable<Product> {
    return this.http.put<Product>(this.api, product);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }

  uploadImage(productId: string, file: File): Observable<string> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<string>(`${this.api}/${productId}/image`, form);
  }

  getVariants(productId: string): Observable<ProductVariant[]> {
    return this.http.get<ProductVariant[]>(`${this.variantApi}/product/${productId}`).pipe(map(r => r ?? []));
  }

  createVariant(variant: Partial<ProductVariant>): Observable<ProductVariant> {
    return this.http.post<ProductVariant>(this.variantApi, variant);
  }

  updateVariant(variant: ProductVariant): Observable<ProductVariant> {
    return this.http.put<ProductVariant>(this.variantApi, variant);
  }

  deleteVariant(id: string): Observable<void> {
    return this.http.delete<void>(`${this.variantApi}/${id}`);
  }
}
