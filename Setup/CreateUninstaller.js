// CreateUninstaller.js
//
// Runs on installation, to create an uninstaller
// .cmd file in the application folder.  This makes it
// easy to uninstall. 
//
// Mon, 31 Aug 2009  05:13
//


    var fso, ts;
    var ForWriting= 2;
    fso = new ActiveXObject("Scripting.FileSystemObject");

    var parameters = Session.Property("CustomActionData").split("|"); 
    var targetDir = parameters[0];
    var productCode = parameters[1];

    ts = fso.OpenTextFile(targetDir + "uninstall.cmd", ForWriting, true);


    ts.WriteLine("@echo off");
    ts.WriteLine("goto START");
    ts.WriteLine("=======================================================");
    ts.WriteLine(" Uninstall.cmd");
    ts.WriteBlankLines(1);
    ts.WriteLine(" This is part of DotNetZip.");
    ts.WriteBlankLines(1);
    ts.WriteLine(" Run this to uninstall the DotNetZip Utilities and Tools");
    ts.WriteBlankLines(1);
    ts.WriteLine("=======================================================");
    ts.WriteBlankLines(1);
    ts.WriteLine(":START");
    ts.WriteLine("@REM The uuid is the 'ProductCode' in the Visual Studio setup project");
    ts.WriteLine("@REM for the DotNetZip Utilities v1.9.1.x");
    ts.WriteLine("%windir%\\system32\\msiexec /x " + productCode);
    ts.WriteBlankLines(1);
    ts.Close();


// all done - try to delete myself.
try 
{
    var scriptName = targetDir + "createUninstaller.js";
    if (fso.FileExists(scriptName))
    {
        fso.DeleteFile(scriptName);
    }
}
catch (e2)
{
}



