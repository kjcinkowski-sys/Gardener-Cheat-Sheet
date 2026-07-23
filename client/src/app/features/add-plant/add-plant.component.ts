import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { GardenService } from '../../core/garden.service';
import { AddCustomPlantRequest } from '../../core/models';
import { PhotoPickerComponent } from '../../shared/photo-picker/photo-picker.component';

@Component({
  selector: 'app-add-plant',
  standalone: true,
  imports: [FormsModule, RouterLink, PhotoPickerComponent],
  templateUrl: './add-plant.component.html',
  styleUrl: './add-plant.component.scss'
})
export class AddPlantComponent {
  private readonly garden = inject(GardenService);
  private readonly router = inject(Router);

  displayName = '';
  scientificName = '';
  isIndoor = false;
  lightLevel: number | null = null;
  wateringIntervalDays: number | null = null;
  notes = '';
  imageUrl: string | null = null;

  saving = false;
  error?: string;

  get canSave(): boolean {
    return this.displayName.trim().length > 0 && !this.saving;
  }

  onPhotoChange(url: string | null): void {
    this.imageUrl = url;
  }

  save(): void {
    if (!this.canSave) {
      return;
    }

    const request: AddCustomPlantRequest = {
      displayName: this.displayName.trim(),
      isIndoor: this.isIndoor
    };
    if (this.scientificName.trim()) {
      request.scientificName = this.scientificName.trim();
    }
    if (this.lightLevel != null) {
      request.lightLevel = this.lightLevel;
    }
    if (this.wateringIntervalDays && this.wateringIntervalDays > 0) {
      request.wateringIntervalDays = this.wateringIntervalDays;
    }
    if (this.notes.trim()) {
      request.notes = this.notes.trim();
    }
    if (this.imageUrl) {
      request.imageUrl = this.imageUrl;
    }

    this.saving = true;
    this.error = undefined;
    this.garden.addCustom(request).subscribe({
      next: () => this.router.navigate(['/garden']),
      error: () => {
        this.saving = false;
        this.error = 'Could not save your plant. Is the API running?';
      }
    });
  }
}
