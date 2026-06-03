import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-menu-management',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  templateUrl: './menu-management.component.html',
  styleUrl: './menu-management.component.scss'
})
export class MenuManagementComponent implements OnInit {
  menus: any[] = [];
  filteredMenus: any[] = [];
  paginatedMenus: any[] = [];
  showForm = false;
  editingMenu = false;
  searchText = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  pages: number[] = [];
  currentMenu = { id: 0, name: '', slug: '', icon: '', displayOrder: 0, isActive: true };
  private apiUrl = 'https://localhost:7283/api/menus';

  constructor(private http: HttpClient) {}

  ngOnInit() { this.loadMenus(); }

  loadMenus() {
    this.http.get<any[]>(this.apiUrl).subscribe(data => {
      this.menus = data;
      this.applyFilter();
    });
  }

  applyFilter() {
    const search = this.searchText.toLowerCase().trim();
    this.filteredMenus = search
      ? this.menus.filter(m => m.name.toLowerCase().includes(search) || m.slug.toLowerCase().includes(search))
      : [...this.menus];
    this.currentPage = 1;
    this.updatePagination();
  }

  updatePagination() {
    this.totalPages = Math.max(1, Math.ceil(this.filteredMenus.length / this.pageSize));
    this.pages = Array.from({ length: this.totalPages }, (_, i) => i + 1);
    const start = (this.currentPage - 1) * this.pageSize;
    this.paginatedMenus = this.filteredMenus.slice(start, start + this.pageSize);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updatePagination();
    }
  }

  exportExcel() {
    const data = this.filteredMenus.map(m => ({
      ID: m.id,
      Nom: m.name,
      Slug: m.slug,
      Icône: m.icon,
      Ordre: m.displayOrder,
      Actif: m.isActive ? 'Oui' : 'Non'
    }));
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Menus');
    const buffer = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'menus.xlsx');
  }

  save() {
    if (this.editingMenu) {
      this.http.put(`${this.apiUrl}/${this.currentMenu.id}`, this.currentMenu).subscribe(() => {
        this.loadMenus();
        this.cancel();
      });
    } else {
      this.http.post(this.apiUrl, this.currentMenu).subscribe(() => {
        this.loadMenus();
        this.cancel();
      });
    }
  }

  edit(menu: any) {
    this.currentMenu = { ...menu };
    this.editingMenu = true;
    this.showForm = true;
  }

  delete(id: number) {
    if (confirm('Supprimer ce menu ?')) {
      this.http.delete(`${this.apiUrl}/${id}`).subscribe(() => this.loadMenus());
    }
  }

  cancel() {
    this.showForm = false;
    this.editingMenu = false;
    this.currentMenu = { id: 0, name: '', slug: '', icon: '', displayOrder: 0, isActive: true };
  }
}
