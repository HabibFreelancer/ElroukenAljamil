import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { AnnonceService } from '../../services/annonce.service';

@Component({
  selector: 'app-my-annonces',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule, RouterLink],
  templateUrl: './my-annonces.component.html',
  styleUrl: './my-annonces.component.scss'
})
export class MyAnnoncesComponent implements OnInit {
  annonces: any[] = [];
  filteredAnnonces: any[] = [];
  searchText = '';
  activeTab = 'published';
  sortBy = 'date';
  publishedCount = 0;
  expiredCount = 0;
  pausedCount = 0;
  showPerformanceModal = false;
  selectedAdPerf: any = null;
  showCategoryFilter = false;
  selectedCategory = '';
  categories: any[] = [];
  constructor(private annonceService: AnnonceService, private authService: AuthService) {}

  ngOnInit() {
    this.load();
    this.loadCategories();
  }

  loadCategories() {
    this.annonceService.getMenus().subscribe(menus => {
      menus.forEach(m => {
        this.annonceService.getCategoriesForDeposit(m.id).subscribe(cats => {
          cats.forEach(c => this.categories.push({ ...c, menuName: m.name }));
        });
      });
    });
  }

  load() {
    const email = this.authService.getEmail();
    const sort = this.sortBy === 'price_asc' ? 'price_asc' : this.sortBy === 'price_desc' ? 'price_desc' : 'date';
    this.annonceService.getMyAnnonces(email, sort).subscribe(data => {
      this.annonces = data;
      this.publishedCount = data.filter(a => a.status === 'published').length;
      this.expiredCount = data.filter(a => a.status === 'expired').length;
      this.pausedCount = data.filter(a => a.status === 'paused').length;
      this.applyFilter();
    });
  }

  setTab(tab: string) { this.activeTab = tab; this.applyFilter(); }

  onSortChange() { this.load(); }

  search() { this.applyFilter(); }

  applyFilter() {
    let list = this.annonces;
    if (this.activeTab === 'published') list = list.filter(a => a.status === 'published' || a.status === 'paused');
    else if (this.activeTab === 'expired') list = list.filter(a => a.status === 'expired');

    if (this.searchText.trim()) {
      const s = this.searchText.toLowerCase();
      list = list.filter(a => a.title.toLowerCase().includes(s) || a.category?.toLowerCase().includes(s));
    }

    if (this.selectedCategory) {
      list = list.filter(a => a.category?.toLowerCase() === this.selectedCategory.toLowerCase());
    }

    this.filteredAnnonces = list;
  }

  onCategoryChange() { this.applyFilter(); }
  clearCategory() { this.selectedCategory = ''; this.applyFilter(); }

  openPerformance(ad: any) {
    this.selectedAdPerf = ad;
    this.showPerformanceModal = true;
  }

  closePerformance() {
    this.showPerformanceModal = false;
    this.selectedAdPerf = null;
  }

  pauseAnnonce(id: number) {
    this.annonceService.pauseAnnonce(id).subscribe(res => {
      const ad = this.annonces.find(a => a.id === id);
      if (ad) ad.status = res.status;
      this.applyFilter();
    });
  }

  deleteAnnonce(id: number) {
    if (!confirm('Supprimer cette annonce ?')) return;
    this.annonceService.deleteAnnonce(id).subscribe(() => {
      this.annonces = this.annonces.filter(a => a.id !== id);
      this.publishedCount = this.annonces.filter(a => a.status === 'published').length;
      this.expiredCount = this.annonces.filter(a => a.status === 'expired').length;
      this.applyFilter();
    });
  }

  getTimeAgo(date: string): string {
    const diff = Date.now() - new Date(date).getTime();
    const days = Math.floor(diff / 86400000);
    if (days === 0) return "Créée aujourd'hui";
    if (days === 1) return 'Créée hier';
    return `Créée il y a ${days} jours`;
  }

  getImageUrl(ad: any): string {
    return `https://placehold.co/120x80/f0f0f0/666?text=${encodeURIComponent(ad.title?.substring(0, 10) || 'Annonce')}`;
  }
}
