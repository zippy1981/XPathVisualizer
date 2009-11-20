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

set config=Debug
call :MakeZipsForConfig

set config=Release
call :MakeZipsForConfig

call :MakeBinZip

call :MakeSourceZip

goto ALL_DONE


-------------------------------------------------------
:MakeZipsForConfig
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

set binzip=XpathVisualizer-v1.1-bin.zip
if EXIST  ..\%binzip% del ..\%binzip%
%zipit%  ..\%binzip%  -s Readme.txt "This is the binary distribution for Ionic's XPathVisualizer v1.1. Packed %stamp%."  -D Tool\bin\Release  XPathVisualizer.exe


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
set srczip=XpathVisualizer-v1.1-src.zip
if EXIST  %srczip% del %srczip%
%zipit%  %srczip%  -s Readme.txt "This is the source distribution for Ionic's XPathVisualizer v1.1. Packed %stamp%."  -r+  -D XPathVisualizer  "(name != *.vssscc) and (name != *.*~) and (name != *.cache) and (name != *\Debug\*.*) and (name != *\Release\*.*) and (name != *\obj\*.*) and (name != *\bin\*.*) and (name != #*.*#) and (name != *.vspscc) and (name != *.suo) and (name != Makezips.cmd) and (name != *.vsp) and (name != *.psess)" 

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
