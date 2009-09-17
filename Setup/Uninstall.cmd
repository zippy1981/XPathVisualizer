@echo off
goto START
=======================================================
 Uninstall.cmd

 This is part of the XPath Visualizer

 Run this to uninstall the product.

 Thu, 17 Sep 2009  12:20

=======================================================

:START
@REM The uuid is the "ProductCode" in the Visual Studio setup project

%windir%\system32\msiexec /x {B03A4C9E-7387-40A5-A3F5-B37E8FBE0281}
