import { Component } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-deposer-annonce',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule, RouterLink],
  templateUrl: './deposer-annonce.component.html',
  styleUrl: './deposer-annonce.component.scss'
})
export class DeposerAnnonceComponent {
  currentStep = 1;

  annonce = {
    category: '',
    title: '',
    description: '',
    price: null as number | null,
    location: '',
    condition: ''
  };

  categoriesList = [
    { name: 'Immobilier', icon: 'fa-solid fa-house' },
    { name: 'Véhicules', icon: 'fa-solid fa-car' },
    { name: 'Électronique', icon: 'fa-solid fa-mobile-screen' },
    { name: 'Mode', icon: 'fa-solid fa-shirt' },
    { name: 'Maison & Jardin', icon: 'fa-solid fa-couch' },
    { name: 'Famille', icon: 'fa-solid fa-baby' },
    { name: 'Loisirs', icon: 'fa-solid fa-futbol' },
    { name: 'Emploi', icon: 'fa-solid fa-briefcase' },
    { name: 'Vacances', icon: 'fa-solid fa-umbrella-beach' },
    { name: 'Autres', icon: 'fa-solid fa-ellipsis' },
  ];

  selectCategory(cat: string) {
    this.annonce.category = cat;
  }

  nextStep() {
    if (this.currentStep < 4) this.currentStep++;
  }

  prevStep() {
    if (this.currentStep > 1) this.currentStep--;
  }

  submit() {
    alert('Annonce publiée avec succès !');
  }
}
