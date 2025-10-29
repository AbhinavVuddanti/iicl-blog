param(
  [Parameter(Mandatory=$true)][string]$PublishProfilePath,
  [Parameter(Mandatory=$true)][string]$ProjectPath,
  [string]$Configuration = "Release"
)

$ErrorActionPreference = 'Stop'

Write-Host "Reading publish profile..." -ForegroundColor Cyan
[xml]$xml = Get-Content -LiteralPath $PublishProfilePath
$pp = $xml.publishData.publishProfile | Where-Object { $_.publishMethod -eq 'MSDeploy' -or $_.publishMethod -eq 'ZipDeploy' } | Select-Object -First 1
if (-not $pp) { throw "No MSDeploy/ZipDeploy profile found in $PublishProfilePath" }

# Build zipdeploy URL from publishUrl (usually *.scm.azurewebsites.net:443)
$pubUrl = $pp.publishUrl
if (-not $pubUrl) { throw "publishUrl missing in publish profile" }
# Extract host without port (avoid conflict with automatic $Host variable)
$kuduHost = $pubUrl -replace ':\d+$',''
if ($kuduHost -notmatch '^https?://') { $kuduHost = "https://$kuduHost" }
$zipDeployUrl = "$kuduHost/api/zipdeploy"

# Credentials
$user = $pp.userName
$pass = $pp.userPWD
$secpass = ConvertTo-SecureString $pass -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential($user, $secpass)

# Publish
$publishDir = Join-Path (Resolve-Path $ProjectPath) "bin\$Configuration\net8.0\publish"
Write-Host "Publishing project to $publishDir..." -ForegroundColor Cyan
& dotnet publish $ProjectPath -c $Configuration -o $publishDir

# Zip package (unique filename, create in TEMP to avoid locking by inclusion)
$zipName = "site-" + [Guid]::NewGuid().ToString("N") + ".zip"
$zipRoot = [System.IO.Path]::GetTempPath()
$zipPath = Join-Path $zipRoot $zipName
Write-Host "Creating zip package $zipPath from $publishDir ..." -ForegroundColor Cyan
Add-Type -AssemblyName System.IO.Compression.FileSystem

$maxRetries = 3
for ($i = 1; $i -le $maxRetries; $i++) {
  try {
    if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
    [System.IO.Compression.ZipFile]::CreateFromDirectory($publishDir, $zipPath)
    break
  } catch {
    if ($i -eq $maxRetries) { throw }
    Write-Warning "Zip create failed (attempt $i). Waiting 2s and retrying... $_"
    Start-Sleep -Seconds 2
  }
}

# Deploy via Kudu ZipDeploy
Write-Host "Deploying to $zipDeployUrl ..." -ForegroundColor Cyan
$resp = Invoke-RestMethod -Uri $zipDeployUrl -Method Post -InFile $zipPath -ContentType 'application/zip' -Credential $cred -TimeoutSec 1800
Write-Host "Deployment triggered. Response:" -ForegroundColor Green
$resp | ConvertTo-Json -Depth 5

Write-Host "Done. Visit your site: https://$($pp.msdeploySite).azurewebsites.net" -ForegroundColor Green
