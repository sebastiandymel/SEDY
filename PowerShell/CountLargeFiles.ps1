(Get-ChildItem -re -in *.cs |  
Where-Object{ -not $_.PSIsContainer }| 
Where-Object{$_.FullName -notmatch 'obj'} |
Where-Object{$_.FullName -notmatch 'Test'} |  
Where-Object{$_.FullName -notmatch 'GlobalSuppression'} |  
Where-Object{$_.FullName -notmatch 'Resource.Designer'} | 
ForEach-Object { @{ File=$_.FullName; Lines= $(Get-Content $_.FullName |  Measure-Object -Line).Lines } }|
Where-Object { $_.Lines  -gt 1000   }|
Select-Object -first 1000).ForEach({[PSCustomObject]$_}) |
Sort-Object -Property Lines -Descending |
Format-Table -AutoSize > bigcsfiles.txt
