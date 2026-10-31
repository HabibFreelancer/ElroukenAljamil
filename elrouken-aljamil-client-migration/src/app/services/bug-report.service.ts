import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export enum BugSeverity { Blocking = 1, Major = 2, Minor = 3 }
export enum BugStatus { New = 1, InProgress = 2, FixedAndToTest = 3, Resolved = 4, KO = 5, Closed = 6 }

export interface BugReportSummary {
  id: number;
  title: string;
  severity: BugSeverity;
  severityLabel: string;
  status: BugStatus;
  statusLabel: string;
  category: string;
  module?: string;
  reportedByUserName: string;
  createdAt: string;
}

export interface BugComment {
  id: number;
  authorId: string;
  authorName: string;
  content: string;
  createdAt: string;
}

export interface BugAttachment {
  id: number;
  fileName: string;
  storageUrl: string;
  contentType: string;
  fileSize: number;
  uploadedByUserId: string;
  createdAt: string;
}

export interface BugReport {
  id: number;
  title: string;
  description: string;
  severity: BugSeverity;
  severityLabel: string;
  status: BugStatus;
  statusLabel: string;
  category: string;
  subCategory?: string;
  module?: string;
  environment?: string;
  stepsToReproduce?: string;
  expectedBehavior?: string;
  actualBehavior?: string;
  screenshotUrl?: string;
  browserInfo?: string;
  reportedByUserId: string;
  reportedByUserName: string;
  assignedToUserId?: string;
  createdAt: string;
  updatedAt?: string;
  closedAt?: string;
  comments: BugComment[];
  attachments: BugAttachment[];
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface BugReportStats {
  total: number;
  new: number;
  inProgress: number;
  fixedAndToTest: number;
  resolved: number;
  kO: number;
  closed: number;
  blocking: number;
  major: number;
  minor: number;
}

export interface CreateBugReportRequest {
  title: string;
  description: string;
  severity: BugSeverity;
  category: string;
  subCategory?: string;
  module?: string;
  environment?: string;
  stepsToReproduce?: string;
  expectedBehavior?: string;
  actualBehavior?: string;
  screenshotUrl?: string;
  browserInfo?: string;
}

@Injectable({ providedIn: 'root' })
export class BugReportService {
  private api = `${environment.apiUrl}/bugreports`;

  constructor(private http: HttpClient) {}

  getAll(filters: {
    page?: number; pageSize?: number;
    status?: BugStatus; severity?: BugSeverity; category?: string;
  } = {}): Observable<PagedResult<BugReportSummary>> {
    let params = new HttpParams()
      .set('page', filters.page ?? 1)
      .set('pageSize', filters.pageSize ?? 20);
    if (filters.status != null) params = params.set('status', filters.status);
    if (filters.severity != null) params = params.set('severity', filters.severity);
    if (filters.category) params = params.set('category', filters.category);
    return this.http.get<PagedResult<BugReportSummary>>(this.api, { params });
  }

  getById(id: number): Observable<BugReport> {
    return this.http.get<BugReport>(`${this.api}/${id}`);
  }

  getStats(): Observable<BugReportStats> {
    return this.http.get<BugReportStats>(`${this.api}/stats`);
  }

  create(payload: CreateBugReportRequest): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.api, payload);
  }

  updateStatus(id: number, status: BugStatus, assignedToUserId?: string): Observable<void> {
    return this.http.put<void>(`${this.api}/${id}/status`, { status, assignedToUserId });
  }

  setTestResult(id: number, passed: boolean): Observable<void> {
    return this.http.put<void>(`${this.api}/${id}/test-result`, { passed });
  }

  addComment(id: number, content: string): Observable<void> {
    return this.http.post<void>(`${this.api}/${id}/comments`, { content });
  }

  uploadAttachment(id: number, file: File): Observable<void> {
    const fd = new FormData();
    fd.append('file', file);
    return this.http.post<void>(`${this.api}/${id}/attachments`, fd);
  }

  deleteAttachment(bugId: number, attachmentId: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${bugId}/attachments/${attachmentId}`);
  }
}
