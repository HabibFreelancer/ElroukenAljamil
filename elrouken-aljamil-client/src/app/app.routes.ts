import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { DeposerAnnonceComponent } from './pages/deposer-annonce/deposer-annonce.component';
import { AuthComponent } from './pages/auth/auth.component';
import { AdminComponent } from './pages/admin/admin.component';
import { MenuManagementComponent } from './pages/admin/menu-management/menu-management.component';
import { CategoryManagementComponent } from './pages/admin/category-management/category-management.component';
import { AdtypeManagementComponent } from './pages/admin/adtype-management/adtype-management.component';
import { WorkflowManagementComponent } from './pages/admin/workflow-management/workflow-management.component';
import { FeedbackManagementComponent } from './pages/admin/feedback-management/feedback-management.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'deposer', component: DeposerAnnonceComponent },
  { path: 'auth', component: AuthComponent },
  {
    path: 'admin',
    component: AdminComponent,
    children: [
      { path: '', redirectTo: 'menus', pathMatch: 'full' },
      { path: 'menus', component: MenuManagementComponent },
      { path: 'categories', component: CategoryManagementComponent },
      { path: 'adtypes', component: AdtypeManagementComponent },
      { path: 'workflows', component: WorkflowManagementComponent },
      { path: 'feedbacks', component: FeedbackManagementComponent },
    ]
  },
];
