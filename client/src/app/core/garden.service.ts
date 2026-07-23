import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AddCustomPlantRequest,
  AddGardenEntryRequest,
  GardenEntry,
  UpdateGardenEntryRequest
} from './models';

@Injectable({ providedIn: 'root' })
export class GardenService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/garden';

  getGarden(): Observable<GardenEntry[]> {
    return this.http.get<GardenEntry[]>(this.baseUrl);
  }

  add(request: AddGardenEntryRequest): Observable<GardenEntry> {
    return this.http.post<GardenEntry>(this.baseUrl, request);
  }

  addCustom(request: AddCustomPlantRequest): Observable<GardenEntry> {
    return this.http.post<GardenEntry>(`${this.baseUrl}/custom`, request);
  }

  update(id: number, request: UpdateGardenEntryRequest): Observable<GardenEntry> {
    return this.http.put<GardenEntry>(`${this.baseUrl}/${id}`, request);
  }

  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
