# -*- coding: utf-8 -*-
import re
import requests
import shutil
import os
import time
import pathlib

# Author: ZeroFly 杰洛飞
# comment: the following script can download resources automatically from genshine web server
# First download index.js and vendor.js in the folder where is this script locate
# Second replace following url and name
# Third run the script, you are good to go
#e.g. for this url
#https://webstatic.mihoyo.com/ys/event/e20220928review_data/index.html

webURL = "https://webstatic.mihoyo.com/ys/event/e20220928review_data/"
indexJSName = 'index_435bc0bfde917c016047.js'
vendorJSName = 'vendors_9f54804aa85053794de9.js'

totalFileNameList = []

#Analyze index js
with open(indexJSName,encoding='utf-8') as f:
    lines = f.readlines()

    for line in lines:
        indexList = [m.start() for m in re.finditer('e.exports=A.p', line)]
        if len(indexList) > 0:
            for start in indexList:
                url = webURL
                startIndex = start + 15 # find the start index of resource string
                while line[startIndex] != "\"":
                    url+=line[startIndex]
                    startIndex += 1
                fileName = os.path.basename(url)
                totalFileNameList.append(fileName)

                # r = requests.get(url, auth=('usrname', 'password'), verify=False,stream=True)
                # r.raw.decode_content = True
                # with open(fileName, 'wb') as f:
                #         shutil.copyfileobj(r.raw, f)
                # time.sleep(0.5)

currentFolderFileNames = next(os.walk("."))[2]

for name in currentFolderFileNames:
    if name.endswith(".mp3") or  name.endswith(".atlas.txt"):
        continue

    splitList = name.split('.')
    if len(splitList) > 2 :
        newName = splitList[0] + "." + splitList[-1]
        os.rename(name,newName)

# Analyze vendors js
with open(vendorJSName,encoding='utf-8') as f:
    lines = f.readlines()

    for line in lines:
        indexList = [m.start() for m in re.finditer('e.exports="', line)]
        if len(indexList) > 0:
            for start in indexList:
                url = ""
                startIndex = start + 11 # find the start index of resource string
                while line[startIndex] != "\"":
                    url+=line[startIndex]
                    startIndex += 1

                firstN = url.find("\\n")

                if firstN != -1:
                    fileName = url[0:firstN]

                    if ".png" in fileName :
                        pngIndex = fileName.find(".png")
                        targetName = fileName[0:pngIndex]
                        pathlib.Path(targetName).mkdir(parents=True, exist_ok=True) 
                        url = url.replace("\\n","\n")
                        atlasFile = open(targetName+".atlas.txt", "w",encoding="utf-8")
                        atlasFile.write(url)
                        atlasFile.close()
                        atlasName = targetName+".atlas.txt"
                        pngName = targetName+".png"
                        jsonName = targetName+".json"

                        try:
                            if os.path.exists(atlasName) :
                                shutil.move(atlasName, targetName+"/")
                            if os.path.exists(pngName) :
                                shutil.move(pngName, targetName+"/")
                            if os.path.exists(jsonName) :
                                jsonVersionCorrection = []
                                with open(jsonName,encoding='utf-8') as jsonF:
                                    jsonLines = f.readlines()
                                    for jsonLine in jsonLines:
                                        jsonAfter = jsonLine.replace("4.0-from-","")
                                        jsonVersionCorrection.append(jsonAfter)
                                    jsonF.close()
                                jsonFile = open(jsonName, "w",encoding="utf-8")
                                for lineAfter in jsonVersionCorrection:
                                    jsonFile.write(lineAfter)
                                jsonFile.close()

                                shutil.move(jsonName, targetName+"/")
                            
                        except Exception as e:
                            continue
                        #print(targetName)

                # r = requests.get(url, auth=('usrname', 'password'), verify=False,stream=True)
                # r.raw.decode_content = True
                # with open(fileName, 'wb') as f:
                #         shutil.copyfileobj(r.raw, f)
                # time.sleep(0.5)

for fName in totalFileNameList:
    if ".png" in fName :
        splitList = fName.split('.')
        if len(splitList) > 2 :
            fileName = splitList[0] + "." + splitList[-1]
        pngIndex = fileName.find(".png")
        targetName = fileName[0:pngIndex]
        jsonName = targetName+".json"
        print(targetName+"/"+jsonName)
        if os.path.exists(targetName+"/"+jsonName) :
            jsonVersionCorrection = []
            with open(targetName+"/"+jsonName,encoding='utf-8') as jsonF:
                jsonLines = jsonF.readlines()
                for jsonLine in jsonLines:
                    jsonAfter = jsonLine.replace("4.0-from-","")
                    jsonVersionCorrection.append(jsonAfter)
                jsonF.close()

            jsonFile = open(targetName+"/"+jsonName, "w",encoding="utf-8")
            for lineAfter in jsonVersionCorrection:
                jsonFile.write(lineAfter)
            jsonFile.close()
        