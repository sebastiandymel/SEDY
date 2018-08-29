    Get-ChildItem -re -in *.cs |  
    Where-Object{ -not $_.PSIsContainer }| 
    Where-Object{$_.FullName -notmatch 'obj'} |
    Where-Object{$_.FullName -notmatch 'Test'} |  
    Where-Object{$_.FullName -notmatch 'GlobalSuppression'} |  
    Where-Object{$_.FullName -notmatch 'Resource.Designer'} | 
    Where-Object {
                 $(Get-Content $_.FullName |
                  Measure-Object -Line).Lines -gt 1000   }|
    Sort-Object Length -descending |   
    Select-Object FullName -first 1000 > bigcsfiles.txt