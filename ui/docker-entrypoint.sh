#!/bin/sh
set -e

# Inject runtime API base URL for the Angular app (set API_BASE in Railway Variables).
if [ -n "$API_BASE" ]; then
  echo "window.__env = { apiBase: '$API_BASE' };" > /srv/env.js
  echo "[entrypoint] env.js generated with API_BASE=$API_BASE"
else
  echo "[entrypoint] API_BASE not set — using build-time default API URL"
fi

exec caddy run --config /etc/caddy/Caddyfile --adapter caddyfile
