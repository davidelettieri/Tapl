$ErrorActionPreference = "Stop"

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (Resolve-Path (Join-Path $scriptDirectory "../..")).Path

& (Join-Path $repoRoot "scripts/run-antlr-in-docker.ps1") "FullSimple/Grammar" "FullSimple.g4"
exit $LASTEXITCODE