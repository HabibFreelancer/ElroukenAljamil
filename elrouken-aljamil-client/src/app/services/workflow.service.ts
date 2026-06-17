import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface FieldOption {
  value: string;
  label: string;
}

export interface StepField {
  id: number;
  fieldKey: string;
  label: string;
  fieldType: string;
  placeholder: string;
  options: FieldOption[];
  defaultValue: string;
  suffix: string;
  helperText: string;
  isRequired: boolean;
  displayOrder: number;
  maxLength: number | null;
  visibilityCondition: string;
}

export interface WorkflowStep {
  id: number;
  stepOrder: number;
  title: string;
  subtitle: string;
  stepKey: string;
  isRequired: boolean;
  fields: StepField[];
}

export interface Workflow {
  id: number;
  categoryId: number;
  name: string;
  description: string;
  steps: WorkflowStep[];
}

@Injectable({ providedIn: 'root' })
export class WorkflowService {
  private apiUrl = 'https://localhost:7283/api/workflow';

  constructor(private http: HttpClient) {}

  getWorkflowByCategory(categoryId: number): Observable<Workflow> {
    return this.http.get<Workflow>(`${this.apiUrl}/${categoryId}`);
  }

  getAllWorkflows(): Observable<Workflow[]> {
    return this.http.get<Workflow[]>(this.apiUrl);
  }
}
