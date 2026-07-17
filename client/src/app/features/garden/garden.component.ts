import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { GardenService } from '../../core/garden.service';
import { GardenEntry, UpdateGardenEntryRequest } from '../../core/models';

interface EditModel {
  nickname: string;
  notes: string;
  wateringOverrideDays: number | null;
  isIndoor: boolean;
}

@Component({
  selector: 'app-garden',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './garden.component.html',
  styleUrl: './garden.component.scss'
})
export class GardenComponent implements OnInit {
  private readonly garden = inject(GardenService);

  entries: GardenEntry[] = [];
  loading = false;
  error?: string;

  editingId: number | null = null;
  edit: EditModel = { nickname: '', notes: '', wateringOverrideDays: null, isIndoor: false };

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
      isIndoor: entry.isIndoor
    };
  }

  cancelEdit(): void {
    this.editingId = null;
  }

  saveEdit(entry: GardenEntry): void {
    const request: UpdateGardenEntryRequest = {
      nickname: this.edit.nickname,
      notes: this.edit.notes,
      isIndoorOverride: this.edit.isIndoor
    };
    if (this.edit.wateringOverrideDays && this.edit.wateringOverrideDays > 0) {
      request.wateringOverrideDays = this.edit.wateringOverrideDays;
    } else {
      request.clearWateringOverride = true;
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
}
