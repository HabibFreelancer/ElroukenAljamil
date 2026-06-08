import { Component, OnInit, AfterViewChecked, ViewChild, ElementRef, ViewEncapsulation } from '@angular/core';
import { NgIf, NgFor, NgStyle } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { debounceTime, Subject } from 'rxjs';
import { WorkflowService, Workflow, WorkflowStep, StepField } from '../../services/workflow.service';
import { AuthService } from '../../services/auth.service';
import * as L from 'leaflet';

// Fix Leaflet default icon path issue with bundlers
const iconDefault = L.icon({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  tooltipAnchor: [16, -28],
  shadowSize: [41, 41]
});
L.Marker.prototype.options.icon = iconDefault;

@Component({
  selector: 'app-deposer-annonce',
  standalone: true,
  imports: [NgIf, NgFor, NgStyle, FormsModule, RouterLink],
  templateUrl: './deposer-annonce.component.html',
  styleUrl: './deposer-annonce.component.scss',
  encapsulation: ViewEncapsulation.None
})
export class DeposerAnnonceComponent implements OnInit {
  currentStep = 1;
  suggestedCategories: any[] = [];
  selectedCategoryId: number | null = null;
  selectedMenuId: number | null = null;
  menus: any[] = [];
  showCategoryBrowser = false;
  showAdType = false;
  adTypes: any[] = [];
  browserCategories: any[] = [];
  private searchSubject = new Subject<string>();
  private searchCache: { [key: string]: any[] } = {};
  private apiUrl = 'https://localhost:7283/api';
  categoryError = false;

  annonce = {
    category: '',
    title: '',
    description: '',
    price: null as number | null,
    location: '',
    condition: '',
    adType: 'offre'
  };

  emploiForm = {
    contract: '',
    industry: '',
    job: '',
    experience: '',
    education: '',
    workType: 'temps_plein',
    salary: null as number | null,
    poste: '',
    experienceDesc: '',
    profilVisible: true,
    address: ''
  };

  addressResults: any[] = [];
  selectedAddress = '';
  private addressSubject = new Subject<string>();
  private map: L.Map | null = null;
  private marker: L.Marker | null = null;
  private mapInitialized = false;
  posteError = false;
  experienceDescError = false;

  // Workflow
  workflow: Workflow | null = null;
  workflowSteps: WorkflowStep[] = [];
  formData: { [key: string]: any } = {};
  fieldErrors: { [key: string]: boolean } = {};
  submitted = false;
  submittedAnnonceId: number | null = null;
  submitting = false;

  contactForm = {
    email: '',
    phone: '',
    hidePhone: false
  };

  constructor(private http: HttpClient, private workflowService: WorkflowService, private authService: AuthService) {}

  ngOnInit() {
    // Restore step from sessionStorage
    const savedStep = sessionStorage.getItem('deposer_step');
    if (savedStep) {
      const step = parseInt(savedStep, 10);
      if (step === 1) {
        this.resetForm();
      } else {
        this.currentStep = step;
        const savedData = sessionStorage.getItem('deposer_data');
        if (savedData) {
          const data = JSON.parse(savedData);
          this.annonce = data.annonce || this.annonce;
          this.emploiForm = data.emploiForm || this.emploiForm;
          this.selectedCategoryId = data.selectedCategoryId || null;
          this.photos = data.photos || [];
          this.contactForm = data.contactForm || this.contactForm;
          this.showAdType = data.showAdType || false;
          this.adTypes = data.adTypes || [];
        }
      }
    }

    this.searchSubject.pipe(debounceTime(300)).subscribe(query => {
      if (query.length >= 2) {
        if (this.searchCache[query]) {
          this.suggestedCategories = this.searchCache[query];
          if (this.searchCache[query].length === 0) this.openCategoryBrowser();
        } else {
          this.http.get<any[]>(`${this.apiUrl}/annonces/suggest-categories?query=${encodeURIComponent(query)}`)
            .subscribe(data => {
              this.searchCache[query] = data;
              this.suggestedCategories = data;
              if (data.length === 0) this.openCategoryBrowser();
            });
        }
      } else {
        this.suggestedCategories = [];
      }
    });

    this.addressSubject.pipe(debounceTime(500)).subscribe(query => {
      if (query.length >= 3) {
        this.http.get<any[]>(`https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&countrycodes=tn&limit=5`)
          .subscribe(data => this.addressResults = data);
      } else {
        this.addressResults = [];
      }
    });

    this.initGeolocation();

    // Pre-fill contact form with authenticated user data
    this.contactForm.email = this.authService.getEmail();
    this.contactForm.phone = this.authService.getPhone();
  }

  resetForm() {
    sessionStorage.removeItem('deposer_step');
    sessionStorage.removeItem('deposer_data');
    this.currentStep = 1;
    this.annonce = { category: '', title: '', description: '', price: null, location: '', condition: '', adType: 'offre' };
    this.emploiForm = { contract: '', industry: '', job: '', experience: '', education: '', workType: 'temps_plein', salary: null, poste: '', experienceDesc: '', profilVisible: true, address: '' };
    this.contactForm = { email: this.authService.getEmail(), phone: this.authService.getPhone(), hidePhone: false };
    this.selectedCategoryId = null;
    this.photos = [];
    this.showAdType = false;
    this.adTypes = [];
    this.suggestedCategories = [];
    this.showCategoryBrowser = false;
    this.categoryError = false;
    this.workflow = null;
    this.workflowSteps = [];
    this.formData = {};
    this.fieldErrors = {};
    this.submitted = false;
    this.submittedAnnonceId = null;
  }

  private saveState() {
    sessionStorage.setItem('deposer_step', this.currentStep.toString());
    sessionStorage.setItem('deposer_data', JSON.stringify({
      annonce: this.annonce,
      emploiForm: this.emploiForm,
      selectedCategoryId: this.selectedCategoryId,
      photos: this.photos,
      contactForm: this.contactForm,
      showAdType: this.showAdType,
      adTypes: this.adTypes
    }));
  }

  private initGeolocation() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(pos => {
        const { latitude, longitude } = pos.coords;
        this.http.get<any>(`https://nominatim.openstreetmap.org/reverse?lat=${latitude}&lon=${longitude}&format=json`)
          .subscribe(data => {
            if (data && data.display_name) {
              this.emploiForm.address = data.display_name;
              this.selectedAddress = data.display_name;
            }
          });
      });
    }
  }

  onTitleChange() {
    this.searchSubject.next(this.annonce.title);
  }

  selectSuggestedCategory(cat: any) {
    this.selectedCategoryId = cat.categoryId;
    this.annonce.category = cat.menuName + ' > ' + cat.categoryName;
    this.categoryError = false;
    this.showCategoryBrowser = false;
    this.checkAdType(cat.categoryName, cat.menuName);
    this.loadWorkflow(cat.categoryId);
  }

  openCategoryBrowser() {
    this.showCategoryBrowser = true;
    this.suggestedCategories = [];
    if (this.menus.length === 0) {
      this.http.get<any[]>(`${this.apiUrl}/menus`).subscribe(data => this.menus = data);
    }
  }

  selectMenu(menu: any) {
    this.selectedMenuId = menu.id;
    this.http.get<any[]>(`${this.apiUrl}/categories/for-deposit/${menu.id}`)
      .subscribe(data => {
        this.browserCategories = data;
      });
  }

  selectBrowserCategory(cat: any) {
    this.selectedCategoryId = cat.id;
    const menu = this.menus.find(m => m.id === cat.menuId);
    this.annonce.category = (menu?.name || '') + ' > ' + cat.name;
    this.showCategoryBrowser = false;
    this.categoryError = false;
    this.checkAdType(cat.name, menu?.name);
    this.loadWorkflow(cat.id);
  }

  private checkAdType(categoryName: string, menuName?: string) {
    if (this.selectedCategoryId) {
      this.http.get<any[]>(`${this.apiUrl}/annonces/ad-types/${this.selectedCategoryId}`)
        .subscribe(data => {
          this.adTypes = data;
          this.showAdType = data.length > 0;
          if (data.length > 0) {
            const defaultType = data.find(t => t.isDefault);
            this.annonce.adType = defaultType ? defaultType.label : data[0].label;
          }
        });
    }
  }

  @ViewChild('cropImage') cropImageRef!: ElementRef<HTMLImageElement>;
  photos: string[] = [];
  maxPhotos = 5;
  activeSlot = 0;
  showEditor = false;
  editingSlot = 0;
  editorRotation = 0;
  isCropping = false;
  cropStart = { x: 0, y: 0 };
  cropEnd = { x: 0, y: 0 };
  isDragging = false;

  get allSlotsFilled(): boolean {
    return this.photos.filter(p => p).length >= this.maxPhotos;
  }

  get emptySlots(): number[] {
    const remaining = this.maxPhotos - this.photos.length;
    return remaining > 0 ? Array.from({ length: remaining }, (_, i) => i) : [];
  }

  selectSlot(slot: number) {
    // Si aucune photo n'est encore téléchargée, forcer le slot 0 (couverture)
    if (this.photos.length === 0) {
      this.activeSlot = 0;
    } else {
      this.activeSlot = slot;
    }
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    if (fileInput) fileInput.click();
  }

  onSlotPhotoSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = (e: any) => {
      while (this.photos.length <= this.activeSlot) {
        this.photos.push('');
      }
      this.photos[this.activeSlot] = e.target.result;
    };
    reader.readAsDataURL(file);
    event.target.value = '';
  }

  onPhotosSelected(event: any) {
    const files: FileList = event.target.files;
    for (let i = 0; i < files.length && this.photos.length < this.maxPhotos; i++) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.photos.push(e.target.result);
      };
      reader.readAsDataURL(files[i]);
    }
    event.target.value = '';
  }

  removePhoto(index: number) {
    this.photos.splice(index, 1);
  }

  openEditor(slot: number) {
    this.editingSlot = slot;
    this.editorRotation = 0;
    this.isCropping = false;
    this.showEditor = true;
  }

  closeEditor() {
    this.showEditor = false;
    this.isCropping = false;
  }

  rotatePhoto(degrees: number) {
    this.editorRotation += degrees;
  }

  cropPhoto() {
    this.isCropping = !this.isCropping;
    if (this.isCropping) {
      this.cropStart = { x: 25, y: 25 };
      this.cropEnd = { x: 75, y: 75 };
    }
  }

  onCropMouseDown(event: MouseEvent) {
    if (!this.isCropping) return;
    event.preventDefault();
    const rect = (event.currentTarget as HTMLElement).getBoundingClientRect();
    this.isDragging = true;
    this.cropStart = {
      x: ((event.clientX - rect.left) / rect.width) * 100,
      y: ((event.clientY - rect.top) / rect.height) * 100
    };
    this.cropEnd = { ...this.cropStart };
  }

  onCropMouseMove(event: MouseEvent) {
    if (!this.isDragging || !this.isCropping) return;
    event.preventDefault();
    const rect = (event.currentTarget as HTMLElement).getBoundingClientRect();
    this.cropEnd = {
      x: Math.min(100, Math.max(0, ((event.clientX - rect.left) / rect.width) * 100)),
      y: Math.min(100, Math.max(0, ((event.clientY - rect.top) / rect.height) * 100))
    };
  }

  onCropMouseUp(event: MouseEvent) {
    this.isDragging = false;
  }

  get cropStyle() {
    const left = Math.min(this.cropStart.x, this.cropEnd.x);
    const top = Math.min(this.cropStart.y, this.cropEnd.y);
    const width = Math.abs(this.cropEnd.x - this.cropStart.x);
    const height = Math.abs(this.cropEnd.y - this.cropStart.y);
    return { left: left + '%', top: top + '%', width: width + '%', height: height + '%' };
  }

  saveEdit() {
    const img = new Image();
    img.src = this.photos[this.editingSlot];
    img.onload = () => {
      const canvas = document.createElement('canvas');
      const ctx = canvas.getContext('2d')!;

      if (this.isCropping && Math.abs(this.cropEnd.x - this.cropStart.x) > 5 && Math.abs(this.cropEnd.y - this.cropStart.y) > 5) {
        // Calculer les coordonnées de recadrage
        const left = Math.min(this.cropStart.x, this.cropEnd.x) / 100 * img.naturalWidth;
        const top = Math.min(this.cropStart.y, this.cropEnd.y) / 100 * img.naturalHeight;
        const width = Math.abs(this.cropEnd.x - this.cropStart.x) / 100 * img.naturalWidth;
        const height = Math.abs(this.cropEnd.y - this.cropStart.y) / 100 * img.naturalHeight;
        canvas.width = width;
        canvas.height = height;
        ctx.drawImage(img, left, top, width, height, 0, 0, width, height);
        this.photos[this.editingSlot] = canvas.toDataURL('image/jpeg', 0.9);
      } else if (this.editorRotation % 360 !== 0) {
        const rad = (this.editorRotation * Math.PI) / 180;
        const angle = ((this.editorRotation % 360) + 360) % 360;
        const isVertical = angle === 90 || angle === 270;
        canvas.width = isVertical ? img.naturalHeight : img.naturalWidth;
        canvas.height = isVertical ? img.naturalWidth : img.naturalHeight;
        ctx.translate(canvas.width / 2, canvas.height / 2);
        ctx.rotate(rad);
        ctx.drawImage(img, -img.naturalWidth / 2, -img.naturalHeight / 2);
        this.photos[this.editingSlot] = canvas.toDataURL('image/jpeg', 0.9);
      }
      this.closeEditor();
    };
  }

  searchAddress() {
    this.addressSubject.next(this.emploiForm.address);
  }

  loadWorkflow(categoryId: number) {
    this.workflowService.getWorkflowByCategory(categoryId).subscribe({
      next: (wf) => {
        this.workflow = wf;
        this.workflowSteps = wf.steps;
        // Initialize formData with default values
        this.formData = {};
        for (const step of wf.steps) {
          for (const field of step.fields) {
            this.formData[field.fieldKey] = field.defaultValue || '';
          }
        }
        // Pre-fill poste with title
        if (this.formData['poste'] !== undefined) {
          this.formData['poste'] = this.annonce.title;
        }
        // Pre-fill email/phone from auth
        if (this.formData['email'] !== undefined) {
          this.formData['email'] = this.authService.getEmail();
        }
        if (this.formData['phone'] !== undefined) {
          this.formData['phone'] = this.authService.getPhone();
        }
        this.saveState();
      },
      error: () => {
        this.workflow = null;
        this.workflowSteps = [];
      }
    });
  }

  getWorkflowStep(stepIndex: number): WorkflowStep | null {
    if (!this.workflowSteps.length) return null;
    // Step 1 = title (always), step 2+ comes from workflow (skip "title" step in workflow)
    const dynamicSteps = this.workflowSteps.filter(s => s.stepKey !== 'title');
    const idx = stepIndex - 2; // starts at step 2 in the UI
    return idx >= 0 && idx < dynamicSteps.length ? dynamicSteps[idx] : null;
  }

  getTotalSteps(): number {
    if (this.workflow && this.workflowSteps.length > 0) {
      return this.workflowSteps.length;
    }
    return this.isEmploiCategory() ? 8 : 4;
  }

  validateCurrentStep(): boolean {
    if (this.currentStep === 1) {
      if (!this.selectedCategoryId || !this.annonce.category) {
        this.categoryError = true;
        return false;
      }
      return true;
    }
    // Validate dynamic workflow step fields
    const step = this.getWorkflowStep(this.currentStep);
    if (step) {
      let valid = true;
      for (const field of step.fields) {
        if (field.isRequired && !this.formData[field.fieldKey]?.toString().trim()) {
          this.fieldErrors[field.fieldKey] = true;
          valid = false;
        }
      }
      return valid;
    }
    return true;
  }

  clearFieldError(fieldKey: string) {
    this.fieldErrors[fieldKey] = false;
  }

  onAddressInput(fieldKey: string) {
    const value = this.formData[fieldKey];
    if (value && value.length >= 3) {
      this.addressSubject.next(value);
    } else {
      this.addressResults = [];
    }
  }

  selectAddressDynamic(result: any, fieldKey: string) {
    this.formData[fieldKey] = result.display_name;
    this.selectedAddress = result.display_name;
    this.addressResults = [];
    const lat = parseFloat(result.lat);
    const lon = parseFloat(result.lon);
    this.showMap(lat, lon);
  }

  selectAddress(result: any) {
    this.emploiForm.address = result.display_name;
    this.selectedAddress = result.display_name;
    this.addressResults = [];
    const lat = parseFloat(result.lat);
    const lon = parseFloat(result.lon);
    this.showMap(lat, lon);
  }

  clearMap() {
    this.selectedAddress = '';
    if (this.map) {
      this.map.remove();
      this.map = null;
      this.mapInitialized = false;
    }
  }

  private showMap(lat: number, lon: number) {
    setTimeout(() => {
      const container = document.getElementById('map');
      if (!container) return;

      if (this.map) {
        this.map.setView([lat, lon], 14);
        if (this.marker) this.marker.setLatLng([lat, lon]);
        else this.marker = L.marker([lat, lon]).addTo(this.map);
        this.map.invalidateSize({animate: false});
      } else {
        this.map = L.map(container, { scrollWheelZoom: true }).setView([lat, lon], 14);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
          attribution: '&copy; OpenStreetMap'
        }).addTo(this.map);
        this.marker = L.marker([lat, lon]).addTo(this.map);
        // Force multiple invalidateSize to ensure tiles load correctly
        setTimeout(() => { this.map!.invalidateSize({animate: false}); }, 100);
        setTimeout(() => { this.map!.invalidateSize({animate: false}); }, 400);
      }
    }, 200);
  }

  get isVehiculeCategory(): boolean {
    return this.annonce.category.toLowerCase().includes('véhicules') || this.annonce.category.toLowerCase().includes('vehicules');
  }

  draggedSlot: number | null = null;

  onDragStart(event: DragEvent, slot: number) {
    this.draggedSlot = slot;
    if (event.dataTransfer) { event.dataTransfer.effectAllowed = 'move'; }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer) { event.dataTransfer.dropEffect = 'move'; }
  }

  onDrop(event: DragEvent, targetSlot: number) {
    event.preventDefault();
    if (this.draggedSlot !== null && this.draggedSlot !== targetSlot) {
      const temp = this.photos[this.draggedSlot];
      this.photos[this.draggedSlot] = this.photos[targetSlot] || '';
      this.photos[targetSlot] = temp;
      while (this.photos.length > 0 && this.photos[this.photos.length - 1] === '') { this.photos.pop(); }
    }
    this.draggedSlot = null;
  }

  getProgressPercent(): number {
    return (this.currentStep / this.getTotalSteps()) * 100;
  }

  isLastStep(): boolean {
    return this.currentStep === this.getTotalSteps();
  }

  nextStep() {
    if (!this.validateCurrentStep()) return;

    if (this.workflow && this.workflowSteps.length > 0) {
      if (this.currentStep === 1) {
        this.currentStep = 2;
      } else if (!this.isLastStep()) {
        this.currentStep++;
      }
    } else {
      if (this.currentStep === 1 && (this.isEmploiCategory() || this.isVehiculeCategory)) {
        if (this.isEmploiCategory()) {
          this.emploiForm.poste = this.annonce.title;
        }
        this.currentStep = 3;
      } else if (this.currentStep === 6 && this.isEmploiCategory()) {
        this.posteError = !this.emploiForm.poste.trim();
        this.experienceDescError = !this.emploiForm.experienceDesc.trim();
        if (this.posteError || this.experienceDescError) return;
        this.currentStep++;
      } else if (!this.isLastStep()) {
        this.currentStep++;
      }
    }
    this.saveState();
    this.onStepChanged();
  }

  prevStep() {
    if (this.workflow && this.workflowSteps.length > 0) {
      if (this.currentStep === 2) {
        this.resetForm();
      } else if (this.currentStep > 1) {
        this.currentStep--;
        this.saveState();
      }
    } else {
      if (this.currentStep === 3 && (this.isEmploiCategory() || this.isVehiculeCategory)) {
        this.resetForm();
      } else if (this.currentStep === 2) {
        this.resetForm();
      } else if (this.currentStep > 1) {
        this.currentStep--;
        this.saveState();
      }
    }
    this.onStepChanged();
  }

  private onStepChanged() {
    // Re-display map if returning to a location step with address already filled
    setTimeout(() => {
      const mapContainer = document.getElementById('map');
      if (!mapContainer) return;

      // Destroy old map instance if container changed
      if (this.map) {
        this.map.remove();
        this.map = null;
        this.marker = null;
      }

      // Check if address is filled (dynamic workflow or legacy)
      const address = this.formData['address'] || this.emploiForm.address;
      if (address && this.selectedAddress) {
        // Re-geocode to get coordinates
        this.http.get<any[]>(`https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(address)}&format=json&limit=1`)
          .subscribe(results => {
            if (results && results.length > 0) {
              this.showMap(parseFloat(results[0].lat), parseFloat(results[0].lon));
            }
          });
      }
    }, 300);
  }

  isEmploiCategory(): boolean {
    return this.annonce.category.toLowerCase().includes('emploi');
  }

  submit() {
    this.submitting = true;
    const payload: any = {
      title: this.annonce.title,
      categoryId: this.selectedCategoryId,
      adType: this.annonce.adType,
      description: this.annonce.description || this.formData['experienceDesc'] || '',
      price: this.annonce.price || this.formData['salary'] || 0,
      condition: this.annonce.condition,
      location: this.annonce.location || this.formData['address'] || this.emploiForm.address || '',
      phone: this.contactForm.phone || this.formData['phone'] || '',
      email: this.contactForm.email || this.formData['email'] || '',
      hidePhone: this.contactForm.hidePhone || this.formData['hidePhone'] === 'true',
      extraData: this.workflow ? this.formData : {
        ...this.emploiForm,
        contactPhone: this.contactForm.phone,
        contactEmail: this.contactForm.email
      }
    };

    this.http.post<any>(`${this.apiUrl}/annonces`, payload).subscribe({
      next: (res) => {
        this.submitted = true;
        this.submittedAnnonceId = res.id;
        this.submitting = false;
        sessionStorage.removeItem('deposer_step');
        sessionStorage.removeItem('deposer_data');
      },
      error: () => {
        this.submitting = false;
        alert('Une erreur est survenue. Veuillez réessayer.');
      }
    });
  }
}
