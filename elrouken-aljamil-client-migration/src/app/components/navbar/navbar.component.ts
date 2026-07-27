import { Component, OnInit } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { NgIf, NgFor, NgSwitch, NgSwitchCase } from '@angular/common';
import { MenuService, Menu, Category } from '../../services/menu.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, NgSwitch, NgSwitchCase],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  activePanel: string | null = null;
  menus: Menu[] = [];
  menuCategories: { [key: string]: Category[] } = {};
  menuColumns: { [key: string]: Category[][] } = {};

  constructor(private menuService: MenuService, private router: Router) {}

  ngOnInit() {
    if (this.router.url.startsWith('/deposer') || this.router.url.startsWith('/admin')) {
      return;
    }
    this.loadMenus();
  }

  loadMenus() {
    if (this.menus.length > 0) return;
    this.menuService.getMenus().subscribe(menus => {
      this.menus = menus;
      menus.forEach(menu => {
        this.menuService.getCategoriesByMenu(menu.id).subscribe(categories => {
          const rootCategories = categories.filter(c => !c.parentCategoryId);
          rootCategories.forEach(cat => {
            cat.subCategories = categories.filter(c => c.parentCategoryId === cat.id);
          });
          this.menuCategories[menu.slug] = rootCategories;
          this.menuColumns[menu.slug] = this.splitIntoColumns(rootCategories, 3);
        });
      });
    });
  }

  private splitIntoColumns(items: Category[], maxCols: number): Category[][] {
    const cols: Category[][] = [];
    const colCount = Math.min(maxCols, Math.max(1, Math.ceil(items.length / 3)));
    const perCol = Math.ceil(items.length / colCount);
    for (let i = 0; i < colCount; i++) {
      cols.push(items.slice(i * perCol, (i + 1) * perCol));
    }
    return cols;
  }
}
