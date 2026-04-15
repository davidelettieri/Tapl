$ErrorActionPreference = "Stop"

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path

& (Join-Path $scriptDirectory "Arith/Grammar/gen.ps1")
& (Join-Path $scriptDirectory "FullSimple/Grammar/gen.ps1")
& (Join-Path $scriptDirectory "LetExercise/Grammar/gen.ps1")
& (Join-Path $scriptDirectory "SimpleBool/Grammar/gen.ps1")
& (Join-Path $scriptDirectory "Untyped/Grammar/gen.ps1")