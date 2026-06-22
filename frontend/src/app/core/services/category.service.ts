import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Category } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly api = `${environment.apiUrl}/category`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.api).pipe(map(r => r ?? []));
  }

  getById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.api}/${id}`);
  }
}
