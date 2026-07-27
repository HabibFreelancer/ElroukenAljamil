import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { MenuAdminService } from '../../../services/menu-admin.service';

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
  constructor(private menuAdminService: MenuAdminService) {}

  ngOnInit() { this.loadMenus(); }

  loadMenus() {
    this.menuAdminService.getAll().subscribe(data => {
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
    const op = this.editingMenu
      ? this.menuAdminService.update(this.currentMenu.id, this.currentMenu)
      : this.menuAdminService.create(this.currentMenu);
    op.subscribe(() => { this.loadMenus(); this.cancel(); });
  }

  edit(menu: any) {
    this.currentMenu = { ...menu };
    this.editingMenu = true;
    this.showForm = true;
  }

  delete(id: number) {
    if (confirm('Supprimer ce menu ?')) {
      this.menuAdminService.delete(id).subscribe(() => this.loadMenus());
    }
  }

  cancel() {
    this.showForm = false;
    this.editingMenu = false;
    this.currentMenu = { id: 0, name: '', slug: '', icon: '', displayOrder: 0, isActive: true };
  }
}
