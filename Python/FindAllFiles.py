import os
import re
from os.path import expanduser
listOfFiles = []
for root, directories, filenames in os.walk(r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\Oasis\\BrandViews'):
#for root, directories, filenames in os.walk(r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\GenieMedical\\GenieMedicalViews'):
#for root, directories, filenames in os.walk(r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\Oasis\\BrandViews'):
    for filename in filenames:           
        #if (re.match("(.*)Medical(.*)View.xaml$", filename) or re.match("(.*)Medical(.*)ViewModel.cs$", filename) or re.match("(.*)Medical(.*)Module.cs$", filename)): 
        if re.match("(.*)EF(.*)ViewModel.cs$", filename): 
        #if re.match("(.*)EF(.*)View.xaml$", filename): 
        #if re.match("(.*)View.xaml$", filename): 
                listOfFiles.append(filename)

print (*listOfFiles, sep = "\n")
print (len(listOfFiles))
        #print(os.path.join(root,filename))