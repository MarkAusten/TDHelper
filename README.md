## 2018.06.26

Trade Dangerous Helper has been updated. The excellent maddavo plugin stopped working some months ago and Cmdr eyeonus wrote a new plugin that pulls data either from a server mainained by Cmdr Tromador or from the EDDB server as a fallback. This new pugin, EDDBlink, allowed TD Helper to work again but not as an integrated system. I have, therefore, updated TD Helper, removing all the maddavo plugin calls and substituting the EDDBlink plugin call allowing TD Helper to work again as intended. There were a few other bugs that got squished in the process.

Cmdr Austen

## 2016.08.28
Trade Dangerous Helper is an extremely helpful GUI by Jon Wrights for the powerful command-line utility tradedangerous by Oliver Smith. Sadly the last update of TDHelper is now over a year ago and due to changes in the netlog format the tool was not able to create the Pilot's log anymore. Jon Wright has kindly put his source code under the MIT License that allows me to modify and publish this code to ensure its functionality with the current Elite version.

Cmdr Olklei

Trade Dangerous Helper
======================
Copyright under the MIT License (C) 2015, Jon Wrights

## FAQ page is [HERE](https://github.com/MarkAusten/TDHelper/wiki/Frequently-Asked-Questions) ##

What is this tool supposed to do?
=================================
I made this GUI frontend to help with common usage of the fantastically useful trade route plotter tool Trade Dangerous made by
[Oliver Smith](https://bitbucket.org/kfsone/tradedangerous) for the awesome space sim MMO [Elite: Dangerous](http://elitedangerous.com/).


Installation Instructions
=========================
First, you'll need to make sure you have at least [.NET 4.5 Framework](http://go.microsoft.com/fwlink/?LinkId=397674) or above installed.
You'll also need at least [Python 3.4](https://www.python.org/downloads/) available. As well as the very latest version of Trade Dangerous
from Eyeonus' ([GitHub GIT](https://github.com/eyeonus/Trade-Dangerous)).

Once you have those requirements:

* Unpack the binary archive you've downloaded from this repository's Download section into a directory of its own.
* Run the TDHelper.exe file, and it will ask you for paths to your Python, TradeDangerous, and Elite: Dangerous launcher directories
if it can't find them.
* There is no step three!

No settings are saved to the registry, and no setup program is required. To uninstall it, just delete the files you extracted from the
directory you put them in; no fuss no muss.


What features does it currently have?
=====================================
There is full support for most TradeDangerous commands. The interface itself is still in constant development. Suggestions for
improvements and features are always welcome.


What do I need to compile this project from source?
===================================================
I use Visual Studio 2017 Community Version for C#, however Visual Studio 2017 works fine as well. The System.Data.SQLite.dll, Newtonsoft.Json.dll and SharpConfig.dll libraries included in this repository are the only additional libraries required.

Respectful mentions
===================
Thanks to Oliver Smith for his [Trade Dangerous](https://bitbucket.org/kfsone/tradedangerous) tool written in Python. Without which
this tool would not be possible.

Thanks also to Cmdr Olklei for his modifications to the original.
