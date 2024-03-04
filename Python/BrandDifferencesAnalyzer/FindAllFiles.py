import os
import re
from os.path import expanduser
from datetime import datetime

def Analyze(title: str,dirPath: str, regPattern: str):
    """
        Analyze given directory for specific regex path files \n
        List all the files and append to output.txt.
    """
    listOfFiles = []
    for root, directories, filenames in os.walk(dirPath):
        for filename in filenames:
            if re.match(regPattern, filename): 
                listOfFiles.append(filename)
    
    listOfFiles.sort()
    
    print(title)
    print (*listOfFiles, sep = "\n")
    
    # Prepare list for writing
    for i in range(len(listOfFiles)):
        listOfFiles[i] = listOfFiles[i] + '\n'
    # Write to file    

    shortDate = datetime.today().strftime('%Y-%m-%d')
    f = open(shortDate + "-output.txt", "a")
    f.write('\n' + title + '\n')
    if len(listOfFiles):
        f.writelines(listOfFiles)
    else:
        f.write('-- No differences!\n')

# EXPRESSFIT
pathToBrandViews = r'C:\\GIT\\Phoenix\\APP\\UserInterface\\BrandViews\\Oasis\\BrandViews'
Analyze(
    "### ExpressFit specific ViewModels:", 
    pathToBrandViews, 
    "(.*)EF(.*)ViewModel.cs$"
    )
Analyze(
    "### ExpressFit specific Views:", 
    pathToBrandViews, 
    "(.*)EF(.*)View.xaml$"
    )

# GENIE MEDICAL
pathToGenieMedicalViews = r'C:\\GIT\\Phoenix\\APP\UserInterface\\BrandViews\\GenieMedical\\GenieMedicalViews'
Analyze(
    "### Medical specific ViewModels:", 
    pathToGenieMedicalViews, 
    "(.*)ViewModel.cs$"
    )
Analyze(
    "### Medical specific Views:", 
    pathToGenieMedicalViews, 
    "(.*)View.xaml$"
    )

# HEAR SUITE
Analyze(
    "### HearSuite specific ViewModels:", 
    pathToBrandViews,
    "(.*)HS(.*)ViewModel.cs$"
    )
Analyze(
    "### HearSuite specific Views:", 
    pathToBrandViews,
    "(.*)HS(.*)View.xaml$"
    )


# GENIE
pathToGenieViews = r'c:\\GIT\\Phoenix\\APP\\UserInterface\BrandViews\\GenieShared\\Views'
Analyze(
    "### GenieShared specific Views:", 
    pathToGenieViews, 
    "(.*)View.xaml$"
    )
pathToGenieViews = r'c:\\GIT\\Phoenix\\APP\\UserInterface\BrandViews\\Genie\\GenieViews'
Analyze(
    "### Genie specific Views:", 
    pathToGenieViews, 
    "(.*)View.xaml$"
    )