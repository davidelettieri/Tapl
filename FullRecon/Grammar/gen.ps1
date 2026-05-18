$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
& "$repoRoot/scripts/run-antlr-in-docker.sh" "FullRecon/Grammar" "FullRecon.g4"
