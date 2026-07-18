export interface UpdateGardenEntryRequest {
  nickname?: string;
  notes?: string;
  lastWatered?: string;
  wateringOverrideDays?: number;
  clearWateringOverride?: boolean;
  isIndoorOverride?: boolean | null;
}
