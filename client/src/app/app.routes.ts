import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'plants' },
  {
    path: 'plants',
    loadComponent: () =>
      import('./features/plant-list/plant-list.component').then((m) => m.PlantListComponent)
  },
  {
    path: 'plants/:id',
    loadComponent: () =>
      import('./features/plant-detail/plant-detail.component').then((m) => m.PlantDetailComponent)
  },
  {
    path: 'garden',
    loadComponent: () =>
      import('./features/garden/garden.component').then((m) => m.GardenComponent)
  },
  { path: '**', redirectTo: 'plants' }
];
