Get-ChildItem . -Filter "*.cs" -Recurse |
    Where-Object { 
        ($_.FullName -notcontains "obj") -and
                 
        ((  
        Get-Content $_.FullName |
         Measure-Object -Line |
         Select-Object -ExpandProperty Lines) -gt 2000)
    } | Out-File largeFiles.txt