function RemoveBinAndObjDirectories($directoryPath) {
    Get-ChildItem -Path $directoryPath -Recurse -Directory -Include bin,obj | Remove-Item -Recurse -Force
}

function CountBigFiles($directoryPath, $extension, $outputFile) {
    # Ensure the directory exists
    if (-not (Test-Path $directoryPath -PathType Container)) {
        Write-Error "Directory not found: $directoryPath"
        return
    }

    (Get-ChildItem -Path $directoryPath -Recurse -Include *.$extension |  
    Where-Object{ -not $_.PSIsContainer }| 
    Where-Object{$_.FullName -notmatch 'GlobalSuppression'} |  
    Where-Object{$_.FullName -notmatch 'Resource.Designer'} | 
    ForEach-Object { @{ File=$_.FullName; Lines= $(Get-Content $_.FullName |  Measure-Object -Line).Lines } }|
    Where-Object { $_.Lines  -gt 1   }|
    Select-Object -first 20).ForEach({[PSCustomObject]$_}) |
    Sort-Object -Property Lines -Descending |
    Format-Table -AutoSize > $outputFile
}

$directoryPath = 'C:\GIT\Phoenix\'
RemoveBinAndObjDirectories $directoryPath
CountBigFiles $directoryPath 'cs' 'big_cs_files.txt'
CountBigFiles $directoryPath 'xml' 'big_xml_files.txt'
CountBigFiles $directoryPath 'json' 'big_json_files.txt'