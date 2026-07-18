import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Subject, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { PlantService } from '../../core/plant.service';
import { GardenService } from '../../core/garden.service';
import { PlantSummary } from '../../core/models';

interface LoadRequest {
  query: string;
  page: number;
}

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

  /** Bound to the search input; may differ from what is currently displayed. */
  query = '';
  /** The query that produced the results on screen; drives pagination. */
  activeQuery = '';
  results: PlantSummary[] = [];
  page = 1;
  totalPages?: number;
  loading = false;
  hasSearched = false;
  /** True once a page returns fewer than a full page of results. */
  lastPageReached = false;
  /** Page-level failure that replaces the results (e.g. API/token down). */
  loadError?: string;
  /** Transient, non-destructive error (e.g. a single add-to-garden failure). */
  actionError?: string;

  readonly pageSize = this.plants.pageSize;

  private readonly requests = new Subject<LoadRequest>();
  private readonly addedIds = new Set<number>();
  private readonly addingIds = new Set<number>();

  constructor() {
    // switchMap cancels any in-flight request when a newer one arrives, so a
    // slow response for an old query/page can never overwrite fresher results.
    this.requests
      .pipe(
        switchMap((req) =>
          this.plants.search(req.query, req.page).pipe(
            map((result) => ({ req, result, failed: false })),
            catchError(() => of({ req, result: null, failed: true }))
          )
        )
      )
      .subscribe(({ req, result, failed }) => {
        this.loading = false;
        if (failed || !result) {
          this.loadError =
            'Could not load plants. Is the API running and the Trefle token set?';
          return;
        }
        this.results = result.plants;
        this.page = result.page;
        this.activeQuery = req.query;
        this.totalPages = result.totalPages;
        this.lastPageReached = result.plants.length < this.pageSize;
        this.hasSearched = true;
      });
  }

  ngOnInit(): void {
    this.dispatch('', 1);
  }

  search(): void {
    this.dispatch(this.query.trim(), 1);
  }

  nextPage(): void {
    if (this.canGoNext) {
      this.dispatch(this.activeQuery, this.page + 1);
    }
  }

  prevPage(): void {
    if (this.canGoPrev) {
      this.dispatch(this.activeQuery, this.page - 1);
    }
  }

  get canGoNext(): boolean {
    if (this.loading || this.results.length === 0) {
      return false;
    }
    return this.totalPages != null ? this.page < this.totalPages : !this.lastPageReached;
  }

  get canGoPrev(): boolean {
    return !this.loading && this.page > 1;
  }

  private dispatch(query: string, page: number): void {
    this.loading = true;
    this.loadError = undefined;
    this.actionError = undefined;
    this.requests.next({ query, page });
  }

  addToGarden(plant: PlantSummary, event: Event): void {
    event.stopPropagation();
    if (this.isAdded(plant) || this.isAdding(plant)) {
      return;
    }
    this.actionError = undefined;
    this.addingIds.add(plant.trefleId);
    this.garden.add({ trefleId: plant.trefleId }).subscribe({
      next: () => {
        this.addingIds.delete(plant.trefleId);
        this.addedIds.add(plant.trefleId);
      },
      error: () => {
        this.addingIds.delete(plant.trefleId);
        this.actionError = `Could not add ${plant.displayName} to your garden.`;
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
