#!/usr/bin/env bash
# First-time ThingsBoard initialization (Linux / macOS)
# Run from repository root: bash docker/scripts/init-thingsboard.sh

set -euo pipefail

COMPOSE_FILE="$(cd "$(dirname "$0")/.." && pwd)/docker-compose.yml"

echo "Starting PostgreSQL..."
docker compose -f "$COMPOSE_FILE" up -d postgres

echo "Waiting for PostgreSQL to become healthy..."
for _ in $(seq 1 30); do
  if docker compose -f "$COMPOSE_FILE" ps postgres | grep -q "(healthy)"; then
    break
  fi
  sleep 2
done

echo "Initializing ThingsBoard schema (clean install, no demo data)..."
docker compose -f "$COMPOSE_FILE" run --rm \
  -e INSTALL_TB=true \
  -e LOAD_DEMO=false \
  thingsboard-ce

echo "Starting ThingsBoard..."
docker compose -f "$COMPOSE_FILE" up -d

echo ""
echo "Done. Open http://localhost:8080"
echo "Login: sysadmin@thingsboard.org / sysadmin"
echo "Next: follow docs/thingsboard/SETUP.md"
