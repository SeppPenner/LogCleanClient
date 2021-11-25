LogCleanClient
====================================

LogCleanClient is an executable to clean log files (or any files depending on configuration).

[![Build status](https://ci.appveyor.com/api/projects/status/ebj1jxkl6a677uqx?svg=true)](https://ci.appveyor.com/project/SeppPenner/logcleanclient)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/LogCleanClient.svg)](https://github.com/SeppPenner/LogCleanClient/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/LogCleanClient.svg)](https://github.com/SeppPenner/LogCleanClient/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/LogCleanClient.svg)](https://github.com/SeppPenner/LogCleanClient/stargazers)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/SeppPenner/LogCleanClient/master/License.txt)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/LogCleanClient/badge.svg)](https://snyk.io/test/github/SeppPenner/LogCleanClient)


## How does the configuration need to look like
```xml
<?xml version="1.0" encoding="utf-8"?>
<Config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    <LogModels>
        <LogModel>
            <FileFilter>.log|.txt</FileFilter>
            <LogFolder>C:\Test</LogFolder>
        </LogModel>
        <LogModel>
            <FileFilter>.log|.txt</FileFilter>
            <LogFolder>C:\Test2</LogFolder>
        </LogModel>
    </LogModels>
</Config>
```


## Screenshot from the executable German
![Screenshot from the executable German](https://github.com/SeppPenner/LogCleanClient/blob/master/Screenshot_DE.PNG "Screenshot from the executable German")

## Screenshot from the executable English
![Screenshot from the executable English](https://github.com/SeppPenner/LogCleanClient/blob/master/Screenshot_EN.PNG "Screenshot from the executable English")

Change history
--------------

See the [Changelog](https://github.com/SeppPenner/LogCleanClient/blob/master/Changelog.md).