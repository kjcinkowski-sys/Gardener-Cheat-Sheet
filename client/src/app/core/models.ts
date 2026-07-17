// TypeScript shapes mirroring the backend DTOs (camelCased by ASP.NET Core).

export interface WateringSchedule {
  category: string;
  daysBetweenWatering: number;
  source: string;
}

export interface PlantSummary {
  trefleId: number;
  displayName: string;
  scientificName?: string;
  imageUrl?: string;
  isIndoor: boolean;
}

export interface PlantSearchResult {
  plants: PlantSummary[];
  page: number;
  totalPages?: number;
}

export interface PlantDetail {
  trefleId: number;
  displayName: string;
  scientificName: string;
  commonName?: string;
  family?: string;
  imageUrl?: string;
  description?: string;
  soilTexture?: string;
  lightRequirement: string;
  isIndoor: boolean;
  watering: WateringSchedule;
}

export interface GardenEntry {
  id: number;
  trefleId: number;
  displayName: string;
  nickname?: string;
  scientificName?: string;
  imageUrl?: string;
  notes?: string;
  isIndoor: boolean;
  lightRequirement: string;
  watering: WateringSchedule;
  dateAdded: string;
  lastWatered?: string;
  nextWateringDate?: string;
  isDue: boolean;
}

export interface AddGardenEntryRequest {
  trefleId: number;
  nickname?: string;
}

export interface UpdateGardenEntryRequest {
  nickname?: string;
  notes?: string;
  lastWatered?: string;
  wateringOverrideDays?: number;
  clearWateringOverride?: boolean;
  isIndoorOverride?: boolean | null;
}
