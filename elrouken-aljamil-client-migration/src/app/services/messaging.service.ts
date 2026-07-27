import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface StartConversationRequest {
  sellerId: string;
  sellerName: string;
  listingId: string;
  listingTitle: string;
  message: string;
}

export interface ConversationDetailDto {
  id: string;
  buyerId: string;
  buyerName: string;
  sellerId: string;
  sellerName: string;
  listingId: string;
  listingTitle: string;
  status: string;
  messages: MessageDto[];
  createdAt: string;
}

export interface MessageDto {
  id: string;
  senderId: string;
  senderName: string;
  content: string;
  sentAt: string;
  isRead: boolean;
  isEdited: boolean;
  isDeleted: boolean;
  isMine: boolean;
}

@Injectable({ providedIn: 'root' })
export class MessagingService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  startConversation(request: StartConversationRequest): Observable<string> {
    return this.http.post<string>(`${this.api}/conversations`, request);
  }

  getConversation(conversationId: string): Observable<ConversationDetailDto> {
    return this.http.get<ConversationDetailDto>(`${this.api}/conversations/${conversationId}`);
  }

  sendMessage(conversationId: string, content: string): Observable<MessageDto> {
    return this.http.post<MessageDto>(`${this.api}/conversations/${conversationId}/messages`, { content });
  }

  getConversations(page = 1, pageSize = 20): Observable<any> {
    return this.http.get<any>(`${this.api}/conversations?page=${page}&pageSize=${pageSize}`);
  }

  markAsRead(conversationId: string): Observable<void> {
    return this.http.post<void>(`${this.api}/conversations/${conversationId}/read`, {});
  }
}
