import { WateringSchedule } from './watering-schedule';

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
