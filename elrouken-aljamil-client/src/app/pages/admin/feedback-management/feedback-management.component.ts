import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgFor, NgIf, DatePipe } from '@angular/common';

@Component({
  selector: 'app-feedback-management',
  standalone: true,
  imports: [NgFor, NgIf, DatePipe],
  templateUrl: './feedback-management.component.html',
  styleUrl: './feedback-management.component.scss'
})
export class FeedbackManagementComponent implements OnInit {
  feedbacks: any[] = [];
  stats: any = null;
  private apiUrl = 'https://localhost:7283/api/feedback';

  constructor(private http: HttpClient) {}

  ngOnInit() { this.load(); }

  load() {
    this.http.get<any[]>(this.apiUrl).subscribe(data => this.feedbacks = data);
    this.http.get<any>(`${this.apiUrl}/stats`).subscribe(data => this.stats = data);
  }

  getRatingLabel(rating: string): string {
    const map: any = { tres_facile: 'Très facile', facile: 'Facile', neutre: 'Neutre', difficile: 'Difficile', tres_difficile: 'Très difficile' };
    return map[rating] || rating;
  }

  getRatingClass(rating: string): string {
    const map: any = { tres_facile: 'badge-green', facile: 'badge-blue', neutre: 'badge-gray', difficile: 'badge-orange', tres_difficile: 'badge-red' };
    return map[rating] || '';
  }
}
