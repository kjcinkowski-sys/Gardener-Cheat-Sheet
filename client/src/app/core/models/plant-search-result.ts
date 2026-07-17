import { PlantSummary } from './plant-summary';

export interface PlantSearchResult {
  plants: PlantSummary[];
  page: number;
  totalPages?: number;
}
