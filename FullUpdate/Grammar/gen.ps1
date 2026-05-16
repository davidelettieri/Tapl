$ErrorActionPreference = "Stop"

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (Resolve-Path (Join-Path $scriptDirectory "../..")).Path

& (Join-Path $repoRoot "scripts/run-antlr-in-docker.ps1") "FullUpdate/Grammar" "FullUpdate.g4"
exit $LASTEXITCODE
