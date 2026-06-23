import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Product, ProductVariant } from '../models/product.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly api = `${environment.apiUrl}/product`;
  private readonly variantApi = `${environment.apiUrl}/productvariant`;

  constructor(private http: HttpClient) {}

  getPaged(page: number, pageSize: number, search?: string, categoryId?: string, sort?: string): Observable<PagedResult<Product>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    if (categoryId) params = params.set('categoryId', categoryId);
    if (sort && sort !== 'default') params = params.set('sort', sort);
    return this.http.get<PagedResult<Product>>(this.api, { params });
  }

  getAll(): Observable<Product[]> {
    return this.getPaged(1, 1000).pipe(map(r => r.items));
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
