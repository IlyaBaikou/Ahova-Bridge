$ErrorActionPreference = "Stop"

$repository = if ($env:AHOVA_BRIDGE_SOURCE_URL) { $env:AHOVA_BRIDGE_SOURCE_URL } else { "https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge" }
$version = $env:AHOVA_BRIDGE_VERSION
$installDirectory = if ($env:AHOVA_BRIDGE_INSTALL_DIR) { $env:AHOVA_BRIDGE_INSTALL_DIR } else { Join-Path (Get-Location) "ahova-bridge" }

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw "Docker Desktop is required. Install it and start Docker before running this installer."
}
docker compose version | Out-Null

if (-not $version) {
    Write-Host "Finding the latest stable Ahova Bridge release..."
    $release = Invoke-RestMethod "https://api.github.com/repos/IlyaBaikou/Ahova-Bridge/releases/latest" -Headers @{ Accept = "application/vnd.github+json" }
    $version = $release.tag_name
    if (-not $version) { throw "Could not determine the latest stable release. Set AHOVA_BRIDGE_VERSION to a release tag and try again." }
}

if ((Test-Path $installDirectory) -and (Get-ChildItem $installDirectory -Force -ErrorAction SilentlyContinue) -and -not (Test-Path (Join-Path $installDirectory "compose.yaml"))) {
    throw "$installDirectory is not empty. Set AHOVA_BRIDGE_INSTALL_DIR to another folder."
}

New-Item -ItemType Directory -Force -Path (Join-Path $installDirectory "family-files") | Out-Null
Write-Host "Installing Ahova Bridge $version in $installDirectory..."
Invoke-WebRequest "$repository/$version/compose.yaml" -OutFile (Join-Path $installDirectory "compose.yaml")
Invoke-WebRequest "$repository/$version/.env.example" -OutFile (Join-Path $installDirectory ".env.example")
Invoke-WebRequest "$repository/$version/scripts/ahova-bridge.ps1" -OutFile (Join-Path $installDirectory "ahova-bridge.ps1")
if (-not (Test-Path (Join-Path $installDirectory ".env"))) {
    Copy-Item (Join-Path $installDirectory ".env.example") (Join-Path $installDirectory ".env")
}

Push-Location $installDirectory
try {
    if ($env:AHOVA_BRIDGE_SKIP_PULL -ne "true") { docker compose pull }
    docker compose up -d
} finally {
    Pop-Location
}

Write-Host "Waiting for the local setup page..."
for ($attempt = 0; $attempt -lt 30; $attempt++) {
    try {
        Invoke-WebRequest "http://127.0.0.1:7433/health/live" -UseBasicParsing | Out-Null
        Write-Host "Ahova Bridge is running. Opening http://127.0.0.1:7433"
        if ($env:AHOVA_BRIDGE_SKIP_OPEN -ne "true") { Start-Process "http://127.0.0.1:7433" }
        exit 0
    } catch {
        Start-Sleep -Seconds 1
    }
}
throw "The container started but the local setup page did not become healthy. Run .\ahova-bridge.ps1 doctor."
