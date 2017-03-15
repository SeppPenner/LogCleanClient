LogCleanClient
====================================

LogCleanClient is an executable to clean log files (or any files depending on configuration).
The executable was written and tested in .Net 4.6.2.

[![Build status](https://ci.appveyor.com/api/projects/status/qx50j5ng4t2ngyt3?svg=true)](https://ci.appveyor.com/project/SeppPenner/bmirechner)


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
![Screenshot from the executable german](https://github.com/SeppPenner/BMIRechner/blob/master/Screenshot_DE.PNG "Screenshot from the executable german")

## Screenshot from the executable english:
![Screenshot from the executable english](https://github.com/SeppPenner/BMIRechner/blob/master/Screenshot_EN.PNG "Screenshot from the executable english")

Change history
--------------

* **Version 1.0.0.2 (2017-03-15)** : Switched to .Net to 4.6.2. Added multilanguage support.
* **Version 1.0.0.1 (2017-03-15)** : Refactored code.
* **Version 1.0.0.0 (2017-03-15)** : 1.0 release.
