// TypeScript shapes mirroring the backend DTOs (camelCased by ASP.NET Core).

export interface WateringSchedule {
  category: string;
  daysBetweenWatering: number;
  source: string;
}
