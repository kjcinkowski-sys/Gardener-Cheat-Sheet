#!/usr/bin/env pwsh
# Runs the Gardener Cheat Sheet frontend (Angular dev server).
#
# Usage (from anywhere):
#   ./client/run.ps1
#
# Serves on http://localhost:4200 and proxies /api/* to the backend on
# http://localhost:5080 (see proxy.conf.json), so start the backend too:
#   ./src/run.ps1
# Installs npm dependencies automatically on first run.

$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    if (-not (Test-Path node_modules)) {
        npm install
    }
    npm start $args
} finally {
    Pop-Location
}
