#!/usr/bin/env pwsh
# Runs the Gardener Cheat Sheet backend (ASP.NET Core Web API).
#
# Usage (from anywhere):
#   ./src/run.ps1
#
# Listens on http://localhost:5080 by default (see Api/Properties/launchSettings.json).
# Set your Trefle token first so plant search works:
#   cd src/GardenerCheatSheet.Api; dotnet user-secrets set "Trefle:Token" "YOUR_TOKEN"
#   # or:  $env:Trefle__Token = "YOUR_TOKEN"
# The SQLite database (gardener.db) is created automatically on first run.

$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project GardenerCheatSheet.Api $args
} finally {
    Pop-Location
}
