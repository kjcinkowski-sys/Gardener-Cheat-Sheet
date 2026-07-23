export interface AddCustomPlantRequest {
  displayName: string;
  scientificName?: string;
  nickname?: string;
  imageUrl?: string;
  isIndoor: boolean;
  wateringIntervalDays?: number;
  lightLevel?: number;
  notes?: string;
}
