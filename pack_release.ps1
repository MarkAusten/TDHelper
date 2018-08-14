$datetime = Get-Date -f MMddyy
$binpath = "c:\Development\Published\TDGUI"
$outfiles = "$binpath\storage\TDHelper-src-$datetime.zip"
$7zpath = "C:\Program Files\7-Zip\7z.exe"
$scriptPath = "c:\Development\TDHelper"

# don't change this START
$srcpath = "$scriptPath\TradeDangerousGUI"
$srcpathb = "$scriptPath\TradeDangerousGUI\bin\Release"
$packpath = "$binpath\pack_temp"
$assemblyPath = "$srcpathb\TDHelper.exe"
$callFileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$assemblyPath").FileVersion
$outfileb = "$binpath\storage\TDHelper-v$callFileVersion.zip"
$binaries = Get-ChildItem "$srcpath\bin\Release\*.*"
# don't change this END

# only xcopy stuff if we've compiled and our output exists
if (($binaries.count -gt 0) -and (Test-Path $binpath))
{
    # pre-cleanup
    cmd /c del /q $outfileb
    cmd /c del /q $outfiles
    cmd /c rmdir /s /q $packpath

    # selectively delete directory contents
    Get-ChildItem -Path "$binpath\*.*" -exclude TDHelper.db | Remove-Item -force

    # preparation
    cmd /c mkdir $packpath
    cmd /c mkdir $packpath\source
    cmd /c mkdir $binpath\storage

    # xcopy latest release
    cmd /c xcopy /s /y $srcpathb\*.* $binpath
	Remove-Item -Path $binpath/app.publish -recurse -force
	
    cmd /c xcopy /s /y $srcpathb\*.* $packpath
	Remove-Item -Path $packpath/app.publish -recurse -force
	
    cmd /c xcopy /e /y $scriptPath\*.* $packpath\source\
    cmd /c copy /y $srcpath\Changelog.txt $binpath

    # xcopy misc files to pack temp
    cmd /c copy /y $srcpath\Changelog.txt $packpath
    cmd /c copy /y $srcpath\License.txt $packpath
    cmd /c copy /y $scriptPath\README.md $packpath

    # packing release archive
    cmd /c $7zpath u -r -tzip -mx9 $outfileb $packpath\*.* -xr!source
    cmd /c $7zpath u -r -tzip -mx9 $outfiles $packpath\source\*.* -xr!TradeDangerousGUI\bin -xr!TradeDangerousGUI\obj

    # clean up
    cmd /c rmdir /s /q $packpath

    cmd /c $binpath\TDHelper.exe /g "https://github.com/MarkAusten/TDHelper/releases/download/v$callFileVersion/TDHelper-v$callFileVersion.zip"
}
else
{
    Write-Host "Compiled binaries and/or the binary path: $binpath, does not exist!`r`nCorrect your `$binpath in this script or recompile!" -backgroundcolor yellow
}
