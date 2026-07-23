export interface UpdateGardenEntryRequest {
  nickname?: string;
  notes?: string;
  lastWatered?: string;
  wateringOverrideDays?: number;
  clearWateringOverride?: boolean;
  isIndoorOverride?: boolean | null;
  imageUrl?: string;
  clearImage?: boolean;
  // Custom-plant identity/care edits (ignored server-side for Trefle plants).
  displayName?: string;
  scientificName?: string;
  lightLevel?: number;
  clearLightLevel?: boolean;
}
