import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-deposer-annonce',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule, RouterLink],
  templateUrl: './deposer-annonce.component.html',
  styleUrl: './deposer-annonce.component.scss'
})
export class DeposerAnnonceComponent implements OnInit {
  currentStep = 1;
  suggestedCategories: any[] = [];
  selectedCategoryId: number | null = null;
  selectedMenuId: number | null = null;
  menus: any[] = [];
  showCategoryBrowser = false;
  showAdType = false;
  adTypes: any[] = [];
  browserCategories: any[] = [];
  private searchSubject = new Subject<string>();
  private searchCache: { [key: string]: any[] } = {};
  private apiUrl = 'https://localhost:7283/api';

  annonce = {
    category: '',
    title: '',
    description: '',
    price: null as number | null,
    location: '',
    condition: '',
    adType: 'offre'
  };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.searchSubject.pipe(debounceTime(300)).subscribe(query => {
      if (query.length >= 2) {
        if (this.searchCache[query]) {
          this.suggestedCategories = this.searchCache[query];
        } else {
          this.http.get<any[]>(`${this.apiUrl}/annonces/suggest-categories?query=${encodeURIComponent(query)}`)
            .subscribe(data => {
              this.searchCache[query] = data;
              this.suggestedCategories = data;
            });
        }
      } else {
        this.suggestedCategories = [];
      }
    });
  }

  onTitleChange() {
    this.searchSubject.next(this.annonce.title);
  }

  selectSuggestedCategory(cat: any) {
    this.selectedCategoryId = cat.categoryId;
    this.annonce.category = cat.menuName + ' > ' + cat.categoryName;
    this.checkAdType(cat.categoryName, cat.menuName);
  }

  openCategoryBrowser() {
    this.showCategoryBrowser = true;
    this.suggestedCategories = [];
    if (this.menus.length === 0) {
      this.http.get<any[]>(`${this.apiUrl}/menus`).subscribe(data => this.menus = data);
    }
  }

  selectMenu(menu: any) {
    this.selectedMenuId = menu.id;
    this.http.get<any[]>(`${this.apiUrl}/categories/for-deposit/${menu.id}`)
      .subscribe(data => {
        this.browserCategories = data;
      });
  }

  selectBrowserCategory(cat: any) {
    this.selectedCategoryId = cat.id;
    const menu = this.menus.find(m => m.id === cat.menuId);
    this.annonce.category = (menu?.name || '') + ' > ' + cat.name;
    this.showCategoryBrowser = false;
    this.checkAdType(cat.name, menu?.name);
  }

  private checkAdType(categoryName: string, menuName?: string) {
    if (this.selectedCategoryId) {
      this.http.get<any[]>(`${this.apiUrl}/annonces/ad-types/${this.selectedCategoryId}`)
        .subscribe(data => {
          this.adTypes = data;
          this.showAdType = data.length > 0;
          if (data.length > 0) {
            const defaultType = data.find(t => t.isDefault);
            this.annonce.adType = defaultType ? defaultType.label : data[0].label;
          }
        });
    }
  }

  nextStep() {
    if (this.currentStep < 4) this.currentStep++;
  }

  prevStep() {
    if (this.currentStep > 1) this.currentStep--;
  }

  submit() {
    alert('Annonce publiée avec succès !');
  }
}
