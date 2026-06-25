import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApplicationUser } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly api = `${environment.apiUrl}/applicationuser`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApplicationUser[]> {
    return this.http.get<ApplicationUser[]>(this.api).pipe(map(r => r ?? []));
  }

  getUser(id: string): Observable<ApplicationUser> {
    return this.http.get<ApplicationUser>(`${this.api}/${id}`);
  }

  updateUser(user: ApplicationUser): Observable<ApplicationUser> {
    const body = {
      firstName: user.firstName,
      lastName: user.lastName,
      phoneNumber: user.phoneNumber,
      dateOfBirth: user.dateOfBirth,
    };
    return this.http.put<ApplicationUser>(`${this.api}/${user.id}`, body);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }

  uploadProfilePicture(file: File): Observable<string> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<string>(`${this.api}/me/image`, form);
  }
}
