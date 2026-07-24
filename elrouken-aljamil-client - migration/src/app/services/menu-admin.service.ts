import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MenuAdminService {
  private api = `${environment.apiUrl}/menus`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.api);
  }

  create(menu: any): Observable<any> {
    return this.http.post(this.api, menu);
  }

  update(id: number, menu: any): Observable<any> {
    return this.http.put(`${this.api}/${id}`, menu);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }
}
