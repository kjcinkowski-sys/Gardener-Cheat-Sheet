import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { GardenService } from '../../core/garden.service';
import { GardenEntry, UpdateGardenEntryRequest } from '../../core/models';
import { PhotoPickerComponent } from '../../shared/photo-picker/photo-picker.component';

interface EditModel {
  nickname: string;
  notes: string;
  wateringOverrideDays: number | null;
  isIndoor: boolean;
  imageUrl: string | null;
  photoTouched: boolean;
  // Custom-plant identity fields (only shown/sent for custom plants).
  isCustom: boolean;
  displayName: string;
  scientificName: string;
  lightLevel: number | null;
}

@Component({
  selector: 'app-garden',
  standalone: true,
  imports: [FormsModule, RouterLink, PhotoPickerComponent],
  templateUrl: './garden.component.html',
  styleUrl: './garden.component.scss'
})
export class GardenComponent implements OnInit {
  private readonly garden = inject(GardenService);

  entries: GardenEntry[] = [];
  loading = false;
  error?: string;

  editingId: number | null = null;
  edit: EditModel = this.emptyEdit();

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = undefined;
    this.garden.getGarden().subscribe({
      next: (entries) => {
        this.entries = entries;
        this.loading = false;
      },
      error: () => {
        this.error = 'Could not load your garden. Is the API running?';
        this.loading = false;
      }
    });
  }

  get dueCount(): number {
    return this.entries.filter((e) => e.isDue).length;
  }

  waterToday(entry: GardenEntry): void {
    const today = new Date().toISOString().substring(0, 10);
    this.apply(entry.id, { lastWatered: today });
  }

  remove(entry: GardenEntry): void {
    this.garden.remove(entry.id).subscribe({
      next: () => (this.entries = this.entries.filter((e) => e.id !== entry.id)),
      error: () => (this.error = `Could not remove ${entry.displayName}.`)
    });
  }

  startEdit(entry: GardenEntry): void {
    this.editingId = entry.id;
    this.edit = {
      nickname: entry.nickname ?? '',
      notes: entry.notes ?? '',
      wateringOverrideDays:
        entry.watering.source === 'UserOverride' ? entry.watering.daysBetweenWatering : null,
      isIndoor: entry.isIndoor,
      imageUrl: entry.imageUrl ?? null,
      photoTouched: false,
      isCustom: entry.isCustom,
      displayName: entry.displayName,
      scientificName: entry.scientificName ?? '',
      lightLevel: this.lightLevelFromLabel(entry.lightRequirement)
    };
  }

  cancelEdit(): void {
    this.editingId = null;
  }

  onEditPhotoChange(url: string | null): void {
    this.edit.imageUrl = url;
    this.edit.photoTouched = true;
  }

  saveEdit(entry: GardenEntry): void {
    const request: UpdateGardenEntryRequest = {
      notes: this.edit.notes,
      isIndoorOverride: this.edit.isIndoor
    };

    if (this.edit.wateringOverrideDays && this.edit.wateringOverrideDays > 0) {
      request.wateringOverrideDays = this.edit.wateringOverrideDays;
    } else {
      request.clearWateringOverride = true;
    }

    if (this.edit.photoTouched) {
      if (this.edit.imageUrl) {
        request.imageUrl = this.edit.imageUrl;
      } else {
        request.clearImage = true;
      }
    }

    if (this.edit.isCustom) {
      // Custom plants edit the plant's own name/scientific/light instead of a nickname.
      request.displayName = this.edit.displayName;
      request.scientificName = this.edit.scientificName;
      if (this.edit.lightLevel != null) {
        request.lightLevel = this.edit.lightLevel;
      } else {
        request.clearLightLevel = true;
      }
    } else {
      request.nickname = this.edit.nickname;
    }

    this.apply(entry.id, request, () => (this.editingId = null));
  }

  private apply(id: number, request: UpdateGardenEntryRequest, done?: () => void): void {
    this.garden.update(id, request).subscribe({
      next: (updated) => {
        this.entries = this.entries.map((e) => (e.id === id ? updated : e));
        done?.();
      },
      error: () => (this.error = 'Could not save changes.')
    });
  }

  private emptyEdit(): EditModel {
    return {
      nickname: '',
      notes: '',
      wateringOverrideDays: null,
      isIndoor: false,
      imageUrl: null,
      photoTouched: false,
      isCustom: false,
      displayName: '',
      scientificName: '',
      lightLevel: null
    };
  }

  // Maps the display label back to the representative level used by the picker.
  private lightLevelFromLabel(label: string): number | null {
    switch (label) {
      case 'Full sun':
        return 8;
      case 'Partial shade':
        return 5;
      case 'Shade':
        return 2;
      default:
        return null;
    }
  }
}
