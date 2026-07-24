import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PlantDetail, PlantSearchResult } from './models';

@Injectable({ providedIn: 'root' })
export class PlantService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/plants`;

  /** Results per page, mirrors the server's Trefle page size. */
  readonly pageSize = 20;

  search(query: string, page = 1): Observable<PlantSearchResult> {
    const params = new HttpParams().set('search', (query ?? '').trim()).set('page', page);
    return this.http.get<PlantSearchResult>(this.baseUrl, { params });
  }

  getById(trefleId: number): Observable<PlantDetail> {
    return this.http.get<PlantDetail>(`${this.baseUrl}/${trefleId}`);
  }
}
