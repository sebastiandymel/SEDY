import os
import re
from os.path import expanduser

def Analyze(title: str,dirPath: str, regPattern: str):
    listOfFiles = []
    for root, directories, filenames in os.walk(dirPath):
        for filename in filenames:
            if re.match(regPattern, filename): 
                listOfFiles.append(filename)
    print(title)
    print (*listOfFiles, sep = "\n")
    for i in range(len(listOfFiles)):
        listOfFiles[i] = listOfFiles[i] + '\n'
    f = open("output.txt", "a")
    f.write(title + '\n')
    f.writelines(listOfFiles)

Analyze(
    "### ExpressFit specific ViewModels", 
    r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\Oasis\\BrandViews', 
    "(.*)EF(.*)ViewModel.cs$")
Analyze(
    "### ExpressFit specific Views", 
    r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\Oasis\\BrandViews', 
    "(.*)EF(.*)View.xaml$")
Analyze(
    "### Medical specific ViewModels", 
    r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\GenieMedical\\GenieMedicalViews', 
    "(.*)Medical(.*)View.xaml$")
Analyze(
    "### Medical specific Views", 
    r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\GenieMedical\\GenieMedicalViews', 
    "(.*)Medical(.*)ViewModel.cs$")


# listOfFiles = []
# for root, directories, filenames in os.walk(r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\Oasis\\BrandViews'):
# #for root, directories, filenames in os.walk(r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\GenieMedical\\GenieMedicalViews'):
# #for root, directories, filenames in os.walk(r'C:\\GIT\\Phoenix\\Integration\\Application\UserInterface\\Oasis\\BrandViews'):
#     for filename in filenames:           
#         #if (re.match("(.*)Medical(.*)View.xaml$", filename) or re.match("(.*)Medical(.*)ViewModel.cs$", filename) or re.match("(.*)Medical(.*)Module.cs$", filename)): 
#         #if re.match("(.*)EF(.*)ViewModel.cs$", filename): 
#         #if re.match("(.*)EF(.*)View.xaml$", filename): 
#         #if re.match("(.*)HS(.*)View.xaml$", filename): 
#         if re.match("(.*)HS(.*)ViewModel.cs$", filename): 
#         #if re.match("(.*)View.xaml$", filename): 
#                 listOfFiles.append(filename)

# print (*listOfFiles, sep = "\n")
# print (len(listOfFiles))
#         #print(os.path.join(root,filename))
