import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

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
  private apiUrl = 'https://localhost:7283/api/annonces';

  constructor(private http: HttpClient, private authService: AuthService) {}

  ngOnInit() { this.load(); }

  load() {
    const email = this.authService.getEmail();
    const sort = this.sortBy === 'price_asc' ? 'price_asc' : this.sortBy === 'price_desc' ? 'price_desc' : 'date';
    this.http.get<any[]>(`${this.apiUrl}/my?email=${encodeURIComponent(email)}&sortBy=${sort}`).subscribe(data => {
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
    this.filteredAnnonces = list;
  }

  pauseAnnonce(id: number) {
    this.http.put<any>(`${this.apiUrl}/${id}/pause`, {}).subscribe(res => {
      const ad = this.annonces.find(a => a.id === id);
      if (ad) ad.status = res.status;
      this.applyFilter();
    });
  }

  deleteAnnonce(id: number) {
    if (!confirm('Supprimer cette annonce ?')) return;
    this.http.delete(`${this.apiUrl}/${id}`).subscribe(() => {
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
