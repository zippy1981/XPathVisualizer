@echo off
goto START

-------------------------------------------------------
 MakeZips.cmd

 Makes the zips and msi for XPath Visualizer

 Thu, 17 Sep 2009  13:56

-------------------------------------------------------


:START
setlocal

set zipit=c:\dinoch\bin\zipit.exe
set stamp=%DATE% %TIME%
set stamp=%stamp:/=-%
set stamp=%stamp: =-%
set stamp=%stamp::=%

:: get the version
for /f "delims==" %%I in ('type Tool\Properties\AssemblyInfo.cs ^| c:\utils\grep AssemblyVersion ^| c:\utils\sed -e "s/^.*(.\(.*\).).*/\1/"') do set longversion=%%I

set version=%longversion:~0,3%
echo version is %version%

set rdir=%cd%\releases\v%longversion%

echo making release dir %rdir%
if exist %rdir% (
  echo Error: Release dir already exists.
  goto ALL_DONE
)
mkdir %rdir%

echo built on %stamp% > %rdir%\BuiltOn-%stamp%.txt

set config=Debug
call :MakeMsiForConfig

set config=Release
call :MakeMsiForConfig
copy Setup\%config%\XPathVisualizer-v1.1.msi %rdir%

call :MakeBinZip
call :MakeSourceZip

goto ALL_DONE


-------------------------------------------------------
:MakeMsiForConfig
  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Building the project, config = %config%...
  echo.

c:\.net3.5\msbuild.exe XPathVisualizer.sln /p:Configuration=%config%


  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the MSI...
  echo.

  c:\vs2008\Common7\ide\devenv.exe XpathVisualizer.sln /build %config% /project "Setup"
  c:\dinoch\dev\dotnet\AwaitFile -v -t 50 Setup\%config%\XPathVisualizer-v1.1.msi

goto :EOF
-------------------------------------------------------




-------------------------------------------------------
:MakeBinZip

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the Bin zip...
  echo.

set binzip=%rdir%\XpathVisualizer-v%version%-bin.zip
%zipit%  %binzip%  -s Readme.txt "This is the binary distribution for Ionic's XPathVisualizer v1.1. Packed %stamp%."  -D MergedTool\bin\Release  XPathVisualizer.exe

  goto :EOF
-------------------------------------------------------


-------------------------------------------------------
:MakeSourceZip

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the source zip...
  echo.

cd ..
set srczip=%rdir%\XpathVisualizer-v1.1-src.zip
%zipit%  %srczip%  -s Readme.txt "This is the source distribution for Ionic's XPathVisualizer v1.1. Packed %stamp%."  -r+  -D XPathVisualizer  "(name != *.vssscc) and (name != *.*~) and (name != *.cache) and (name != *\Debug\*.*) and (name != *\Release\*.*) and (name != *\obj\*.*) and (name != *\bin\*.*) and (name != #*.*#) and (name != *.vspscc) and (name != *.suo) and (name != Makezips.cmd) and (name != *.vsp) and (name != *.psess) and (name != *\releases\*)"

cd XPathVisualizer

  goto :EOF
-------------------------------------------------------


:ALL_DONE

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo done.
  echo.

endlocal
