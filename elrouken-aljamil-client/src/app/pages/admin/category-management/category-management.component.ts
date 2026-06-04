import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-category-management',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  templateUrl: './category-management.component.html',
  styleUrl: './category-management.component.scss'
})
export class CategoryManagementComponent implements OnInit {
  categories: any[] = [];
  filteredCategories: any[] = [];
  paginatedCategories: any[] = [];
  menus: any[] = [];
  parentCategories: any[] = [];
  showForm = false;
  editingCategory = false;
  searchText = '';
  filterMenuId = 0;
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  pages: number[] = [];
  currentCategory = { id: 0, menuId: 0, parentCategoryId: null as number | null, name: '', slug: '', isLink: true, showInDeposit: true, displayOrder: 0, isActive: true };
  private apiUrl = 'https://localhost:7283/api';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadMenus();
    this.loadCategories();
  }

  loadMenus() {
    this.http.get<any[]>(`${this.apiUrl}/menus`).subscribe(data => this.menus = data);
  }

  loadCategories() {
    this.http.get<any[]>(`${this.apiUrl}/categories`).subscribe(data => {
      this.categories = data;
      this.parentCategories = data.filter(c => !c.parentCategoryId);
      this.applyFilter();
    });
  }

  applyFilter() {
    const search = this.searchText.toLowerCase().trim();
    let result = [...this.categories];

    if (this.filterMenuId != 0) {
      result = result.filter(c => c.menuId == this.filterMenuId);
    }

    if (search) {
      result = result.filter(c => c.name.toLowerCase().includes(search) || c.slug.toLowerCase().includes(search));
    }

    this.filteredCategories = result;
    this.currentPage = 1;
    this.updatePagination();
  }

  updatePagination() {
    this.totalPages = Math.max(1, Math.ceil(this.filteredCategories.length / this.pageSize));
    this.pages = Array.from({ length: this.totalPages }, (_, i) => i + 1);
    const start = (this.currentPage - 1) * this.pageSize;
    this.paginatedCategories = this.filteredCategories.slice(start, start + this.pageSize);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updatePagination();
    }
  }

  exportExcel() {
    const data = this.filteredCategories.map(c => ({
      ID: c.id,
      Menu: this.getMenuName(c.menuId),
      Parent: c.parentCategoryId ? this.getParentName(c.parentCategoryId) : '-',
      Nom: c.name,
      Slug: c.slug,
      Lien: c.isLink ? 'Oui' : 'Non',
      Ordre: c.displayOrder
    }));
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Categories');
    const buffer = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'categories.xlsx');
  }

  save() {
    const payload = {
      ...this.currentCategory,
      menuId: Number(this.currentCategory.menuId),
      parentCategoryId: this.currentCategory.parentCategoryId ? Number(this.currentCategory.parentCategoryId) : null
    };
    if (this.editingCategory) {
      this.http.put(`${this.apiUrl}/categories/${payload.id}`, payload).subscribe(() => {
        this.loadCategories();
        this.cancel();
      });
    } else {
      this.http.post(`${this.apiUrl}/categories`, payload).subscribe(() => {
        this.loadCategories();
        this.cancel();
      });
    }
  }

  edit(cat: any) {
    this.currentCategory = { ...cat };
    this.editingCategory = true;
    this.showForm = true;
  }

  delete(id: number) {
    if (confirm('Supprimer cette catégorie ?')) {
      this.http.delete(`${this.apiUrl}/categories/${id}`).subscribe(() => this.loadCategories());
    }
  }

  cancel() {
    this.showForm = false;
    this.editingCategory = false;
    this.currentCategory = { id: 0, menuId: 0, parentCategoryId: null, name: '', slug: '', isLink: true, showInDeposit: true, displayOrder: 0, isActive: true };
  }

  getMenuName(menuId: number): string {
    return this.menus.find(m => m.id === menuId)?.name || '-';
  }

  getParentName(parentId: number): string {
    return this.categories.find(c => c.id === parentId)?.name || '-';
  }
}
