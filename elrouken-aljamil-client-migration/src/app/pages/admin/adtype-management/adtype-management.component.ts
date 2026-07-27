import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MenuAdminService } from '../../../services/menu-admin.service';
import { CategoryAdminService } from '../../../services/category-admin.service';
import { AdTypeAdminService } from '../../../services/adtype-admin.service';

@Component({
  selector: 'app-adtype-management',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  templateUrl: './adtype-management.component.html',
  styleUrl: './adtype-management.component.scss'
})
export class AdtypeManagementComponent implements OnInit {
  items: any[] = [];
  filtered: any[] = [];
  categories: any[] = [];
  menus: any[] = [];
  showForm = false;
  editing = false;
  searchText = '';
  current = { id: 0, categoryId: 0, label: '', description: '', isDefault: false, displayOrder: 0, isActive: true };
  constructor(private menuAdminService: MenuAdminService, private categoryAdminService: CategoryAdminService, private adTypeAdminService: AdTypeAdminService) {}

  ngOnInit() {
    this.load();
    this.categoryAdminService.getAll().subscribe(d => this.categories = d);
    this.menuAdminService.getAll().subscribe(d => this.menus = d);
  }

  load() {
    this.adTypeAdminService.getAll().subscribe(d => {
      this.items = d;
      this.applyFilter();
    });
  }

  applyFilter() {
    const s = this.searchText.toLowerCase().trim();
    this.filtered = s ? this.items.filter(i => i.label.toLowerCase().includes(s) || i.description.toLowerCase().includes(s)) : [...this.items];
  }

  save() {
    const payload = { ...this.current, categoryId: Number(this.current.categoryId) };
    const op = this.editing
      ? this.adTypeAdminService.update(payload.id, payload)
      : this.adTypeAdminService.create(payload);
    op.subscribe(() => { this.load(); this.cancel(); });
  }

  edit(item: any) {
    this.current = { ...item };
    this.editing = true;
    this.showForm = true;
  }

  delete(id: number) {
    if (confirm('Supprimer ce type ?')) {
      this.adTypeAdminService.delete(id).subscribe(() => this.load());
    }
  }

  cancel() {
    this.showForm = false;
    this.editing = false;
    this.current = { id: 0, categoryId: 0, label: '', description: '', isDefault: false, displayOrder: 0, isActive: true };
  }

  getCategoryName(catId: number): string {
    return this.categories.find(c => c.id === catId)?.name || '-';
  }

  getMenuName(menuId: number): string {
    return this.menus.find(m => m.id === menuId)?.name || '-';
  }

  getMenuNameFromCategory(catId: number): string {
    const cat = this.categories.find(c => c.id === catId);
    return cat ? this.getMenuName(cat.menuId) : '-';
  }
}
