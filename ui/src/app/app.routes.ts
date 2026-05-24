import { Routes } from '@angular/router';
import { Home } from './home/home';

export const routes: Routes = [
  { path: '', component: Home },
  {
    path: 'catalog',
    loadComponent: () => import('./catalog/catalog').then(m => m.CatalogComponent)
  },
  {
    path: 'catalog/:id',
    loadComponent: () => import('./dress-detail/dress-detail').then(m => m.DressDetailComponent)
  },
  {
    path: 'appointment',
    loadComponent: () => import('./appointment/appointment').then(m => m.AppointmentComponent)
  },
  { path: '**', redirectTo: '' }
];
