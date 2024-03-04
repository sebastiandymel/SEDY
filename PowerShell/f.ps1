Get-ChildItem -Path "C:\GIT\Phoenix" -Filter *.xml -Recurse |
ForEach-Object {
    $lines = (Get-Content $_.FullName | Measure-Object -Line).Lines
    [PSCustomObject]@{
        FullName = $_.FullName
        Lines = $lines
    }
} |
Sort-Object Lines -Descending |
Select-Object -First 20