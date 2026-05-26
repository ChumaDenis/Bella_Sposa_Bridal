import { Routes } from '@angular/router';
import { Home } from './home/home';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Home },
  {
    path: 'catalog',
    loadComponent: () => import('./catalog/catalog').then(m => m.CatalogComponent)
  },
  {
    path: 'catalog/:slug',
    loadComponent: () => import('./dress-detail/dress-detail').then(m => m.DressDetailComponent)
  },
  {
    path: 'appointment',
    loadComponent: () => import('./appointment/appointment').then(m => m.AppointmentComponent)
  },
  {
    path: 'collections',
    loadComponent: () => import('./collections/collections').then(m => m.CollectionsComponent)
  },
  {
    path: 'collections/:slug',
    loadComponent: () => import('./collection-detail/collection-detail').then(m => m.CollectionDetailComponent)
  },
  {
    path: 'contact',
    loadComponent: () => import('./contact/contact').then(m => m.ContactComponent)
  },
  {
    path: 'admin/login',
    loadComponent: () => import('./admin/login/login').then(m => m.LoginComponent)
  },
  {
    path: 'admin',
    loadComponent: () => import('./admin/admin').then(m => m.AdminComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '' }
];
