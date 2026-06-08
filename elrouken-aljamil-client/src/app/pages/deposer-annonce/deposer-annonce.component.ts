import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { NgIf, NgFor, NgStyle } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-deposer-annonce',
  standalone: true,
  imports: [NgIf, NgFor, NgStyle, FormsModule, RouterLink],
  templateUrl: './deposer-annonce.component.html',
  styleUrl: './deposer-annonce.component.scss'
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
    workType: 'temps_plein'
  };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.searchSubject.pipe(debounceTime(300)).subscribe(query => {
      if (query.length >= 2) {
        if (this.searchCache[query]) {
          this.suggestedCategories = this.searchCache[query];
        } else {
          this.http.get<any[]>(`${this.apiUrl}/annonces/suggest-categories?query=${encodeURIComponent(query)}`)
            .subscribe(data => {
              this.searchCache[query] = data;
              this.suggestedCategories = data;
            });
        }
      } else {
        this.suggestedCategories = [];
      }
    });
  }

  onTitleChange() {
    this.searchSubject.next(this.annonce.title);
  }

  selectSuggestedCategory(cat: any) {
    this.selectedCategoryId = cat.categoryId;
    this.annonce.category = cat.menuName + ' > ' + cat.categoryName;
    this.checkAdType(cat.categoryName, cat.menuName);
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
    this.checkAdType(cat.name, menu?.name);
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

  get isVehiculeCategory(): boolean {
    return this.annonce.category.toLowerCase().includes('véhicules') || this.annonce.category.toLowerCase().includes('vehicules');
  }

  draggedSlot: number | null = null;

  onDragStart(event: DragEvent, slot: number) {
    this.draggedSlot = slot;
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'move';
    }
  }

  onDrop(event: DragEvent, targetSlot: number) {
    event.preventDefault();
    if (this.draggedSlot !== null && this.draggedSlot !== targetSlot) {
      const temp = this.photos[this.draggedSlot];
      this.photos[this.draggedSlot] = this.photos[targetSlot] || '';
      this.photos[targetSlot] = temp;
      // Nettoyer les slots vides à la fin
      while (this.photos.length > 0 && this.photos[this.photos.length - 1] === '') {
        this.photos.pop();
      }
    }
    this.draggedSlot = null;
  }

  nextStep() {
    if (this.currentStep === 1 && (this.isEmploiCategory() || this.isVehiculeCategory)) {
      this.currentStep = 3;
    } else if (this.currentStep < 4) {
      this.currentStep++;
    }
  }

  prevStep() {
    if (this.currentStep === 3 && (this.isEmploiCategory() || this.isVehiculeCategory)) {
      this.currentStep = 1;
    } else if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  isEmploiCategory(): boolean {
    return this.annonce.category.toLowerCase().includes('emploi');
  }

  submit() {
    alert('Annonce publiée avec succès !');
  }
}
