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
    return this.http.get<any[]>(`${this.api}/listings/suggest-categories?query=${encodeURIComponent(query)}`);
  }

  getMenus(): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/menus`);
  }

  getCategoriesForDeposit(menuId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/categories/for-deposit/${menuId}`);
  }

  getAdTypes(categoryId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/listings/adtypes/${categoryId}`);
  }

  lookupVehicle(immat: string): Observable<any> {
    return this.http.get<any>(`${this.api}/vehicle/lookup/${encodeURIComponent(immat)}`);
  }

  generateDescription(context: any): Observable<{ description: string }> {
    return this.http.post<{ description: string }>(`${this.api}/ai/generate-description`, context);
  }

  getPriceEstimate(categoryId: number, brand: string, model: string): Observable<any> {
    return this.http.post<any>(`${this.api}/listings/price-estimate`, { categoryId, brand, model });
  }

  saveDraft(payload: AnnoncePayload): Observable<any> {
    return this.http.post<any>(`${this.api}/listings/draft`, payload);
  }

  submit(payload: AnnoncePayload): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.api}/listings`, payload);
  }

  submitFeedback(annonceId: number, rating: string, category: string): Observable<void> {
    return this.http.post<void>(`${this.api}/feedback`, { annonceId, rating, category });
  }

  getMyAnnonces(search: string, sortBy: string, status?: string): Observable<any[]> {
    let url = `${this.api}/listings/mine?sortBy=${sortBy}`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (status) url += `&status=${encodeURIComponent(status)}`;
    return this.http.get<any[]>(url);
  }

  pauseAnnonce(id: number): Observable<any> {
    return this.http.put<any>(`${this.api}/listings/${id}/pause`, {});
  }

  deleteAnnonce(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/listings/${id}`);
  }

  getAnnonce(id: number): Observable<any> {
    return this.http.get<any>(`${this.api}/listings/${id}`);
  }

  trackView(id: number, userId: string): Observable<any> {
    return this.http.post(`${this.api}/listings/${id}/view`, { userId });
  }

  toggleFavorite(id: number): Observable<{ favorited: boolean }> {
    return this.http.post<{ favorited: boolean }>(`${this.api}/listings/${id}/favorite`, {});
  }

}
