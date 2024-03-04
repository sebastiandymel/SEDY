import os
import re
import PIL
from datetime import datetime
from PIL import Image
from glob import glob

def Resize(id: str, dirPath: str):
    listOfFiles = []
    result = [y for x in os.walk(dirPath) for y in glob(os.path.join(x[0], '*.png'))]
    resize_factor = 0.25 # 0.25 * 640 = 160px ==> Genie uses 160x108 size in InstrumentSelection and in Save&Exit screens.
    for r in result:
        imgPath = r.replace("\\\\", "\\")                    
        im = Image.open(imgPath)
        width, height = im.size
        dirPathNormalized = r.replace(dirPath, "")
        msg = f"{dirPathNormalized},{str(width)}x{str(height)},{str(round(os.stat(imgPath).st_size / 1024, 3))}\n"
        listOfFiles.append(msg)

        #RESIZE IMAGE        
        if width == 640:
            print ("resizing " + r)
            im = im.resize((int(resize_factor * width), int(resize_factor * height)), Image.LANCZOS)
            im.save(imgPath)
            im.close()

    print (*listOfFiles)

    # Write to log file
    shortDate = datetime.today().strftime('%Y-%m-%d')
    f = open(f"{shortDate}_{id}_images_before.csv", "w")
    f.write('\n' + 'HI IMAGES' + '\n')
    f.write("Name,Resolution,FileSize (kB)\n")
    if len(listOfFiles):
        f.writelines(listOfFiles)    
    listOfFiles.clear()
    for r in result:
        imgPath = r.replace("\\\\", "\\")                    
        im = Image.open(imgPath)
        width, height = im.size
        dirPathNormalized = r.replace(dirPath, "")
        msg = f"{dirPathNormalized},{str(width)}x{str(height)},{str(round(os.stat(imgPath).st_size / 1024, 3))}\n"
        listOfFiles.append(msg)
    f = open(f"{shortDate}_{id}_images_after.csv", "w")
    f.write('\n' + 'HI IMAGES' + '\n')
    f.write("Name,Resolution,FileSize (kB)\n")
    if len(listOfFiles):
        f.writelines(listOfFiles)

# RESIZE GENIE
pathToGenieImages = r'c:\\GIT\\Phoenix\\Integration\\Application\\Resources\\Genie\\StyleLibrary.Flatview\\Images\\InstrumentImages'
Resize("genie" ,pathToGenieImages)

# RESIZE OASIS
# pathToOasis = r'c:\\GIT\\Phoenix\\Integration\\Application\\Resources\\Oasis\\StyleLibrary.Oasis\\Images\\InstrumentImages'
# Resize("oasis" ,pathToOasis)