param(
    [Parameter(Mandatory = $true)]
    [string]$GrammarDirectory,

    [Parameter(Mandatory = $true)]
    [string]$GrammarFile
)

$ErrorActionPreference = "Stop"

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (Resolve-Path (Join-Path $scriptDirectory "..")).Path
$normalizedGrammarDirectory = $GrammarDirectory.TrimEnd("/", "\\") -replace "\\", "/"
$hostGrammarDirectory = Join-Path $repoRoot $normalizedGrammarDirectory
$dockerfilePath = Join-Path $repoRoot "docker/antlr-generator/Dockerfile"
$imageName = if ($env:ANTLR_DOCKER_IMAGE) { $env:ANTLR_DOCKER_IMAGE } else { "tapl-antlr-generator" }
$grammarName = [System.IO.Path]::GetFileNameWithoutExtension($GrammarFile)

if (-not (Test-Path -Path $hostGrammarDirectory -PathType Container)) {
    throw "Grammar directory not found: $hostGrammarDirectory"
}

if (-not (Test-Path -Path (Join-Path $hostGrammarDirectory $GrammarFile) -PathType Leaf)) {
    throw "Grammar file not found: $(Join-Path $hostGrammarDirectory $GrammarFile)"
}

@(
    "$grammarName.interp",
    "$grammarName.tokens",
    "${grammarName}BaseVisitor.cs",
    "${grammarName}Lexer.cs",
    "${grammarName}Lexer.interp",
    "${grammarName}Lexer.tokens",
    "${grammarName}Parser.cs",
    "${grammarName}Visitor.cs"
) | ForEach-Object {
    $artifactPath = Join-Path $hostGrammarDirectory $_
    if (Test-Path -Path $artifactPath -PathType Leaf) {
        Remove-Item -Path $artifactPath
    }
}

& docker build -t $imageName -f $dockerfilePath $repoRoot
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$volumeSpec = "${repoRoot}:/workspace"
$containerGrammarDirectory = "/workspace/$normalizedGrammarDirectory"

$containerCommand = "set -eu && mkdir -p /tmp/antlr-out && cp $containerGrammarDirectory/$GrammarFile /tmp/$GrammarFile && java -jar /opt/antlr/antlr-4.13.1-complete.jar -Dlanguage=CSharp -o /tmp/antlr-out /tmp/$GrammarFile -visitor -no-listener && cp /tmp/antlr-out/* $containerGrammarDirectory/"

& docker run --rm -v $volumeSpec --entrypoint sh $imageName -lc $containerCommand
exit $LASTEXITCODE