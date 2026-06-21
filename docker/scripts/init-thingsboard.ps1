# First-time ThingsBoard initialization (Windows PowerShell)
# Run from repository root:
#
#   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
#   .\docker\scripts\init-thingsboard.ps1
#
# Or without the script (manual):
#   cd docker
#   docker compose up -d postgres
#   docker compose run --rm -e INSTALL_TB=true -e LOAD_DEMO=false thingsboard-ce
#   docker compose up -d

$ErrorActionPreference = "Stop"
$ComposeFile = Join-Path $PSScriptRoot "..\docker-compose.yml"

Write-Host "Starting PostgreSQL..."
docker compose -f $ComposeFile up -d postgres

Write-Host "Waiting for PostgreSQL to become healthy..."
$retries = 30
for ($i = 0; $i -lt $retries; $i++) {
    $status = docker compose -f $ComposeFile ps postgres --format json 2>$null
    if ($status -match '"Health":"healthy"') { break }
    Start-Sleep -Seconds 2
}

Write-Host "Initializing ThingsBoard schema (clean install, no demo data)..."
docker compose -f $ComposeFile run --rm `
    -e INSTALL_TB=true `
    -e LOAD_DEMO=false `
    thingsboard-ce

Write-Host "Starting ThingsBoard..."
docker compose -f $ComposeFile up -d

Write-Host ""
Write-Host "Done. Open http://localhost:8080"
Write-Host "Login: sysadmin@thingsboard.org / sysadmin"
Write-Host "Next: follow docs/thingsboard/SETUP.md"
