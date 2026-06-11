import { Component, OnInit, AfterViewChecked, ViewChild, ElementRef, ViewEncapsulation } from '@angular/core';
import { NgIf, NgFor, NgStyle, NgClass, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
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
  imports: [NgIf, NgFor, NgStyle, NgClass, DecimalPipe, FormsModule, RouterLink],
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
  photosError = false;
  immatError = '';
  immatSuccess = '';
  immatLoading = false;
  aiGenerating = false;

  // Workflow
  workflow: Workflow | null = null;
  workflowSteps: WorkflowStep[] = [];
  formData: { [key: string]: any } = {};
  fieldErrors: { [key: string]: boolean } = {};
  submitted = false;
  submittedAnnonceId: number | null = null;
  submitting = false;
  priceEstimate: any = null;
  priceGaugePosition = 50;
  priceRanges: { min: number; max: number }[] = [];
  showQuitModal = false;
  savingDraft = false;

  contactForm = {
    email: '',
    phone: '',
    hidePhone: false
  };

  constructor(private http: HttpClient, private workflowService: WorkflowService, private authService: AuthService, private router: Router) {}

  ngOnInit() {
    // Auth check disabled for now
    // if (!this.authService.isAuthenticated()) {
    //   this.router.navigate(['/auth']);
    //   return;
    // }

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
          this.formData = data.formData || {};
          if (data.workflowId) {
            this.loadWorkflow(data.selectedCategoryId);
          }
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
      adTypes: this.adTypes,
      formData: this.formData,
      workflowId: this.workflow?.id || null
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
      this.photosError = false;
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
        this.applyWorkflow(wf);
      },
      error: () => {
        // If not found, try parent category (17 = Voitures)
        // The backend already does this, but just in case
        this.workflow = null;
        this.workflowSteps = [];
      }
    });
  }

  private applyWorkflow(wf: Workflow) {
    this.workflow = wf;
    this.workflowSteps = wf.steps;
    // Only initialize formData if it's empty (don't overwrite restored data)
    if (Object.keys(this.formData).length === 0) {
      for (const step of wf.steps) {
        for (const field of step.fields) {
          this.formData[field.fieldKey] = field.defaultValue || '';
        }
      }
      if (this.formData['poste'] !== undefined) {
        this.formData['poste'] = this.annonce.title;
      }
      if (this.formData['email'] !== undefined) {
        this.formData['email'] = this.authService.getEmail();
      }
      if (this.formData['phone'] !== undefined) {
        this.formData['phone'] = this.authService.getPhone();
      }
    }
    this.saveState();
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
    // Validate photos step (disabled for now)
    /*
    if (this.workflow && this.workflowSteps.length > 0) {
      const stepIndex = this.currentStep - 1;
      if (stepIndex >= 0 && stepIndex < this.workflowSteps.length && this.workflowSteps[stepIndex].stepKey === 'photos') {
        if (this.photos.filter(p => p).length < this.maxPhotos) {
          this.photosError = true;
          return false;
        }
      }
    }
    */
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

  prefillTitleField(fieldKey: string) {
    if (!this.formData[fieldKey] && this.annonce.title) {
      this.formData[fieldKey] = this.annonce.title;
    }
  }

  isMultiselectChecked(fieldKey: string, value: string): boolean {
    const current = this.formData[fieldKey];
    if (!current) return false;
    const arr = Array.isArray(current) ? current : current.split(',');
    return arr.includes(value);
  }

  toggleMultiselect(fieldKey: string, value: string) {
    let current = this.formData[fieldKey];
    let arr: string[] = [];
    if (current) {
      arr = Array.isArray(current) ? current : current.split(',');
    }
    const idx = arr.indexOf(value);
    if (idx >= 0) arr.splice(idx, 1);
    else arr.push(value);
    this.formData[fieldKey] = arr;
    this.clearFieldError(fieldKey);
  }

  // Also pre-fill when entering the step
  private prefillDescriptionStep() {
    if (this.formData['description'] !== undefined && !this.formData['description'] && this.annonce.title) {
      this.formData['description'] = this.annonce.title;
    }
  }

  onDateMonthInput(fieldKey: string) {
    let val = this.formData[fieldKey]?.replace(/[^0-9]/g, '') || '';
    if (val.length > 2) {
      val = val.substring(0, 2) + '/' + val.substring(2, 6);
    }
    this.formData[fieldKey] = val;
  }

  onImmatInput(fieldKey: string) {
    let val = (this.formData[fieldKey] || '').toUpperCase().replace(/[^0-9A-Z]/g, '');
    // Format: 123 TU 4567
    // Auto-insert TU after first 1-3 digits
    const digits = val.replace(/[^0-9]/g, '');
    const letters = val.replace(/[^A-Z]/g, '');

    if (letters.includes('TU')) {
      // Already has TU, just format spacing
      const parts = val.split('TU');
      const before = parts[0].replace(/[^0-9]/g, '').substring(0, 3);
      const after = (parts[1] || '').replace(/[^0-9]/g, '').substring(0, 4);
      this.formData[fieldKey] = after ? `${before} TU ${after}` : before.length >= 1 ? `${before} TU ${after}` : before;
    } else {
      // No TU yet - auto-insert after digits if user typed enough
      const nums = val.replace(/[^0-9]/g, '');
      if (nums.length <= 3) {
        this.formData[fieldKey] = nums;
      } else {
        const first = nums.substring(0, 3);
        const rest = nums.substring(3, 7);
        this.formData[fieldKey] = `${first} TU ${rest}`;
      }
    }
  }

  validateImmatriculation() {
    const immat = (this.formData['immatriculation'] || '').trim().toUpperCase();
    this.immatError = '';
    this.immatSuccess = '';

    if (!immat) {
      this.immatError = 'Veuillez saisir un numéro d\'immatriculation.';
      return;
    }

    // Tunisian plate format: 123 TU 1234 or 123TU1234
    const tunisianFormat = /^(\d{1,3})\s*(?:TU|تونس)\s*(\d{1,4})$/i;
    if (!tunisianFormat.test(immat)) {
      this.immatError = 'Format invalide. Le format tunisien est : 123 TU 4567';
      return;
    }

    this.immatLoading = true;
    this.http.get<any>(`${this.apiUrl}/vehicle/lookup/${encodeURIComponent(immat)}`).subscribe({
      next: (data) => {
        this.immatLoading = false;
        if (data && data.brand) {
          this.immatSuccess = `Véhicule identifié : ${data.brand} ${data.model || ''} (${data.year || ''})`;
          // Auto-fill form fields
          if (data.brand) this.formData['brand'] = data.brand.toLowerCase();
          if (data.model) this.formData['model'] = data.model.toLowerCase().replace(/\s+/g, '_');
          if (data.year) this.formData['year'] = data.year.toString();
          if (data.fuel) this.formData['fuel'] = data.fuel.toLowerCase();
          if (data.gearbox) this.formData['gearbox'] = data.gearbox.toLowerCase();
          if (data.fiscalPower) this.formData['fiscalPower'] = data.fiscalPower;
          if (data.dinPower) this.formData['dinPower'] = data.dinPower;
          if (data.firstCirculation) this.formData['firstCirculation'] = data.firstCirculation;
        } else {
          this.immatSuccess = 'Immatriculation validée. Veuillez remplir les informations manuellement.';
        }
      },
      error: () => {
        this.immatLoading = false;
        this.immatSuccess = 'Immatriculation validée. Aucune information trouvée, veuillez remplir manuellement.';
      }
    });
  }

  getDependentOptions(field: StepField): any[] {
    if (field.fieldKey === 'model') {
      const brand = this.formData['brand'];
      if (!brand) return [];
      // Check if it's moto or car based on category
      if (this.annonce.category.toLowerCase().includes('moto') || this.formData['cylindree'] || this.formData['motoType']) {
        return this.motoModels[brand] || [{value:'autre',label:'Autre'}];
      }
      return this.carModels[brand] || [{value:'autre',label:'Autre'}];
    }
    if (field.fieldKey === 'motoType') {
      const vType = this.formData['vehicleType'];
      if (!vType) return [];
      return this.motoTypesByVehicle[vType] || [];
    }
    return field.options;
  }

  motoTypesByVehicle: { [key: string]: { value: string; label: string }[] } = {
    moto: [{value:'sportive',label:'Sportive'},{value:'roadster',label:'Roadster'},{value:'trail',label:'Trail / Enduro'},{value:'custom',label:'Custom / Cruiser'},{value:'touring',label:'Touring / GT'},{value:'cafe_racer',label:'Cafe Racer'},{value:'cross',label:'Cross / Supermotard'},{value:'classique',label:'Classique / Vintage'},{value:'autre',label:'Autre'}],
    scooter: [{value:'urbain',label:'Urbain'},{value:'gt',label:'GT / Maxi-scooter'},{value:'3_roues',label:'3 roues'},{value:'electrique',label:'Electrique'},{value:'autre',label:'Autre'}],
    quad: [{value:'sport',label:'Sport'},{value:'utilitaire',label:'Utilitaire'},{value:'enfant',label:'Enfant'},{value:'autre',label:'Autre'}],
    sidecar: [{value:'classique',label:'Classique'},{value:'moderne',label:'Moderne'},{value:'autre',label:'Autre'}],
    autre: [{value:'autre',label:'Autre'}]
  };

  motoModels: { [brand: string]: { value: string; label: string }[] } = {
    aprilia: [{value:'rs50',label:'RS 50'},{value:'rs125',label:'RS 125'},{value:'rs660',label:'RS 660'},{value:'rsv4',label:'RSV4'},{value:'tuono125',label:'Tuono 125'},{value:'tuono660',label:'Tuono 660'},{value:'tuono_v4',label:'Tuono V4'},{value:'shiver750',label:'Shiver 750'},{value:'dorsoduro',label:'Dorsoduro 900'},{value:'scarabeo',label:'Scarabeo'}],
    benelli: [{value:'tnt125',label:'TNT 125'},{value:'tnt300',label:'TNT 300'},{value:'tnt600',label:'TNT 600'},{value:'leoncino125',label:'Leoncino 125'},{value:'leoncino500',label:'Leoncino 500'},{value:'leoncino800',label:'Leoncino 800'},{value:'trk502',label:'TRK 502'},{value:'trk702',label:'TRK 702'},{value:'imperiale400',label:'Imperiale 400'}],
    bmw: [{value:'s1000rr',label:'S1000RR'},{value:'s1000r',label:'S1000R'},{value:'s1000xr',label:'S1000XR'},{value:'r1250gs',label:'R1250GS'},{value:'r1250r',label:'R1250R'},{value:'r1250rt',label:'R1250RT'},{value:'f750gs',label:'F750GS'},{value:'f850gs',label:'F850GS'},{value:'f900r',label:'F900R'},{value:'f900xr',label:'F900XR'},{value:'g310r',label:'G310R'},{value:'g310gs',label:'G310GS'},{value:'k1600gt',label:'K1600GT'}],
    ducati: [{value:'monster',label:'Monster'},{value:'panigale_v2',label:'Panigale V2'},{value:'panigale_v4',label:'Panigale V4'},{value:'supersport',label:'Supersport 950'},{value:'hypermotard',label:'Hypermotard 950'},{value:'multistrada_v4',label:'Multistrada V4'},{value:'scrambler',label:'Scrambler'},{value:'diavel',label:'Diavel'},{value:'streetfighter_v4',label:'Streetfighter V4'}],
    harley: [{value:'sportster883',label:'Sportster 883'},{value:'sportster1200',label:'Sportster 1200'},{value:'iron883',label:'Iron 883'},{value:'fortyeight',label:'Forty-Eight'},{value:'fat_boy',label:'Fat Boy'},{value:'fat_bob',label:'Fat Bob'},{value:'street_bob',label:'Street Bob'},{value:'low_rider',label:'Low Rider'},{value:'road_king',label:'Road King'},{value:'street_glide',label:'Street Glide'},{value:'road_glide',label:'Road Glide'},{value:'pan_america',label:'Pan America'}],
    honda: [{value:'cbr125',label:'CBR 125'},{value:'cbr500',label:'CBR 500'},{value:'cbr650r',label:'CBR 650R'},{value:'cbr1000rr',label:'CBR 1000RR'},{value:'cb500f',label:'CB 500F'},{value:'cb650r',label:'CB 650R'},{value:'cb1000r',label:'CB 1000R'},{value:'africa_twin',label:'Africa Twin'},{value:'nc750',label:'NC 750'},{value:'forza125',label:'Forza 125'},{value:'forza350',label:'Forza 350'},{value:'pcx125',label:'PCX 125'},{value:'goldwing',label:'Goldwing'}],
    kawasaki: [{value:'z125',label:'Z125'},{value:'z400',label:'Z400'},{value:'z650',label:'Z650'},{value:'z900',label:'Z900'},{value:'z1000',label:'Z1000'},{value:'ninja400',label:'Ninja 400'},{value:'ninja650',label:'Ninja 650'},{value:'zx6r',label:'ZX-6R'},{value:'zx10r',label:'ZX-10R'},{value:'versys650',label:'Versys 650'},{value:'versys1000',label:'Versys 1000'},{value:'vulcan_s',label:'Vulcan S'}],
    ktm: [{value:'duke125',label:'Duke 125'},{value:'duke200',label:'Duke 200'},{value:'duke390',label:'Duke 390'},{value:'duke690',label:'Duke 690'},{value:'duke790',label:'Duke 790'},{value:'duke890',label:'Duke 890'},{value:'duke1290',label:'Super Duke 1290'},{value:'rc390',label:'RC 390'},{value:'adv390',label:'Adventure 390'},{value:'adv890',label:'Adventure 890'},{value:'adv1290',label:'Adventure 1290'}],
    malaguti: [{value:'xsm50',label:'XSM 50'},{value:'xtm50',label:'XTM 50'},{value:'drakon125',label:'Drakon 125'},{value:'monte_pro',label:'Monte Pro 125'},{value:'madison',label:'Madison'},{value:'phantom',label:'Phantom'}],
    peugeot: [{value:'kisbee50',label:'Kisbee 50'},{value:'speedfight',label:'Speedfight'},{value:'django',label:'Django'},{value:'tweet',label:'Tweet'},{value:'citystar',label:'Citystar'},{value:'metropolis',label:'Metropolis 400'}],
    suzuki: [{value:'gsxr600',label:'GSX-R 600'},{value:'gsxr750',label:'GSX-R 750'},{value:'gsxr1000',label:'GSX-R 1000'},{value:'gsxs750',label:'GSX-S 750'},{value:'gsxs1000',label:'GSX-S 1000'},{value:'sv650',label:'SV 650'},{value:'vstrom650',label:'V-Strom 650'},{value:'vstrom1000',label:'V-Strom 1000'},{value:'burgman',label:'Burgman'}],
    triumph: [{value:'street_triple',label:'Street Triple'},{value:'speed_triple',label:'Speed Triple'},{value:'tiger800',label:'Tiger 800'},{value:'tiger900',label:'Tiger 900'},{value:'tiger1200',label:'Tiger 1200'},{value:'bonneville',label:'Bonneville'},{value:'scrambler',label:'Scrambler'},{value:'thruxton',label:'Thruxton'},{value:'rocket3',label:'Rocket 3'}],
    yamaha: [{value:'mt07',label:'MT-07'},{value:'mt09',label:'MT-09'},{value:'mt10',label:'MT-10'},{value:'mt125',label:'MT-125'},{value:'r125',label:'R125'},{value:'r3',label:'R3'},{value:'r6',label:'R6'},{value:'r1',label:'R1'},{value:'xsr700',label:'XSR 700'},{value:'xsr900',label:'XSR 900'},{value:'tracer700',label:'Tracer 700'},{value:'tracer900',label:'Tracer 900'},{value:'tenere700',label:'T\u00e9n\u00e9r\u00e9 700'},{value:'xmax',label:'XMAX'},{value:'tmax',label:'TMAX'}],
    autre: [{value:'autre',label:'Autre'}]
  };

  generateDescription(fieldKey: string) {
    this.aiGenerating = true;
    const context: any = {};
    for (const key of Object.keys(this.formData)) {
      if (this.formData[key] && key !== fieldKey) context[key] = this.formData[key];
    }
    context.title = this.annonce.title;
    context.category = this.annonce.category;

    this.http.post<any>(`${this.apiUrl}/ai/generate-description`, context).subscribe({
      next: (res) => {
        this.aiGenerating = false;
        if (res && res.description) this.formData[fieldKey] = res.description;
      },
      error: () => {
        this.aiGenerating = false;
        this.formData[fieldKey] = this.buildFallbackDescription();
      }
    });
  }

  private buildFallbackDescription(): string {
    const brand = this.formData['brand'] || '';
    const model = this.formData['model'] || '';
    const year = this.formData['year'] || '';
    const fuel = this.formData['fuel'] || '';
    const mileage = this.formData['mileage'] || '';
    const gearbox = this.formData['gearbox'] || '';
    const dinPower = this.formData['dinPower'] || '';
    const fiscalPower = this.formData['fiscalPower'] || '';
    const vehicleType = this.formData['vehicleType'] || '';
    const seats = this.formData['seats'] || '';
    const doors = this.formData['doors'] || '';
    const color = this.formData['color'] || '';
    const technicalControl = this.formData['technicalControl'] || '';
    const upholstery = this.formData['upholstery'];
    const equipment = this.formData['equipment'];
    const history = this.formData['history'];
    const cylindree = this.formData['cylindree'] || '';
    const motoType = this.formData['motoType'] || '';
    const license = this.formData['license'] || '';

    const isMoto = this.annonce.category.toLowerCase().includes('moto') || cylindree || motoType;

    let desc = '';
    if (isMoto) {
      desc = `Je vends ma ${brand} ${model}`;
      if (year) desc += ` de ${year}`;
      if (motoType) desc += `, une ${motoType} agile et puissante`;
      if (mileage) desc += ` avec seulement ${mileage} km au compteur`;
      desc += '.\n';
      if (brand) desc += `- Marque : ${brand}\n`;
      if (model) desc += `- Mod\u00e8le : ${model}\n`;
      if (year) desc += `- Ann\u00e9e : ${year}\n`;
      if (mileage) desc += `- Kilom\u00e9trage : ${mileage} km\n`;
      if (cylindree) desc += `- Cylindr\u00e9e : ${cylindree}\n`;
      if (motoType) desc += `- Type : ${motoType}\n`;
      if (color) desc += `- Couleur : ${color}\n`;
      if (license) desc += `- Permis : ${license}\n`;
      if (equipment && Array.isArray(equipment) && equipment.length) desc += `- \u00c9quipements : ${equipment.join(', ')}\n`;
    } else {
      desc = `Je vends mon ${brand} ${model}`;
      if (year) desc += ` de ${year}`;
      if (vehicleType) desc += `, un ${vehicleType} spacieux et confortable`;
      if (mileage) desc += ` avec seulement ${mileage} km au compteur`;
      desc += '.\n';
      if (brand) desc += `- Marque : ${brand}\n`;
      if (model) desc += `- Mod\u00e8le : ${model}\n`;
      if (year) desc += `- Ann\u00e9e : ${year}\n`;
      if (mileage) desc += `- Kilom\u00e9trage : ${mileage} km\n`;
      if (dinPower) desc += `- Motorisation : ${dinPower} Ch\n`;
      if (fuel) desc += `- Carburant : ${fuel}\n`;
      if (gearbox) desc += `- Bo\u00eete de vitesses : ${gearbox}\n`;
      if (color) desc += `- Couleur : ${color}\n`;
      if (vehicleType) desc += `- Type de v\u00e9hicule : ${vehicleType}\n`;
      if (seats) desc += `- Nombre de si\u00e8ges : ${seats}\n`;
      if (doors) desc += `- Nombre de portes : ${doors}\n`;
      if (fiscalPower) desc += `- Puissance fiscale : ${fiscalPower} CV\n`;
      if (technicalControl) desc += `- Contr\u00f4le technique : Valide jusqu'en ${technicalControl}\n`;
      if (upholstery && Array.isArray(upholstery) && upholstery.length) desc += `- Sellerie : ${upholstery.join(', ')}\n`;
      if (equipment && Array.isArray(equipment) && equipment.length) desc += `- \u00c9quipements : ${equipment.join(', ')}\n`;
      if (history && Array.isArray(history) && history.length) desc += `- Historique : ${history.join(', ')}\n`;
    }

    desc += `\nN'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour convenir d'un essai !`;
    return desc;
  }

  carModels: { [brand: string]: { value: string; label: string }[] } = {
    peugeot: [{value:'108',label:'108'},{value:'208',label:'208'},{value:'308',label:'308'},{value:'408',label:'408'},{value:'508',label:'508'},{value:'2008',label:'2008'},{value:'3008',label:'3008'},{value:'5008',label:'5008'},{value:'rifter',label:'Rifter'},{value:'partner',label:'Partner'}],
    renault: [{value:'clio',label:'Clio'},{value:'megane',label:'Megane'},{value:'captur',label:'Captur'},{value:'kadjar',label:'Kadjar'},{value:'scenic',label:'Scenic'},{value:'talisman',label:'Talisman'},{value:'kangoo',label:'Kangoo'},{value:'twingo',label:'Twingo'},{value:'zoe',label:'Zoe'},{value:'arkana',label:'Arkana'},{value:'austral',label:'Austral'}],
    citroen: [{value:'c1',label:'C1'},{value:'c3',label:'C3'},{value:'c4',label:'C4'},{value:'c5_aircross',label:'C5 Aircross'},{value:'berlingo',label:'Berlingo'},{value:'ds3',label:'DS3'},{value:'ds4',label:'DS4'},{value:'ds7',label:'DS7'}],
    volkswagen: [{value:'polo',label:'Polo'},{value:'golf',label:'Golf'},{value:'tiguan',label:'Tiguan'},{value:'touran',label:'Touran'},{value:'passat',label:'Passat'},{value:'t_roc',label:'T-Roc'},{value:'t_cross',label:'T-Cross'},{value:'arteon',label:'Arteon'},{value:'id3',label:'ID.3'},{value:'id4',label:'ID.4'}],
    bmw: [{value:'serie1',label:'Série 1'},{value:'serie2',label:'Série 2'},{value:'serie3',label:'Série 3'},{value:'serie4',label:'Série 4'},{value:'serie5',label:'Série 5'},{value:'x1',label:'X1'},{value:'x3',label:'X3'},{value:'x5',label:'X5'},{value:'x6',label:'X6'},{value:'ix',label:'iX'}],
    mercedes: [{value:'classe_a',label:'Classe A'},{value:'classe_b',label:'Classe B'},{value:'classe_c',label:'Classe C'},{value:'classe_e',label:'Classe E'},{value:'classe_s',label:'Classe S'},{value:'gla',label:'GLA'},{value:'glb',label:'GLB'},{value:'glc',label:'GLC'},{value:'gle',label:'GLE'},{value:'eqc',label:'EQC'}],
    audi: [{value:'a1',label:'A1'},{value:'a3',label:'A3'},{value:'a4',label:'A4'},{value:'a5',label:'A5'},{value:'a6',label:'A6'},{value:'q2',label:'Q2'},{value:'q3',label:'Q3'},{value:'q5',label:'Q5'},{value:'q7',label:'Q7'},{value:'e_tron',label:'e-tron'}],
    toyota: [{value:'yaris',label:'Yaris'},{value:'corolla',label:'Corolla'},{value:'c_hr',label:'C-HR'},{value:'rav4',label:'RAV4'},{value:'land_cruiser',label:'Land Cruiser'},{value:'hilux',label:'Hilux'},{value:'camry',label:'Camry'},{value:'aygo',label:'Aygo'}],
    hyundai: [{value:'i10',label:'i10'},{value:'i20',label:'i20'},{value:'i30',label:'i30'},{value:'tucson',label:'Tucson'},{value:'kona',label:'Kona'},{value:'santa_fe',label:'Santa Fe'},{value:'ioniq',label:'Ioniq'}],
    kia: [{value:'picanto',label:'Picanto'},{value:'rio',label:'Rio'},{value:'ceed',label:'Ceed'},{value:'sportage',label:'Sportage'},{value:'niro',label:'Niro'},{value:'sorento',label:'Sorento'},{value:'ev6',label:'EV6'}],
    fiat: [{value:'500',label:'500'},{value:'panda',label:'Panda'},{value:'tipo',label:'Tipo'},{value:'500x',label:'500X'},{value:'doblo',label:'Doblo'}],
    nissan: [{value:'micra',label:'Micra'},{value:'juke',label:'Juke'},{value:'qashqai',label:'Qashqai'},{value:'x_trail',label:'X-Trail'},{value:'leaf',label:'Leaf'},{value:'navara',label:'Navara'}],
    ford: [{value:'fiesta',label:'Fiesta'},{value:'focus',label:'Focus'},{value:'puma',label:'Puma'},{value:'kuga',label:'Kuga'},{value:'mustang',label:'Mustang'},{value:'ranger',label:'Ranger'}],
    opel: [{value:'corsa',label:'Corsa'},{value:'astra',label:'Astra'},{value:'mokka',label:'Mokka'},{value:'crossland',label:'Crossland'},{value:'grandland',label:'Grandland'}],
    dacia: [{value:'sandero',label:'Sandero'},{value:'duster',label:'Duster'},{value:'logan',label:'Logan'},{value:'jogger',label:'Jogger'},{value:'spring',label:'Spring'}],
    seat: [{value:'ibiza',label:'Ibiza'},{value:'leon',label:'Leon'},{value:'arona',label:'Arona'},{value:'ateca',label:'Ateca'},{value:'tarraco',label:'Tarraco'}],
    skoda: [{value:'fabia',label:'Fabia'},{value:'octavia',label:'Octavia'},{value:'kamiq',label:'Kamiq'},{value:'karoq',label:'Karoq'},{value:'kodiaq',label:'Kodiaq'},{value:'superb',label:'Superb'}],
    suzuki: [{value:'swift',label:'Swift'},{value:'vitara',label:'Vitara'},{value:'s_cross',label:'S-Cross'},{value:'jimny',label:'Jimny'},{value:'ignis',label:'Ignis'}],
    autre: [{value:'autre',label:'Autre modèle'}]
  };

  onAddressInput(fieldKey: string) {
    const value = this.formData[fieldKey];
    if (value && value.length >= 3) {
      this.addressSubject.next(value);
    } else {
      this.addressResults = [];
    }
  }

  loadPriceEstimate() {
    if (!this.selectedCategoryId) return;
    const payload = {
      categoryId: this.selectedCategoryId,
      brand: this.formData['brand'] || '',
      model: this.formData['model'] || ''
    };
    this.http.post<any>(`${this.apiUrl}/annonces/price-estimate`, payload).subscribe({
      next: (data) => { this.priceEstimate = data; this.updatePriceGauge(); },
      error: () => { this.priceEstimate = null; }
    });
  }

  updatePriceGauge() {
    if (!this.priceEstimate) { this.priceGaugePosition = 50; this.priceRanges = []; return; }
    const min = this.priceEstimate.minPrice;
    const max = this.priceEstimate.maxPrice;

    // Compute 5 equal ranges
    const step = (max - min) / 5;
    this.priceRanges = [];
    for (let i = 0; i < 5; i++) {
      this.priceRanges.push({
        min: Math.round(min + step * i),
        max: Math.round(min + step * (i + 1))
      });
    }

    // Position indicator
    if (!this.formData['price']) { this.priceGaugePosition = 50; return; }
    const price = parseFloat(this.formData['price']);
    if (max <= min) { this.priceGaugePosition = 50; return; }
    const pct = ((price - min) / (max - min)) * 100;
    this.priceGaugePosition = Math.max(0, Math.min(100, pct));
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
    // Pre-fill title field in description step
    this.prefillDescriptionStep();

    // Load price estimate when entering price step
    if (this.workflow && this.workflowSteps.length > 0) {
      const stepIndex = this.currentStep - 1;
      if (stepIndex >= 0 && stepIndex < this.workflowSteps.length && this.workflowSteps[stepIndex].stepKey === 'price') {
        this.loadPriceEstimate();
      }
    }

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

  onQuitClick(event: Event) {
    event.preventDefault();
    if (this.currentStep > 1) {
      this.showQuitModal = true;
    } else {
      this.router.navigate(['/']);
    }
  }

  quitWithoutSaving() {
    this.showQuitModal = false;
    sessionStorage.removeItem('deposer_step');
    sessionStorage.removeItem('deposer_data');
    this.router.navigate(['/']);
  }

  saveDraft() {
    this.savingDraft = true;
    const payload: any = {
      title: this.annonce.title || this.formData['description'] || 'Brouillon',
      categoryId: this.selectedCategoryId || 0,
      adType: this.annonce.adType || 'Brouillon',
      description: this.formData['annonce_description'] || this.annonce.description || '',
      price: this.formData['price'] || this.annonce.price || 0,
      condition: this.annonce.condition || '',
      location: this.formData['address'] || this.annonce.location || '',
      phone: this.formData['phone'] || this.contactForm.phone || '',
      email: this.formData['email'] || this.contactForm.email || '',
      hidePhone: false,
      status: 'draft',
      currentStep: this.currentStep,
      extraData: this.workflow ? this.formData : { ...this.emploiForm }
    };

    this.http.post<any>(`${this.apiUrl}/annonces/draft`, payload).subscribe({
      next: () => {
        this.savingDraft = false;
        this.showQuitModal = false;
        sessionStorage.removeItem('deposer_step');
        sessionStorage.removeItem('deposer_data');
        this.router.navigate(['/']);
      },
      error: () => {
        this.savingDraft = false;
        alert('Erreur lors de l\'enregistrement du brouillon.');
      }
    });
  }

  submit() {
    this.submitting = true;
    const payload: any = {
      title: this.annonce.title,
      categoryId: this.selectedCategoryId,
      adType: this.annonce.adType,
      description: this.annonce.description || this.formData['annonce_description'] || this.formData['experienceDesc'] || '',
      price: this.annonce.price || this.formData['price'] || this.formData['salary'] || 0,
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
