import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AnnoncePayload {
  title: string;
  categoryId: number | null;
  adType: string;
  description: string;
  price: any;
  condition: string;
  location: string;
  phone: string;
  email: string;
  hidePhone: boolean;
  extraData: any;
  status?: string;
  currentStep?: number;
}

@Injectable({ providedIn: 'root' })
export class AnnonceService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  suggestCategories(query: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/annonces/suggest-categories?query=${encodeURIComponent(query)}`);
  }

  getMenus(): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/menus`);
  }

  getCategoriesForDeposit(menuId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/categories/for-deposit/${menuId}`);
  }

  getAdTypes(categoryId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/annonces/ad-types/${categoryId}`);
  }

  lookupVehicle(immat: string): Observable<any> {
    return this.http.get<any>(`${this.api}/vehicle/lookup/${encodeURIComponent(immat)}`);
  }

  generateDescription(context: any): Observable<{ description: string }> {
    return this.http.post<{ description: string }>(`${this.api}/ai/generate-description`, context);
  }

  getPriceEstimate(categoryId: number, brand: string, model: string): Observable<any> {
    return this.http.post<any>(`${this.api}/annonces/price-estimate`, { categoryId, brand, model });
  }

  saveDraft(payload: AnnoncePayload): Observable<any> {
    return this.http.post<any>(`${this.api}/annonces/draft`, payload);
  }

  submit(payload: AnnoncePayload): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.api}/annonces`, payload);
  }

  submitFeedback(annonceId: number, rating: string, category: string): Observable<void> {
    return this.http.post<void>(`${this.api}/feedback`, { annonceId, rating, category });
  }

  getMyAnnonces(email: string, sortBy: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/annonces/my?email=${encodeURIComponent(email)}&sortBy=${sortBy}`);
  }

  pauseAnnonce(id: number): Observable<any> {
    return this.http.put<any>(`${this.api}/annonces/${id}/pause`, {});
  }

  deleteAnnonce(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/annonces/${id}`);
  }

  getAnnonce(id: number): Observable<any> {
    return this.http.get<any>(`${this.api}/annonces/${id}`);
  }

  trackView(id: number, userId: string): Observable<any> {
    return this.http.post(`${this.api}/annonces/${id}/view`, { userId });
  }

  toggleFavorite(id: number, userId: string): Observable<{ favorited: boolean }> {
    return this.http.post<{ favorited: boolean }>(`${this.api}/annonces/${id}/favorite`, { userId });
  }

  sendMessage(id: number, senderId: string, senderEmail: string, content: string): Observable<any> {
    return this.http.post(`${this.api}/annonces/${id}/message`, { senderId, senderEmail, content });
  }

}
