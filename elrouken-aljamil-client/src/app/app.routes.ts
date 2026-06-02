import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { DeposerAnnonceComponent } from './pages/deposer-annonce/deposer-annonce.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'deposer', component: DeposerAnnonceComponent },
];
