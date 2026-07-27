import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { AnnonceService } from '../../services/annonce.service';
import { MessagingService } from '../../services/messaging.service';

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
  constructor(
    private annonceService: AnnonceService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private messagingService: MessagingService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadAnnonce(+id);
      this.trackView(+id);
    }
  }

  loadAnnonce(id: number) {
    this.annonceService.getAnnonce(id).subscribe(data => this.annonce = data);
  }

  trackView(id: number) {
    const userId = this.authService.getUser()?.userId || 'anonymous-' + Date.now();
    this.annonceService.trackView(id, userId).subscribe();
  }

  toggleFavorite() {
    if (!this.annonce) return;
    this.annonceService.toggleFavorite(this.annonce.id).subscribe(res => {
      this.isFavorited = res.favorited;
      if (res.favorited) this.annonce.favorites++;
      else this.annonce.favorites--;
    });
  }

  sendMessage() {
    if (!this.messageText.trim() || !this.annonce) return;
    this.messagingService.startConversation({
      sellerId:     this.annonce.userId || '',
      sellerName:   this.annonce.email  || 'Vendeur',
      listingId:    this.annonce.id.toString(),
      listingTitle: this.annonce.title  || '',
      message:      this.messageText
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
