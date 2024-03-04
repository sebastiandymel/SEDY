param (
    [Parameter(Mandatory=$true)]
    [string]$solutionFilePath
)

try {
    # Ensure the file exists
    if (-not (Test-Path $solutionFilePath)) {
        throw "File not found: $solutionFilePath"
    }

    # Read the file content
    $content = Get-Content $solutionFilePath

    # Find lines that reference .csproj files
    $projectLines = $content | Where-Object { $_ -match '\.csproj"' }

    # Count the number of .csproj references
    $projectCount = $projectLines.Count

    Write-Output "Number of .csproj files: $projectCount"
}
catch {
    Write-Error $_.Exception.Message
}