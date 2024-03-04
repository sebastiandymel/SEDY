param (
    [Parameter(Mandatory=$true)]
    [string]$directoryPath
)

try {
    # Ensure the directory exists
    if (-not (Test-Path $directoryPath -PathType Container)) {
        throw "Directory not found: $directoryPath"
    }

    # Get all .csproj files in the directory and its subdirectories
    $projectFiles = Get-ChildItem -Path $directoryPath -Filter *.csproj -Recurse -ErrorAction Stop

    # Count the number of .csproj files
    $projectCount = $projectFiles.Count

    Write-Output "Number of .csproj files: $projectCount"
}
catch {
    Write-Error $_.Exception.Message
}