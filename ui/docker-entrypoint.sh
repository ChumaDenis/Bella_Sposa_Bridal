#!/bin/sh
set -e

# Inject runtime API base URL for the Angular app (set API_BASE in Railway Variables).
if [ -n "$API_BASE" ]; then
  echo "window.__env = { apiBase: '$API_BASE' };" > /srv/env.js
  echo "[entrypoint] env.js generated with API_BASE=$API_BASE"
else
  echo "[entrypoint] API_BASE not set — using build-time default API URL"
fi

# API origin (scheme + host) for Caddy's /sitemap.xml reverse proxy.
API_ORIGIN="${API_BASE:-https://bellasposabridal-production-073a.up.railway.app/api}"
API_ORIGIN="${API_ORIGIN%/api}"
export API_ORIGIN
echo "[entrypoint] sitemap proxy target: $API_ORIGIN"

exec caddy run --config /etc/caddy/Caddyfile --adapter caddyfile
