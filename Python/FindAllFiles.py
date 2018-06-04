import os
import re
from os.path import expanduser
listOfFiles = []
for root, directories, filenames in os.walk(r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\Oasis\\BrandViews'):
    for filename in filenames:           
     if re.match("(.*)EF(.*)View.xaml$", filename): 
     #if re.match("(.*)View.xaml$", filename): 
                listOfFiles.append(filename)

print (*listOfFiles, sep = "\n")
print (len(listOfFiles))
        #print(os.path.join(root,filename))