import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface NotificationPreference {
  notificationType: string;
  emailEnabled: boolean;
  smsEnabled: boolean;
  pushEnabled: boolean;
  inAppEnabled: boolean;
}

@Injectable({ providedIn: 'root' })
export class NotificationPreferenceService {
  private api = `${environment.apiUrl}/notifications/preferences`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<NotificationPreference[]> {
    return this.http.get<NotificationPreference[]>(this.api);
  }

  update(pref: NotificationPreference): Observable<void> {
    return this.http.put<void>(this.api, pref);
  }
}
