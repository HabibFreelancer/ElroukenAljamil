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
  browserCategories: any[] = [];
  private searchSubject = new Subject<string>();
  private apiUrl = 'https://localhost:7283/api';

  annonce = {
    category: '',
    title: '',
    description: '',
    price: null as number | null,
    location: '',
    condition: ''
  };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any[]>(`${this.apiUrl}/menus`).subscribe(data => this.menus = data);

    this.searchSubject.pipe(debounceTime(400)).subscribe(query => {
      if (query.length >= 2) {
        this.http.get<any[]>(`${this.apiUrl}/annonces/suggest-categories?query=${encodeURIComponent(query)}`)
          .subscribe(data => this.suggestedCategories = data);
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
  }

  openCategoryBrowser() {
    this.showCategoryBrowser = true;
    this.suggestedCategories = [];
  }

  selectMenu(menu: any) {
    this.selectedMenuId = menu.id;
    this.http.get<any[]>(`${this.apiUrl}/categories/by-menu/${menu.id}`)
      .subscribe(data => {
        this.browserCategories = data.filter(c => !c.parentCategoryId);
      });
  }

  selectBrowserCategory(cat: any) {
    this.selectedCategoryId = cat.id;
    const menu = this.menus.find(m => m.id === cat.menuId);
    this.annonce.category = (menu?.name || '') + ' > ' + cat.name;
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
