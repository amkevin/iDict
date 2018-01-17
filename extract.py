import re
import urllib.request

strWord = "打"
strWordUrl = urllib.parse.quote(strWord)
response = urllib.request.urlopen('http://open.iciba.com/huaci/dict.php?word=' + strWordUrl)
pattern = re.compile(r'<[^>]+>', re.S)
regexStr = ".*?(<[\u4E00-\u9FA5]+>+)"

for line in response:
    strLine = line.decode('utf-8')
    strLine = strLine.strip()
    if strLine.startswith("dict.innerHTML"):
        strLine = strLine.replace('\\"', '"')
        strLine = strLine.replace(strWord, '', 1)
        strLine = strLine.replace("dict.innerHTML='", "")
        matchStr = re.match(regexStr, strLine)
        while matchStr:
            matchWord = matchStr.group(1)
            replaceWd = matchWord.replace('<', '[')
            replaceWd = replaceWd.replace('>', ']')
            strLine = strLine.replace(matchWord, replaceWd)
            matchStr = re.match(regexStr, strLine)
        # strLine = strLine.replace("<名>", "[名]")
        # strLine = strLine.replace("<量>", "[量]")
        # strLine = strLine.replace("<动>", "[动]")
        # strLine = strLine.replace("<介>", "[介]")
        # strLine = strLine.replace("<形>", "[形]")
        # strLine = strLine.replace("<代>", "[代]")
        # strLine = strLine.replace("<副>", "[副]")
        strLine = strLine.replace("生词本</a>", "")
        strLine = strLine.replace("详细释义</a>", "")
        strLine = strLine.replace("';", "")
        strLine = strLine.replace("</a>", "~^~")
        strLine = strLine.replace("</p>", "~^~")
        strLine = strLine.replace("~^~；", "；")
        strLine = strLine.strip()
        strLine = pattern.sub('', strLine)
        strLine = ''.join(strLine.strip().split())
        strLine = strLine[0:len(strLine) - 3]
        print(strLine)
