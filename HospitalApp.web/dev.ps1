# Development environment management script

param(
    [string]$command = "start"
)

function Start-DevEnvironment {
    Write-Host "Starting development environment..." -ForegroundColor Green
    docker-compose -f docker-compose.development.yml up --build
}

function Stop-DevEnvironment {
    Write-Host "Stopping development environment..." -ForegroundColor Yellow
    docker-compose -f docker-compose.development.yml down
}

function Clean-DevEnvironment {
    Write-Host "Cleaning development environment..." -ForegroundColor Red
    docker-compose -f docker-compose.development.yml down -v
    docker system prune -f
}

switch ($command) {
    "start" { Start-DevEnvironment }
    "stop" { Stop-DevEnvironment }
    "clean" { Clean-DevEnvironment }
    default { Write-Host "Unknown command. Use: start, stop, or clean" -ForegroundColor Red }
} 