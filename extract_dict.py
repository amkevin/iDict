# -*- coding:utf-8 -*-
import re
import time
import codecs
import socket
import urllib.request

socket.setdefaulttimeout(60)


def grabDict(strWord):
    strWord = strWord.strip()
    strWordUrl = urllib.parse.quote(strWord)
    boolRetry = True
    while boolRetry:
        try:
            response = urllib.request.urlopen('http://open.iciba.com/huaci/dict.php?word=' + strWordUrl)
        except:
            print("time out happen, wait 5 seconds...")
            time.sleep(5)
        else:
            boolRetry = False
    pattern = re.compile(r'<[^>]+>', re.S)
    regexStr = ".*?(<[\u4E00-\u9FA5]+>+)"
    regexBlk = "\\+s"
    for line in response:
        strLine = line.decode('utf-8')
        strLine = strLine.strip()
        if strLine.startswith("dict.innerHTML"):
            strLine = strLine.replace('\\"', '"')
            strLine = strLine.replace('\\\'s', "'s")
            # strLine = strLine.replace(strWord, '', 1)
            strLine = strLine.replace("dict.innerHTML='", "")
            matchStr = re.match(regexStr, strLine)
            while matchStr:
                matchWord = matchStr.group(1)
                replaceWd = matchWord.replace('<', '〈')
                replaceWd = replaceWd.replace('>', '〉')
                strLine = strLine.replace(matchWord, replaceWd)
                matchStr = re.match(regexStr, strLine)
            strLine = strLine.replace("生词本</a>", "")
            strLine = strLine.replace("详细释义</a>", "")
            strLine = strLine.replace("';", "")
            strLine = strLine.replace("]</strong>", "]~^~")
            strLine = strLine.replace("</p>", "~^~")
            strLine = strLine.replace("~^~；", "；")
            strLine = strLine.strip()
            strLine = pattern.sub('', strLine)
            # strLine = ''.join(strLine.strip().split())
            strLine = strLine.replace("\t", "").strip()
            if strLine[len(strLine) - 3: len(strLine)] == '~^~':
                strLine = strLine[0:len(strLine) - 3]
            if strLine[0:3] == '~^~':
                strLine = strLine[3:len(strLine)]
            strLine = strLine.replace('〈', '<')
            strLine = strLine.replace('〉', '>')
            strLineTmp = ""
            for strSec in strLine.strip().split("~^~"):
                strLineTmp += strSec.strip() + "~^~"
            if strLineTmp[len(strLineTmp) - 3: len(strLineTmp)] == '~^~':
                strLine = strLineTmp[0:len(strLineTmp) - 3]
            # strLine = strLine.replace("                    ", "")
            strLine = re.sub(r'\s+', ' ', strLine)
            if strLine.startswith(strWord):
                strLine = strLine.replace(strWord, '', 1)
            strLine = strLine.strip()
            return strLine


# strDict = grabDict("斯")
# print(strDict)
file_in = open('iDict.bin')
idx = 0
for linef in file_in:
    strLineWord = linef.split('\t')[0].strip()
    strUrlWord = grabDict(strLineWord)
    if strUrlWord == "以上为百度翻译结果":
        strLineFull = strLineWord + '\t' + linef.split('\t')[1].strip() + '\r\n'
    else:
        strLineFull = strLineWord + '\t' + strUrlWord + '\r\n'
    file_ot = codecs.open('iDict_new.bin', 'a', 'utf-8')
    file_ot.write(strLineFull)
    file_ot.close()
    idx += 1
    print('we have grabbed ' + str(idx) + ' words.')
    time.sleep(0.1)
file_in.close()
