import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AdTypeAdminService {
  private api = `${environment.apiUrl}/adtypes`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.api);
  }

  create(adType: any): Observable<any> {
    return this.http.post(this.api, adType);
  }

  update(id: number, adType: any): Observable<any> {
    return this.http.put(`${this.api}/${id}`, adType);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }
}
