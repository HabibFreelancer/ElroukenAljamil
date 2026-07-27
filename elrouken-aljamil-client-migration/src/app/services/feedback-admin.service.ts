import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FeedbackAdminService {
  private api = `${environment.apiUrl}/feedback`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.api);
  }

  getStats(): Observable<any> {
    return this.http.get<any>(`${this.api}/stats`);
  }
}
