import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PlantService } from '../../core/plant.service';
import { GardenService } from '../../core/garden.service';
import { PlantDetail } from '../../core/models';

@Component({
  selector: 'app-plant-detail',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './plant-detail.component.html',
  styleUrl: './plant-detail.component.scss'
})
export class PlantDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly plants = inject(PlantService);
  private readonly garden = inject(GardenService);

  plant?: PlantDetail;
  loading = false;
  error?: string;
  added = false;
  adding = false;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.error = 'Invalid plant id.';
      return;
    }
    this.loading = true;
    this.plants.getById(id).subscribe({
      next: (plant) => {
        this.plant = plant;
        this.loading = false;
      },
      error: () => {
        this.error = 'Could not load this plant.';
        this.loading = false;
      }
    });
  }

  addToGarden(): void {
    if (!this.plant || this.added || this.adding) {
      return;
    }
    this.adding = true;
    this.garden.add({ trefleId: this.plant.trefleId }).subscribe({
      next: () => {
        this.adding = false;
        this.added = true;
      },
      error: () => {
        this.adding = false;
        this.error = 'Could not add this plant to your garden.';
      }
    });
  }
}
