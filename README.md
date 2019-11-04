LogCleanClient
====================================

LogCleanClient is an executable to clean log files (or any files depending on configuration).
The executable was written and tested in .Net 4.8.

[![Build status](https://ci.appveyor.com/api/projects/status/ebj1jxkl6a677uqx?svg=true)](https://ci.appveyor.com/project/SeppPenner/logcleanclient)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/LogCleanClient.svg)](https://github.com/SeppPenner/LogCleanClient/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/LogCleanClient.svg)](https://github.com/SeppPenner/LogCleanClient/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/LogCleanClient.svg)](https://github.com/SeppPenner/LogCleanClient/stargazers)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/SeppPenner/LogCleanClient/master/License.txt)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/LogCleanClient/badge.svg)](https://snyk.io/test/github/SeppPenner/LogCleanClient)


## How does the configuration need to look like:
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


## Screenshot from the executable german:
![Screenshot from the executable german](https://github.com/SeppPenner/LogCleanClient/blob/master/Screenshot_DE.PNG "Screenshot from the executable german")

## Screenshot from the executable english:
![Screenshot from the executable english](https://github.com/SeppPenner/LogCleanClient/blob/master/Screenshot_EN.PNG "Screenshot from the executable english")

Change history
--------------

* **Version 1.0.1.0 (2019-10-27)** : Updated nuget packages, added GitVersionTask.
* **Version 1.0.0.6 (2019-05-07)** : Updated .Net version to 4.8.
* **Version 1.0.0.5 (2017-03-24)** : Updated Languages.dll to version 1.0.0.4.
* **Version 1.0.0.4 (2017-03-21)** : Updated Languages.dll.
* **Version 1.0.0.3 (2017-03-15)** : Bugfix for config import.
* **Version 1.0.0.2 (2017-03-15)** : Switched to .Net to 4.6.2. Added multilanguage support.
* **Version 1.0.0.1 (2017-03-15)** : Refactored code.
* **Version 1.0.0.0 (2017-03-15)** : 1.0 release.
