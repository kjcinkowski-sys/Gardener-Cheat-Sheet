# Deploying Gardener Cheat Sheet

The app is three pieces plus a photo store:

| Piece | Host | Notes |
| --- | --- | --- |
| Angular frontend (`client/`) | **Vercel** | Static SPA build |
| .NET API (`src/`) | **Render** (or Railway / Fly) | Vercel cannot run ASP.NET Core |
| PostgreSQL | **Neon** | Free managed Postgres |
| Plant photos | **Cloudflare R2** | S3-compatible object storage (app hosts have ephemeral disks) |

Do them in this order: **database → photo bucket → API → frontend**, because each needs values from the previous.

---

## 1. Database (Neon)

1. Create a Postgres database at [neon.tech](https://neon.tech).
2. Copy the connection string. Neon gives a URI like
   `postgresql://user:pass@ep-cool-name.aws.neon.tech/dbname`.
3. **Convert it to Npgsql's keyword format** — the .NET driver does not accept the URI form:

   ```
   Host=ep-cool-name.aws.neon.tech;Database=dbname;Username=user;Password=pass;SSL Mode=Require;Trust Server Certificate=true
   ```

You don't run migrations by hand — the API applies them on startup (`db.Database.Migrate()`), creating
the schema on first boot.

---

## 2. Photo bucket (Cloudflare R2)

1. In Cloudflare, create an **R2 bucket** (e.g. `gardener-photos`).
2. Enable public access — either the bucket's **r2.dev** public URL or a custom domain. Note that
   public base URL (e.g. `https://pub-xxxx.r2.dev`).
3. Create an **R2 API token** (Object Read & Write) — note the Access Key ID and Secret Access Key.
4. Your S3 endpoint is `https://<account-id>.r2.cloudflarestorage.com`.

---

## 3. API (Render)

Deploy as a **Web Service** using the repo-root [Dockerfile](Dockerfile). Point Render at the repo;
leave the root directory at the repo root (the Dockerfile copies `src/`).

Set these **environment variables** (double underscores map to nested config keys — see
[.env.example](.env.example)):

| Variable | Value |
| --- | --- |
| `ConnectionStrings__Default` | the Npgsql string from step 1 |
| `Trefle__Token` | your [Trefle](https://trefle.io) API token |
| `Cors__AllowedOrigins__0` | your frontend URL, e.g. `https://gardener.vercel.app` (no trailing slash) |
| `Storage__Provider` | `S3` |
| `Storage__S3__ServiceUrl` | `https://<account-id>.r2.cloudflarestorage.com` |
| `Storage__S3__Bucket` | your bucket name |
| `Storage__S3__AccessKey` | R2 access key ID |
| `Storage__S3__SecretKey` | R2 secret access key |
| `Storage__S3__PublicBaseUrl` | the public URL from step 2, e.g. `https://pub-xxxx.r2.dev` |
| `Storage__S3__Region` | `auto` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

On first deploy the API connects to Neon and creates the tables. Note the API's public URL
(e.g. `https://gardener-api.onrender.com`) — the frontend needs it next.

> **Cold starts:** Render's free web services sleep after ~15 min idle; the first request after a lull
> takes ~30s. Fine for friends/demos.

---

## 4. Frontend (Vercel)

Import the repo and set the **Root Directory** to `client`. The committed
[client/vercel.json](client/vercel.json) handles the build command, the output directory
(`dist/client/browser`), and the SPA rewrite that keeps deep links working on refresh.

Set one **environment variable**:

| Variable | Value |
| --- | --- |
| `API_URL` | your API URL **with `/api`**, e.g. `https://gardener-api.onrender.com/api` |

At build time [client/set-env.js](client/set-env.js) bakes `API_URL` into the app.

---

## Local development

```bash
docker compose up -d                          # local Postgres on :5432
dotnet run --project src/GardenerCheatSheet.Api   # API on http://localhost:5080 (uses local disk for photos)
cd client && npm start                        # Angular dev server on http://localhost:4200
```

The API's default connection string points at the compose Postgres; photo storage defaults to local
disk (`Storage:Provider=Local`). Set your Trefle token for local use via user-secrets:
`dotnet user-secrets set "Trefle:Token" "<token>" --project src/GardenerCheatSheet.Api`.

---

## Checklist / gotchas

- **CORS 403s** — the Vercel frontend URL must exactly match `Cors__AllowedOrigins__0` (scheme + host,
  no trailing slash).
- **`API_URL` must include `/api`** — all backend routes live under `/api/...`.
- **R2 public access** — if photos 404, the bucket isn't public or `PublicBaseUrl` is wrong.
- **No auth yet** — the app currently has a single shared garden with no login. See the note below
  before opening it beyond a small trusted group.
