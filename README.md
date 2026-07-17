# Gardener Cheat Sheet

A web app for browsing plants, viewing their care needs (photos, watering
schedule, light needs, indoor/outdoor, and more), and adding them to your own
garden. Plant data comes from the free [Trefle API](https://trefle.io).

- **Frontend:** Angular 18 (standalone components) — browse, detail, and My Garden
- **Backend:** ASP.NET Core Web API (C#), object-oriented / layered design
- **Persistence:** EF Core + SQLite
- **External data:** Trefle (proxied through the backend so the token stays server-side)

> **Status:** Backend for the single-user MVP is in place (plant catalog + garden
> management). The Angular frontend and authentication are planned next. See the
> phased plan for the full roadmap.

## Architecture

The backend follows a layered, object-oriented design. Dependencies point
inward: `Api → Application → Domain`, with `Infrastructure` implementing the
interfaces the `Application` layer defines.

```
src/
  GardenerCheatSheet.Domain          Entities & value objects (Plant, GardenEntry,
                                     WateringSchedule, Garden) — no framework deps
  GardenerCheatSheet.Application     Service + repository interfaces, DTOs, services,
                                     mapping (PlantCatalogService, GardenService,
                                     WateringScheduleCalculator)
  GardenerCheatSheet.Infrastructure  EF Core (AppDbContext, repositories) + Trefle
                                     HTTP client (with in-memory caching)
  GardenerCheatSheet.Api             Controllers, DI wiring, CORS, Swagger
tests/
  GardenerCheatSheet.UnitTests       Unit tests for the domain / watering logic
```

### Key design notes

- **Watering schedule.** Trefle has no watering field, so
  `WateringScheduleCalculator` derives a cadence from growth data (precipitation,
  then light as a fallback). Users can override it per plant; `GardenEntry`
  resolves the effective schedule.
- **Indoor / outdoor.** Also not a Trefle field. `Plant` carries a heuristic
  default (`GuessIsIndoor`) that a `GardenEntry` can override.
- **Trefle is proxied.** The Angular app never calls Trefle directly — the API
  holds the token, maps Trefle's JSON into our own DTOs, and caches responses.

## Running the backend

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download).

1. **Set your Trefle token** (get one free at https://trefle.io). Never commit it —
   use user-secrets or an environment variable:

   ```bash
   cd src/GardenerCheatSheet.Api
   dotnet user-secrets init
   dotnet user-secrets set "Trefle:Token" "YOUR_TREFLE_TOKEN"
   # or:  export Trefle__Token=YOUR_TREFLE_TOKEN
   ```

2. **Run the API:**

   ```bash
   dotnet run --project src/GardenerCheatSheet.Api
   ```

   The SQLite database (`gardener.db`) is created automatically on first run.
   Swagger UI is available at the root in Development.

3. **Try it:**

   ```
   GET  /health
   GET  /api/plants?search=rose
   GET  /api/plants/{trefleId}
   GET  /api/garden
   POST /api/garden           { "trefleId": 123, "nickname": "Front porch rose" }
   PUT  /api/garden/{id}       { "lastWatered": "2026-07-17", "wateringOverrideDays": 5 }
   DELETE /api/garden/{id}
   ```

## Running the frontend

Requires Node 18+ and the backend running on `http://localhost:5080`.

```bash
cd client
npm install
npm start          # ng serve on http://localhost:4200
```

`ng serve` proxies `/api/*` to the backend via `client/proxy.conf.json`, so no
CORS setup is needed in dev. Open http://localhost:4200 to browse plants and
build your garden.

## Running the tests

```bash
dotnet test                 # backend (domain / watering logic)
cd client && npm test       # frontend (Angular / Karma)
```

## Roadmap

1. **Foundations** — solution, EF Core + SQLite, Trefle client, CORS/health ✅
2. **Plant catalog** — search + detail with derived watering/light/indoor ✅ (backend)
3. **My Garden** — add/remove, overrides, watering tracking ✅ (backend)
4. **Angular frontend** — browse, detail, and garden UIs ✅
5. **Polish** — due-today dashboard, SQLite response cache, more tests
6. **Multi-user** — ASP.NET Core Identity + JWT, per-user gardens
