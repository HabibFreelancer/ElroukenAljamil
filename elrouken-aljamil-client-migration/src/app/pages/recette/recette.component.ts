import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  BugReportService, BugReport, BugReportSummary, BugReportStats,
  BugSeverity, BugStatus, CreateBugReportRequest
} from '../../services/bug-report.service';
import { AuthService } from '../../services/auth.service';
import { NotificationPreferenceService, NotificationPreference } from '../../services/notification-preference.service';

type View = 'list' | 'create' | 'detail' | 'preferences';

@Component({
  selector: 'app-recette',
  standalone: true,
  imports: [NgFor, NgIf, NgClass, DatePipe, FormsModule],
  templateUrl: './recette.component.html',
  styleUrl: './recette.component.scss'
})
export class RecetteComponent implements OnInit {
  view: View = 'list';

  BugSeverity = BugSeverity;
  BugStatus = BugStatus;

  // Liste
  bugs: BugReportSummary[] = [];
  stats: BugReportStats | null = null;
  total = 0;
  page = 1;
  pageSize = 20;
  filterStatus: BugStatus | '' = '';
  filterSeverity: BugSeverity | '' = '';
  filterCategory = '';
  loading = false;

  // Détail
  selectedBug: BugReport | null = null;
  newComment = '';
  submittingComment = false;

  // Pièces jointes
  pendingFiles: File[] = [];
  uploadingAttachment = false;
  attachmentError = '';
  isDragOver = false;
  pastedImageUploading = false;
  pastedImageName = '';

  // Formulaire création
  form: CreateBugReportRequest = this.emptyForm();
  submitting = false;
  formError = '';

  // Rôle
  isAdmin = false;
  isTester = false;

  // Préférences
  preferences: NotificationPreference[] = [];
  prefSaving = false;
  prefSaved = false;

  readonly bugNotifTypes = ['BugReportCreated', 'BugStatusChanged'];
  readonly categories = ['UI', 'API', 'Authentification', 'Annonces', 'Médias', 'Messagerie', 'Recherche', 'Notifications', 'Autre'];
  readonly modules = ['Listings', 'Identity', 'Media', 'Messaging', 'Search', 'Notification', 'Gateway'];
  readonly severities = [
    { value: BugSeverity.Blocking, label: 'Bloquant' },
    { value: BugSeverity.Major,    label: 'Majeur' },
    { value: BugSeverity.Minor,    label: 'Mineur' }
  ];
  readonly statuses = [
    { value: BugStatus.New,            label: 'Nouveau' },
    { value: BugStatus.InProgress,     label: 'En cours' },
    { value: BugStatus.FixedAndToTest, label: 'Corrigé et à tester' },
    { value: BugStatus.Resolved,       label: 'Résolu' },
    { value: BugStatus.KO,             label: 'KO' },
    { value: BugStatus.Closed,         label: 'Fermé' }
  ];

  constructor(
    private bugService: BugReportService,
    private authService: AuthService,
    private prefService: NotificationPreferenceService
  ) {}

  ngOnInit() {
    this.detectRole();
    this.loadList();
    if (this.isAdmin) this.loadStats();
  }

  private detectRole() {
    const token = this.authService.getToken();
    if (!token) return;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const roles: string[] = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        ?? payload['role'] ?? [];
      const rolesArr = Array.isArray(roles) ? roles : [roles];
      this.isAdmin  = rolesArr.includes('Admin');
      this.isTester = rolesArr.includes('Tester') || rolesArr.includes('Admin');
    } catch {}
  }

  loadList() {
    this.loading = true;
    this.bugService.getAll({
      page: this.page, pageSize: this.pageSize,
      status:   this.filterStatus   !== '' ? this.filterStatus   : undefined,
      severity: this.filterSeverity !== '' ? this.filterSeverity : undefined,
      category: this.filterCategory || undefined
    }).subscribe({
      next: res => { this.bugs = res.items; this.total = res.total; this.loading = false; },
      error: () => this.loading = false
    });
  }

  loadStats() {
    this.bugService.getStats().subscribe(s => this.stats = s);
  }

  applyFilters() { this.page = 1; this.loadList(); }
  resetFilters() { this.filterStatus = ''; this.filterSeverity = ''; this.filterCategory = ''; this.applyFilters(); }
  prevPage() { if (this.page > 1) { this.page--; this.loadList(); } }
  nextPage() { if (this.page * this.pageSize < this.total) { this.page++; this.loadList(); } }
  get totalPages() { return Math.ceil(this.total / this.pageSize); }

  openDetail(id: number) {
    this.bugService.getById(id).subscribe(b => { this.selectedBug = b; this.view = 'detail'; });
  }

  openCreate() { this.form = this.emptyForm(); this.formError = ''; this.pendingFiles = []; this.attachmentError = ''; this.view = 'create'; }
  backToList()  { this.view = 'list'; this.selectedBug = null; this.loadList(); if (this.isAdmin) this.loadStats(); }

  submitCreate() {
    if (!this.form.title || !this.form.description || !this.form.category) {
      this.formError = 'Titre, description et catégorie sont obligatoires.';
      return;
    }
    this.submitting = true;
    this.formError = '';
    this.bugService.create(this.form).subscribe({
      next: res => {
        if (this.pendingFiles.length > 0) {
          this.uploadPendingFiles(res.id, () => { this.submitting = false; this.openDetail(res.id); });
        } else {
          this.submitting = false;
          this.openDetail(res.id);
        }
      },
      error: () => { this.submitting = false; this.formError = 'Erreur lors de la création.'; }
    });
  }

  private uploadPendingFiles(bugId: number, done: () => void) {
    const uploads = this.pendingFiles.map(f =>
      this.bugService.uploadAttachment(bugId, f).toPromise().catch(() => null)
    );
    Promise.all(uploads).then(() => { this.pendingFiles = []; done(); });
  }

  changeStatus(status: BugStatus) {
    if (!this.selectedBug) return;
    this.bugService.updateStatus(this.selectedBug.id, status).subscribe(() => {
      this.openDetail(this.selectedBug!.id);
    });
  }

  setTestResult(passed: boolean) {
    if (!this.selectedBug) return;
    this.bugService.setTestResult(this.selectedBug.id, passed).subscribe({
      next: () => this.openDetail(this.selectedBug!.id),
      error: (err: any) => alert(err?.error?.message ?? 'Erreur')
    });
  }

  submitComment() {
    if (!this.newComment.trim() || !this.selectedBug) return;
    this.submittingComment = true;
    this.bugService.addComment(this.selectedBug.id, this.newComment).subscribe(() => {
      this.newComment = '';
      this.submittingComment = false;
      this.openDetail(this.selectedBug!.id);
    });
  }

  // ── Pièces jointes (formulaire création) ──────────────────────────────────

  onFileDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
    const files = Array.from(event.dataTransfer?.files ?? []);
    this.addPendingFiles(files);
  }

  onDragOver(event: DragEvent) { event.preventDefault(); this.isDragOver = true; }
  onDragLeave() { this.isDragOver = false; }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) this.addPendingFiles(Array.from(input.files));
    input.value = '';
  }

  private addPendingFiles(files: File[]) {
    const allowed = ['.jpg','.jpeg','.png','.gif','.webp','.avif','.bmp','.pdf','.doc','.docx','.xls','.xlsx','.ppt','.pptx','.txt','.csv','.log'];
    this.attachmentError = '';
    for (const f of files) {
      const ext = '.' + f.name.split('.').pop()!.toLowerCase();
      if (!allowed.includes(ext)) { this.attachmentError = `Type non autorisé : ${f.name}`; continue; }
      if (f.size > 20 * 1024 * 1024) { this.attachmentError = `Fichier trop volumineux : ${f.name}`; continue; }
      if (!this.pendingFiles.find(p => p.name === f.name && p.size === f.size))
        this.pendingFiles.push(f);
    }
  }

  removePendingFile(index: number) { this.pendingFiles.splice(index, 1); }

  // ── Pièces jointes (vue détail) ───────────────────────────────────────────

  uploadAttachmentOnDetail(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || !this.selectedBug) return;
    this.uploadingAttachment = true;
    this.attachmentError = '';
    const uploads = Array.from(input.files).map(f =>
      this.bugService.uploadAttachment(this.selectedBug!.id, f).toPromise().catch(() => null)
    );
    Promise.all(uploads).then(() => {
      this.uploadingAttachment = false;
      this.openDetail(this.selectedBug!.id);
    });
    input.value = '';
  }

  onCommentPaste(event: ClipboardEvent) {
    const items = event.clipboardData?.items;
    if (!items || !this.selectedBug) return;
    for (let i = 0; i < items.length; i++) {
      if (items[i].type.startsWith('image/')) {
        event.preventDefault();
        const file = items[i].getAsFile();
        if (!file) return;
        const ext = file.type.split('/')[1] || 'png';
        const named = new File([file], `capture-${Date.now()}.${ext}`, { type: file.type });
        this.pastedImageUploading = true;
        this.pastedImageName = '';
        this.bugService.uploadAttachment(this.selectedBug.id, named).subscribe({
          next: () => {
            this.pastedImageUploading = false;
            this.pastedImageName = named.name;
            setTimeout(() => this.pastedImageName = '', 4000);
            this.openDetail(this.selectedBug!.id);
          },
          error: () => { this.pastedImageUploading = false; }
        });
        return;
      }
    }
  }

  deleteAttachment(attachmentId: number) {
    if (!this.selectedBug || !confirm('Supprimer cette pièce jointe ?')) return;
    this.bugService.deleteAttachment(this.selectedBug.id, attachmentId).subscribe(() =>
      this.openDetail(this.selectedBug!.id)
    );
  }

  // ── Utilitaires ───────────────────────────────────────────────────────────

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} o`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} Ko`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} Mo`;
  }

  fileIcon(name: string): string {
    const ext = name.split('.').pop()?.toLowerCase() ?? '';
    if (['jpg','jpeg','png','gif','webp','avif','bmp'].includes(ext)) return 'fa-image';
    if (ext === 'pdf') return 'fa-file-pdf';
    if (['doc','docx'].includes(ext)) return 'fa-file-word';
    if (['xls','xlsx'].includes(ext)) return 'fa-file-excel';
    if (['ppt','pptx'].includes(ext)) return 'fa-file-powerpoint';
    return 'fa-file-lines';
  }

  openPreferences() {
    this.prefService.getAll().subscribe(prefs => {
      this.bugNotifTypes.forEach(type => {
        if (!prefs.find(p => p.notificationType === type))
          prefs.push({ notificationType: type, emailEnabled: true, smsEnabled: false, pushEnabled: false, inAppEnabled: true });
      });
      this.preferences = prefs.filter(p => this.bugNotifTypes.includes(p.notificationType));
      this.view = 'preferences';
    });
  }

  savePreferences() {
    this.prefSaving = true;
    const saves = this.preferences.map(p => this.prefService.update(p).toPromise());
    Promise.all(saves).then(() => {
      this.prefSaving = false;
      this.prefSaved = true;
      setTimeout(() => this.prefSaved = false, 3000);
    });
  }

  prefLabel(type: string): string {
    return type === 'BugReportCreated' ? 'Nouveau ticket créé' : 'Changement de statut';
  }

  severityClass(s: BugSeverity): string {
    return { [BugSeverity.Blocking]: 'badge-red', [BugSeverity.Major]: 'badge-orange', [BugSeverity.Minor]: 'badge-blue' }[s] ?? '';
  }

  statusClass(s: BugStatus): string {
    const map: Record<BugStatus, string> = {
      [BugStatus.New]:            'badge-gray',
      [BugStatus.InProgress]:     'badge-blue',
      [BugStatus.FixedAndToTest]: 'badge-orange',
      [BugStatus.Resolved]:       'badge-green',
      [BugStatus.KO]:             'badge-red',
      [BugStatus.Closed]:         'badge-dark'
    };
    return map[s] ?? '';
  }

  private emptyForm(): CreateBugReportRequest {
    return {
      title: '', description: '', severity: BugSeverity.Major,
      category: '', subCategory: '', module: '', environment: '',
      stepsToReproduce: '', expectedBehavior: '', actualBehavior: '',
      browserInfo: ''
    };
  }
}
