import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-annonce-detail',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule, RouterLink],
  templateUrl: './annonce-detail.component.html',
  styleUrl: './annonce-detail.component.scss'
})
export class AnnonceDetailComponent implements OnInit {
  annonce: any = null;
  isFavorited = false;
  messageText = '';
  messageSent = false;
  private apiUrl = 'https://localhost:7283/api/annonces';

  constructor(private http: HttpClient, private route: ActivatedRoute, private authService: AuthService) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadAnnonce(+id);
      this.trackView(+id);
    }
  }

  loadAnnonce(id: number) {
    this.http.get<any>(`${this.apiUrl}/${id}`).subscribe(data => this.annonce = data);
  }

  trackView(id: number) {
    const userId = this.authService.getUser()?.userId || 'anonymous-' + Date.now();
    this.http.post(`${this.apiUrl}/${id}/view`, { userId }).subscribe();
  }

  toggleFavorite() {
    if (!this.annonce) return;
    const userId = this.authService.getUser()?.userId || 'anonymous';
    this.http.post<any>(`${this.apiUrl}/${this.annonce.id}/favorite`, { userId }).subscribe(res => {
      this.isFavorited = res.favorited;
      if (res.favorited) this.annonce.favorites++;
      else this.annonce.favorites--;
    });
  }

  sendMessage() {
    if (!this.messageText.trim() || !this.annonce) return;
    const user = this.authService.getUser();
    this.http.post(`${this.apiUrl}/${this.annonce.id}/message`, {
      senderId: user?.userId || 'anonymous',
      senderEmail: user?.email || '',
      content: this.messageText
    }).subscribe(() => {
      this.messageSent = true;
      this.messageText = '';
    });
  }

  getPhone(): string {
    if (this.annonce?.hidePhone) return 'Numéro masqué';
    return this.annonce?.phone || '';
  }

  getEncodedTitle(): string {
    return encodeURIComponent(this.annonce?.title?.substring(0, 20) || 'Annonce');
  }
}
