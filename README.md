Trade Dangerous Helper is an extremely helpful GUI by Jon Wrights for the powerful command-line utility tradedangerous by Oliver Smith (latest horizon branch version: https://bitbucket.org/bgol/tradedangerous/branch/horizon).
Sadly the last update of TDHelper is now over a year ago and due to changes in the netlog format the tool was not able to create the Pilot's log anymore. Jon Wright has kindly put his source code under the MIT License that allows me to modify and publish this code to ensure its functionality with the current Elite version.

Cmdr Olklei

2018.06.26
==========
Trade Dangerous Helper has been updated. The excellent maddavo plugin stopped working some months ago and CMDR Eyeonus wrote a new plugin that pulls data either from a server mainained by CMDR Tromador or from the EDDB server as a fallback. This new pugin, EDDBlink, allowed TD Helper to work again but not as an integrated system. I have, therefore, removed all the maddavo plugin calls and substituted the EDDBlink plugin call allowing TD Helkper to work again as intended. Tjere were a few other bugs that got squished in the process and the applicationis currently in beta testing.

Cmdr MarkAusten

Trade Dangerous Helper
======================
Copyright under the MIT License (C) 2015, Jon Wrights

## FAQ page is [HERE](https://bitbucket.org/WombatFromHell/trade-dangerous-helper/wiki) ##

What is this tool supposed to do?
=================================
I made this GUI frontend to help with common usage of the fantastically useful trade route plotter tool Trade Dangerous made by
[Oliver Smith](https://bitbucket.org/kfsone/tradedangerous) for the awesome space sim MMO [Elite: Dangerous](http://elitedangerous.com/).


Installation Instructions
=========================
First, you'll need to make sure you have at least [.NET 4.5 Framework](http://go.microsoft.com/fwlink/?LinkId=397674) or above installed.
You'll also need at least [Python 3.4](https://www.python.org/downloads/) available. As well as the very latest version of Trade Dangerous
from Oliver Smith's ([BitBucket GIT](https://bitbucket.org/kfsone/tradedangerous)).

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
I use Visual Studio 2013 for C#, however Visual Studio Express 2010/2013 works fine as well. The System.Data.SQLite.dll library
included in this repository is the only additional library required. To compile successfully you must add the System.Data.SQLite.dll
library as an additional reference in Solution Explorer if it doesn't already exist there.


Planned Changes
===============
* WIP: Better support for EDAPI importing (removal of the console requirement)
* WIP: A proper method of calculating rare commodity routes that integrates with the TreeView


Recent Changes
==============
v2.0.0.0-Beta
--------
* Fixed pilot's log (again) due to another NetLog change.
* Removed all Maddavo plugincalls.
* Added calls to the new EDDBlink plugin.
* Fixed a bug that threw an exception on saving settings.

v1.0.7.8
--------
* Fixed pilot's log for use with ED2.3

v1.0.7.7
--------
* Fixed pilot's log for use with horizon

v1.0.7.6
--------
* Fixed a bug in readNetLog() preventing quick-loading systems from the database on startup
* Fixed some bugs that caused index exceptions when deleting rows from the Pilot's Log
* Fixed the "C" button reordering the most recent systems in the recents dropdown
* Fixed the selection behavior of the top-left-corner selection button in the DataGridView
* We now prevent from updating the recent systems dropdown unless there are changes
* We now batch removal of rows from the Pilot's Log for a performance increase and stability improvement
* Lots of performance improvements when building or updating the Pilot's Log with systems

v1.0.7.5
--------
* The netLog path collection is now more resilient against errors and file timestamp shenanigans
* Fixed some more sorting bugs when loading the database and appending the most recent systems
* Fixed incorrect behavior in getMaddavoUpdates() when using "Force Prices" with the "Skip prices" checkbox
* Rewrite of readNetLog() and accompanying methods feeding the recent systems list-- this should alleviate many stubborn bugs

v1.0.7.4
--------
* Fixed a fatal index exception when deleting the last row in the Pilot's Log
* Fixed a bug in readNetLog() causing the recent systems updater to print duplicates of the recents array on startup
* Fixed some bugs in how readNetLog() sorts the recent systems array that resulted in improper ordering
* We now use the AppConfigLocal.xml to save our VerboseLogging setting, which should survive Elite Dangerous patches

v1.0.7.3
--------
* Added a Ctrl+Click handler to allow resetting the font in the Misc. Settings dialog
* Added a splash window when populating the database to let the user know we're working
* Added settings box for changing the font in TreeView to the Misc. Settings dialog
* Fixed TreeView unnecessarily making a sound when copying system/station names
* Fixed the TreeView cutting off the bottom of a list when resizing
* Fixed the Ctrl+Shift+C shortcut in TreeView grabbing the whole line instead of the System/Station
* Fixed a rare race condition in the recent systems updater that caused multiple updates to fire at once
* Fixed a bug that resulted in duplicates being inserted into the Pilot's Log when appending multiple systems
* Fixed some bugs in row insertion, now we generate a unique timestamp from the selected row
* Fixed the sort when inserting rows in Pilot's Log, entries shouldn't drop to the bottom anymore
* Fixed more bugs in the Pilot's Log and recent systems updater, they should no longer update unless they need to
* Improved performance when loading the pilot's log database by moving the work to a background thread
* Re-ordered the way database transactions are performed, should drastically increase insert/delete performance
* We now perform a database vacuum during potentially fragmenting procedures for ideal performance over time

v1.0.7.2
--------
* Fixed a bug in the circular text buffer that caused the output log to spaz out on large text dumps
* Fixed a bug that caused the recent systems list to fail while updating with new systems
* Fixed a bug that caused the Pilot's Log to constantly update unnecessarily

v1.0.7.1
--------
* Added a handler (Ctrl+C/Ctrl+Shift+C) to allow clipboard copying of the system or system/station name from nodes in the TreeView
* Added a context menu option for copying system names from the Pilot's Log to the src/dest boxes
* Added a Ctrl+Click handler to the config file selector to allow deleting non-default config files
* Fixed a bug that caused the Pilot's Log DataGridView to update unnecessarily
* Fixed a bug that caused the Pilot's Log to not retain the most recently visited entries in the list
* Fixed a Data Property Mismatch exception when switching config files from the config selection box
* Fixed the Padsize box not using the Padsize value from the config when loading alternate config files
* Fixed a bug in the worker delegate that was causing all price file imports to go to cleanUpdatedPricesFile() and fail
* Fixed a bug causing strange selection behavior in the Pilot's Log when selecting a cell after editing any other cell
* Fixed an exception when cancelling out of the Import button's path selection dialog when no .prices file is selected
* Fixed the Import button not asking for a path when the current path to the .prices file is blank
* We no longer switch to the Output pane when loading a config file
* The EDAPI import executed from the Import button will now open a console window for user input
* The Recent Systems dropdown in src/dest is now capped at the most recent 50 unique items to reduce clutter
* Rewrote the support code for the DataGrid--it should now be faster, safer, and lighter on resources
* Rewrote the recent systems list to integrate better with the Pilot's Log and to only read logs when necessary

v1.0.7.0
--------
NOTE: This release requires that you update to Trade Dangerous v7.2.0 or above to function correctly
* Added hyperlinks to the main window pointing to the FAQ and the Issue tracker to encourage people to report bugs
* Added a Pilot's Log tab with the previous systems visited in a timestamped format with notes
* Added a couple of context menu options to the Pilot's Log GridView, mainly for inserting custom rows (like timestamped notes)
* Added a config option ("CopySystemToClipboard") to allow copying unrecognized system names to clipboard automatically
* Added a button to the misc. settings dialog to allow for completely wiping all settings from the current config file
* Fixed path validation dialogs throwing exceptions when simply cancelling a dialog
* Fixed the "T" button in TreeView to properly remember state when re-entering the dialog
* Fixed "Notify on unknown system" not notifying when starting in an unknown system
* Fixed Routes value being wiped when switching between panels after the worker delegate has run
* "Notify on unrecognized system" option no longer steals window focus, flashes the TDHelper window, and has a new audio alarm
* We now allow the user to change the "Unknown System" alarm by putting an "unknown.wav" in the executable directory
* We now grab new systems automatically in the background every few seconds instead of requiring manual updates (the "C" button is optional)
* The Gear button on the Run options panel now opens a more detailed configuration window to give access to obscure settings from the config file
* Changed the behavior of the path validators to better support the Trade Dangerous Installer
* Changed the default behavior of the unrecognized system notifier, we no longer copy system names automatically--instead we print a message to the output log buffer by default
* Changed the behavior of the "Import" button: a normal click now uses EDAPI, Shift+Click imports a previous .prices file, and Ctrl+Click imports a custom .prices file

v1.0.6.7
--------
* The Import and Upload buttons now properly save/load their chosen file paths
* Fixed an exception when readNetLog() processes a netLog that contains no System names
* Fixed an OOR exception when updating a blank recent systems list with more than 1 marked stations

v1.0.6.6
--------
* Improved CPU usage when dumping large amounts of text to the output log (like during database updates)
* Fixed a bug that caused an OOR exception when setting the SelectedIndex after using the "C" button
* Fixed some more potential index and null exceptions when getting recent systems

v1.0.6.5
--------
* Added a cog button in the Run panel for customizing ExtraRunParams
* (Re-release) Fixed a null reference when getting recent systems from an empty Logs directory
* Fixed station/shipvendor information populating incorrectly for stations with the same name in different systems
* Fixed failing when adding VerboseLogging="1" to the AppConfig.xml in some situations
* Fixed a bug that resulted in loading/validating the default config multiple times on startup
* Fixed an issue with the Local checkbox resetting ladenLY to 1 in some situations
* Fixed the ShipVendor panel neglecting to update the ShipVendor panel when adding/removing
* Fixed the "C" button not pulling recent systems since ED 1.3
* The Ships Sold dropdown box should now behave like all other dropdowns when entered/clicked
* Jumps between hops are now included in the TreeView output when available
* We now clean the input when passing ships in the ShipVendor panel to the delegate
* We now parse all network logs for previous destinations and not just the most recent


Respectful mentions
===================
Thanks to Oliver Smith for his [Trade Dangerous](https://bitbucket.org/kfsone/tradedangerous) tool written in Python. Without which
this tool would not be possible.