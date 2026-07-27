import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Menu {
  id: number;
  name: string;
  slug: string;
  icon: string;
  displayOrder: number;
  isActive: boolean;
  categories?: Category[];
}

export interface Category {
  id: number;
  menuId: number;
  parentCategoryId: number | null;
  name: string;
  slug: string;
  isLink: boolean;
  displayOrder: number;
  isActive: boolean;
  subCategories?: Category[];
}

@Injectable({ providedIn: 'root' })
export class MenuService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getMenus(): Observable<Menu[]> {
    return this.http.get<Menu[]>(`${this.apiUrl}/menus`);
  }

  getCategoriesByMenu(menuId: number): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/categories/by-menu/${menuId}`);
  }

  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
  }
}
