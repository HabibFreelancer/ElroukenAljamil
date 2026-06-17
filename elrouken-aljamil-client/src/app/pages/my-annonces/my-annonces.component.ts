import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgFor, NgIf, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-my-annonces',
  standalone: true,
  imports: [NgFor, NgIf, DatePipe, FormsModule, RouterLink],
  templateUrl: './my-annonces.component.html',
  styleUrl: './my-annonces.component.scss'
})
export class MyAnnoncesComponent implements OnInit {
  annonces: any[] = [];
  filteredAnnonces: any[] = [];
  searchText = '';
  activeTab = 'published';
  publishedCount = 0;
  expiredCount = 0;
  private apiUrl = 'https://localhost:7283/api/annonces/my';

  constructor(private http: HttpClient, private authService: AuthService) {}

  ngOnInit() { this.load(); }

  load() {
    const email = this.authService.getEmail();
    this.http.get<any[]>(`${this.apiUrl}?email=${encodeURIComponent(email)}`).subscribe(data => {
      this.annonces = data;
      this.publishedCount = data.filter(a => a.status === 'published').length;
      this.expiredCount = data.filter(a => a.status === 'expired').length;
      this.applyFilter();
    });
  }

  setTab(tab: string) {
    this.activeTab = tab;
    this.applyFilter();
  }

  search() { this.applyFilter(); }

  applyFilter() {
    let list = this.annonces;
    if (this.activeTab === 'published') list = list.filter(a => a.status === 'published');
    else if (this.activeTab === 'expired') list = list.filter(a => a.status === 'expired');

    if (this.searchText.trim()) {
      const s = this.searchText.toLowerCase();
      list = list.filter(a => a.title.toLowerCase().includes(s) || a.category?.toLowerCase().includes(s));
    }
    this.filteredAnnonces = list;
  }

  getTimeAgo(date: string): string {
    const diff = Date.now() - new Date(date).getTime();
    const days = Math.floor(diff / 86400000);
    if (days === 0) return "Créée aujourd'hui";
    if (days === 1) return 'Créée hier';
    return `Créée il y a ${days} jours`;
  }
}
