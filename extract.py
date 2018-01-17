import re
import sys
import urllib.request

strWord = "美丽"
strWord = urllib.parse.quote(strWord)
response = urllib.request.urlopen('http://open.iciba.com/huaci/dict.php?word=' + strWord)
pattern = re.compile(r'<[^>]+>', re.S)

for line in response:
    strLine = line.decode('utf-8')
    strLine = strLine.strip()
    if strLine.startswith("dict.innerHTML"):
        strLine = strLine.replace('\\"', '"')
        strLine = strLine.replace(strWord, '', 1)
        strLine = strLine.replace("dict.innerHTML='", "")
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
