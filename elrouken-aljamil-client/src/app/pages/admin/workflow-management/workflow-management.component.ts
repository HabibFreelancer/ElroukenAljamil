import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgFor, NgIf, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-workflow-management',
  standalone: true,
  imports: [NgFor, NgIf, NgClass, FormsModule],
  templateUrl: './workflow-management.component.html',
  styleUrl: './workflow-management.component.scss'
})
export class WorkflowManagementComponent implements OnInit {
  workflows: any[] = [];
  filteredWorkflows: any[] = [];
  categories: any[] = [];
  private apiUrl = 'https://localhost:7283/api/workflow';
  searchText = '';
  activeView: 'workflows' | 'steps' | 'fields' = 'workflows';

  // Workflow
  showWorkflowForm = false;
  editingWorkflow = false;
  currentWorkflow = { id: 0, categoryId: 0, name: '', description: '', isActive: true };

  // Steps
  selectedWorkflow: any = null;
  steps: any[] = [];
  showStepForm = false;
  editingStep = false;
  currentStep = { id: 0, stepOrder: 1, title: '', subtitle: '', stepKey: '', isRequired: true, isActive: true };

  // Fields
  selectedStep: any = null;
  fields: any[] = [];
  showFieldForm = false;
  editingField = false;
  currentField = { id: 0, fieldKey: '', label: '', fieldType: 'text', placeholder: '', options: '', defaultValue: '', suffix: '', helperText: '', isRequired: false, displayOrder: 1, isActive: true, maxLength: null as number | null };

  fieldTypes = ['text', 'number', 'select', 'textarea', 'radio', 'pills', 'toggle', 'address', 'email', 'phone'];
  stepKeys = ['title', 'photos', 'details', 'salary', 'description', 'location', 'contact', 'recap', 'custom'];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadWorkflows();
    this.loadCategories();
  }

  // === Data loading ===
  loadWorkflows() {
    this.http.get<any[]>(this.apiUrl).subscribe(data => {
      this.workflows = data;
      this.applyFilter();
    });
  }

  loadCategories() {
    this.http.get<any[]>('https://localhost:7283/api/menus').subscribe(menus => {
      this.categories = [];
      menus.forEach(m => {
        this.http.get<any[]>(`https://localhost:7283/api/categories/for-deposit/${m.id}`).subscribe(cats => {
          cats.forEach(c => this.categories.push({ ...c, menuName: m.name }));
        });
      });
    });
  }

  applyFilter() {
    const s = this.searchText.toLowerCase().trim();
    this.filteredWorkflows = s
      ? this.workflows.filter(w => w.name.toLowerCase().includes(s) || w.categoryName?.toLowerCase().includes(s))
      : [...this.workflows];
  }

  // === Stats ===
  get totalWorkflows() { return this.workflows.length; }
  get activeWorkflows() { return this.workflows.filter(w => w.isActive).length; }
  get totalSteps() { return this.steps.length; }
  get totalFields() { return this.fields.length; }

  // === Export ===
  exportWorkflows() {
    const data = this.workflows.map(w => ({ ID: w.id, Nom: w.name, Catégorie: w.categoryName, Étapes: w.stepsCount, Actif: w.isActive ? 'Oui' : 'Non' }));
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Workflows');
    const buffer = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'workflows.xlsx');
  }

  // === Breadcrumb ===
  goToWorkflows() {
    this.activeView = 'workflows';
    this.selectedWorkflow = null;
    this.selectedStep = null;
    this.steps = [];
    this.fields = [];
  }

  goToSteps() {
    this.activeView = 'steps';
    this.selectedStep = null;
    this.fields = [];
  }

  // === Workflow CRUD ===
  saveWorkflow() {
    const payload = { ...this.currentWorkflow, categoryId: +this.currentWorkflow.categoryId };
    if (this.editingWorkflow) {
      this.http.put(`${this.apiUrl}/${this.currentWorkflow.id}`, payload).subscribe(() => { this.loadWorkflows(); this.cancelWorkflow(); });
    } else {
      this.http.post(this.apiUrl, payload).subscribe(() => { this.loadWorkflows(); this.cancelWorkflow(); });
    }
  }

  editWorkflow(wf: any) {
    this.currentWorkflow = { id: wf.id, categoryId: wf.categoryId, name: wf.name, description: wf.description || '', isActive: wf.isActive };
    this.editingWorkflow = true;
    this.showWorkflowForm = true;
  }

  toggleWorkflowActive(wf: any) {
    this.http.put(`${this.apiUrl}/${wf.id}`, { ...wf, isActive: !wf.isActive }).subscribe(() => this.loadWorkflows());
  }

  deleteWorkflow(id: number) {
    if (confirm('Supprimer ce workflow et toutes ses étapes/champs ?')) {
      this.http.delete(`${this.apiUrl}/${id}`).subscribe(() => {
        this.loadWorkflows();
        if (this.selectedWorkflow?.id === id) this.goToWorkflows();
      });
    }
  }

  cancelWorkflow() {
    this.showWorkflowForm = false;
    this.editingWorkflow = false;
    this.currentWorkflow = { id: 0, categoryId: 0, name: '', description: '', isActive: true };
  }

  selectWorkflow(wf: any) {
    this.selectedWorkflow = wf;
    this.activeView = 'steps';
    this.selectedStep = null;
    this.fields = [];
    this.http.get<any[]>(`${this.apiUrl}/${wf.id}/steps`).subscribe(data => this.steps = data);
  }

  duplicateWorkflow(wf: any) {
    const payload = { categoryId: wf.categoryId, name: wf.name + ' (copie)', description: wf.description, isActive: false };
    this.http.post(this.apiUrl, payload).subscribe(() => this.loadWorkflows());
  }

  // === Step CRUD ===
  saveStep() {
    if (this.editingStep) {
      this.http.put(`${this.apiUrl}/steps/${this.currentStep.id}`, this.currentStep).subscribe(() => { this.refreshSteps(); this.cancelStep(); });
    } else {
      this.http.post(`${this.apiUrl}/${this.selectedWorkflow.id}/steps`, this.currentStep).subscribe(() => { this.refreshSteps(); this.cancelStep(); });
    }
  }

  editStep(step: any) {
    this.currentStep = { ...step };
    this.editingStep = true;
    this.showStepForm = true;
  }

  toggleStepActive(step: any) {
    this.http.put(`${this.apiUrl}/steps/${step.id}`, { ...step, isActive: !step.isActive }).subscribe(() => this.refreshSteps());
  }

  deleteStep(id: number) {
    if (confirm('Supprimer cette étape et tous ses champs ?')) {
      this.http.delete(`${this.apiUrl}/steps/${id}`).subscribe(() => {
        this.refreshSteps();
        if (this.selectedStep?.id === id) { this.selectedStep = null; this.fields = []; }
      });
    }
  }

  cancelStep() {
    this.showStepForm = false;
    this.editingStep = false;
    this.currentStep = { id: 0, stepOrder: this.steps.length + 1, title: '', subtitle: '', stepKey: '', isRequired: true, isActive: true };
  }

  selectStep(step: any) {
    this.selectedStep = step;
    this.activeView = 'fields';
    this.http.get<any[]>(`${this.apiUrl}/steps/${step.id}/fields`).subscribe(data => this.fields = data);
  }

  refreshSteps() {
    this.http.get<any[]>(`${this.apiUrl}/${this.selectedWorkflow.id}/steps`).subscribe(data => this.steps = data);
  }

  // === Field CRUD ===
  saveField() {
    if (this.editingField) {
      this.http.put(`${this.apiUrl}/fields/${this.currentField.id}`, this.currentField).subscribe(() => { this.refreshFields(); this.cancelField(); });
    } else {
      this.http.post(`${this.apiUrl}/steps/${this.selectedStep.id}/fields`, this.currentField).subscribe(() => { this.refreshFields(); this.cancelField(); });
    }
  }

  editField(field: any) {
    this.currentField = { ...field };
    this.editingField = true;
    this.showFieldForm = true;
  }

  toggleFieldActive(field: any) {
    this.http.put(`${this.apiUrl}/fields/${field.id}`, { ...field, isActive: !field.isActive }).subscribe(() => this.refreshFields());
  }

  deleteField(id: number) {
    if (confirm('Supprimer ce champ ?')) {
      this.http.delete(`${this.apiUrl}/fields/${id}`).subscribe(() => this.refreshFields());
    }
  }

  cancelField() {
    this.showFieldForm = false;
    this.editingField = false;
    this.currentField = { id: 0, fieldKey: '', label: '', fieldType: 'text', placeholder: '', options: '', defaultValue: '', suffix: '', helperText: '', isRequired: false, displayOrder: this.fields.length + 1, isActive: true, maxLength: null };
  }

  refreshFields() {
    this.http.get<any[]>(`${this.apiUrl}/steps/${this.selectedStep.id}/fields`).subscribe(data => this.fields = data);
  }

  getFieldTypeBadgeClass(type: string): string {
    const map: any = { text: 'badge-blue', number: 'badge-green', select: 'badge-purple', textarea: 'badge-orange', radio: 'badge-teal', toggle: 'badge-pink', address: 'badge-red', email: 'badge-gray', phone: 'badge-gray' };
    return map[type] || 'badge-blue';
  }
}
