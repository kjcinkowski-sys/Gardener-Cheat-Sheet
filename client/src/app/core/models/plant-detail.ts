import { WateringSchedule } from './watering-schedule';

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
