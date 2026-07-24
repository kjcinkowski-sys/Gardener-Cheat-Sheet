// Default production values. On the deploy host (Vercel), set the API_URL environment
// variable instead of editing this file — set-env.js overwrites apiUrl below at build time
// with API_URL (which must include the /api suffix, e.g. https://gardener-api.onrender.com/api).
// See DEPLOY.md.
export const environment = {
  production: true,
  apiUrl: '/api',
};
