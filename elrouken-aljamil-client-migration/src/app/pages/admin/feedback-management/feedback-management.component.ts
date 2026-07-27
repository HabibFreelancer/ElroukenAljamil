import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe } from '@angular/common';
import { FeedbackAdminService } from '../../../services/feedback-admin.service';

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
  constructor(private feedbackAdminService: FeedbackAdminService) {}

  ngOnInit() { this.load(); }

  load() {
    this.feedbackAdminService.getAll().subscribe(data => this.feedbacks = data);
    this.feedbackAdminService.getStats().subscribe(data => this.stats = data);
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
