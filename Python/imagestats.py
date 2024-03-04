import os
import re
import PIL
from os.path import expanduser
from datetime import datetime
from PIL import Image
from glob import glob

def IdentifyImages(id: str, dirPath: str):
    listOfFiles = []
    result = [y for x in os.walk(dirPath) for y in glob(os.path.join(x[0], '*.png'))]
    for r in result:
        imgPath = r.replace("\\\\", "\\")                    
        im = Image.open(imgPath)
        width, height = im.size
        listOfFiles.append(r.replace(dirPath, "") + "," + str(width) + "x" + str(height) +"," + str(round(os.stat(imgPath).st_size / 1024, 3)))

    print (*listOfFiles, sep = "\n")
    
    # Prepare list for writing
    for i in range(len(listOfFiles)):
        listOfFiles[i] = listOfFiles[i] + '\n'
        

    # Write to file

    shortDate = datetime.today().strftime('%Y-%m-%d')
    f = open(id + "_images_before.txt", "w")
    f.write('\n' + 'HI IMAGES' + '\n')
    f.write("Name,Resolution,FileSize (kB)\n")
    if len(listOfFiles):
        f.writelines(listOfFiles)


pathToImages = r'c:\\GIT\\Phoenix\\APP\\UserInterface\\Resources\\Genie\\StyleLibrary.Flatview\\Images\\InstrumentImages'
IdentifyImages("genie" ,pathToImages)

pathToOasisImages = r'c:\\GIT\\Phoenix\\APP\\UserInterface\\Resources\\Oasis\\StyleLibrary.Oasis\\Images\\InstrumentImages'
IdentifyImages("oasis" ,pathToOasisImages)