import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class WorkflowAdminService {
  private api = `${environment.apiUrl}/workflow`;

  constructor(private http: HttpClient) {}

  // Workflows
  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.api);
  }

  create(workflow: any): Observable<any> {
    return this.http.post(this.api, workflow);
  }

  update(id: number, workflow: any): Observable<any> {
    return this.http.put(`${this.api}/${id}`, workflow);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }

  // Steps
  getSteps(workflowId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/${workflowId}/steps`);
  }

  createStep(workflowId: number, step: any): Observable<any> {
    return this.http.post(`${this.api}/${workflowId}/steps`, step);
  }

  updateStep(stepId: number, step: any): Observable<any> {
    return this.http.put(`${this.api}/steps/${stepId}`, step);
  }

  deleteStep(stepId: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/steps/${stepId}`);
  }

  // Fields
  getFields(stepId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/steps/${stepId}/fields`);
  }

  createField(stepId: number, field: any): Observable<any> {
    return this.http.post(`${this.api}/steps/${stepId}/fields`, field);
  }

  updateField(fieldId: number, field: any): Observable<any> {
    return this.http.put(`${this.api}/fields/${fieldId}`, field);
  }

  deleteField(fieldId: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/fields/${fieldId}`);
  }
}
