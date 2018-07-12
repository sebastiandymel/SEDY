get-childitem -Include  obj -Recurse -force | % { $_.FullName } | Remove-Item -Force –Recurse
get-childitem -Include bin -Recurse -force | % { $_.FullName } | Remove-Item -Force –Recurse
get-childitem -Include _Result -Recurse -force | % { $_.FullName } | Remove-Item -Force –Recurse
#get-childitem -Include packages -Recurse -force | % { $_.FullName } | Remove-Item -Force –Recurse