import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PlantService } from '../../core/plant.service';
import { GardenService } from '../../core/garden.service';
import { PlantSummary } from '../../core/models';

@Component({
  selector: 'app-plant-list',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './plant-list.component.html',
  styleUrl: './plant-list.component.scss'
})
export class PlantListComponent implements OnInit {
  private readonly plants = inject(PlantService);
  private readonly garden = inject(GardenService);

  query = '';
  results: PlantSummary[] = [];
  page = 1;
  totalPages?: number;
  loading = false;
  error?: string;

  private readonly addedIds = new Set<number>();
  private readonly addingIds = new Set<number>();

  ngOnInit(): void {
    this.load(1);
  }

  search(): void {
    this.load(1);
  }

  load(page: number): void {
    this.loading = true;
    this.error = undefined;
    this.plants.search(this.query, page).subscribe({
      next: (result) => {
        this.results = result.plants;
        this.page = result.page;
        this.totalPages = result.totalPages;
        this.loading = false;
      },
      error: () => {
        this.error = 'Could not load plants. Is the API running and the Trefle token set?';
        this.loading = false;
      }
    });
  }

  nextPage(): void {
    if (!this.totalPages || this.page < this.totalPages) {
      this.load(this.page + 1);
    }
  }

  prevPage(): void {
    if (this.page > 1) {
      this.load(this.page - 1);
    }
  }

  addToGarden(plant: PlantSummary, event: Event): void {
    event.stopPropagation();
    if (this.isAdded(plant) || this.isAdding(plant)) {
      return;
    }
    this.addingIds.add(plant.trefleId);
    this.garden.add({ trefleId: plant.trefleId }).subscribe({
      next: () => {
        this.addingIds.delete(plant.trefleId);
        this.addedIds.add(plant.trefleId);
      },
      error: () => {
        this.addingIds.delete(plant.trefleId);
        this.error = `Could not add ${plant.displayName} to your garden.`;
      }
    });
  }

  isAdded(plant: PlantSummary): boolean {
    return this.addedIds.has(plant.trefleId);
  }

  isAdding(plant: PlantSummary): boolean {
    return this.addingIds.has(plant.trefleId);
  }
}
