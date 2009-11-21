// PrettifyMsi.js
// 
// Performs a post-build fixup of an msi to prettify the text in the wizard.
//
// Sat, 21 Nov 2009  12:31
// 
// ==================================================================


function UpdateText(formName, text, textName)
{
    sql = "UPDATE `Control` SET `Control`.`Text` = '" + text + "' "  +
        "WHERE `Control`.`Dialog_`='" + formName + "' AND `Control`.`Control`='" + textName + "'"
    view = database.OpenView(sql);
    view.Execute();
    view.Close();
}

// Constant values from Windows Installer
var msiOpenDatabaseModeTransact = 1;

var msiViewModifyInsert         = 1;
var msiViewModifyUpdate         = 2;
var msiViewModifyAssign         = 3;
var msiViewModifyReplace        = 4;
var msiViewModifyDelete         = 6;


if (WScript.Arguments.Length != 1)
{
    WScript.StdErr.WriteLine(WScript.ScriptName + ": Updates an MSI to move the custom action in sequence");
    WScript.StdErr.WriteLine("Usage: ");
    WScript.StdErr.WriteLine("  " + WScript.ScriptName + " <file>");
    WScript.Quit(1);
}

var filespec = WScript.Arguments(0);
WScript.Echo(WScript.ScriptName + " " + filespec);
var WshShell = new ActiveXObject("WScript.Shell");

var database = null;
try
{
    var installer = new ActiveXObject("WindowsInstaller.Installer");
    database = installer.OpenDatabase(filespec, msiOpenDatabaseModeTransact);
    // this will fail if Orca.exe has the same MSI already opened
}
catch (e1)
{
    WScript.Echo("Error: " + e1.message);
    for (var x in e1)
        WScript.Echo("e[" + x + "] = " + e1[x]);
}

if (database==null) 
{
    WScript.Quit(1);
}

var sql;
var view;
var record;

try
{
    WScript.Echo("Beautifying the text in the setup wizard...");

    // The titles of the first screen of the install Wizard. 
    // The titles of the first screen of the install Wizard. 
    UpdateText("WelcomeForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\n[ProductName]",
               "BannerText");
    UpdateText("ResumeForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\n[ProductName]",
               "BannerText");
    UpdateText("MaintenanceForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\n[ProductName]",
               "BannerText");

    UpdateText("AdminWelcomeForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Network Setup Wizard for\r\n[ProductName]",
               "BannerText");
    UpdateText("AdminResumeForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\n[ProductName]",
               "BannerText");
    UpdateText("AdminMaintenanceForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\n[ProductName]",
               "BannerText");


    // The confirmation form
    UpdateText("ConfirmInstallForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Ready to install...",
               "BannerText");


    // The user exit text 
    UpdateText("UserExitForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}You interrupted the installation...",
               "BannerText");

    // EULA
    UpdateText("EulaForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Here`s the License for [ProductName].\r\nYou have to accept it to proceed.",
              "BannerText");

    UpdateText("EulaForm", 
               "{\\VSI_MS_Sans_Serif13.0_0_0}Please take a moment to read the license agreement now. If you accept the terms below, click \"I Accept\", then \"Next\". Otherwise click \"Cancel\".",
               "BodyText");


    // update the text for the radio buttons
    sql = "UPDATE `RadioButton` SET `RadioButton`.`Text` = '{\\VSI_MS_Sans_Serif13.0_0_0}I &Accept' "  +
        "WHERE `RadioButton`.`Property`='EulaForm_Property'  AND " +
        " `RadioButton`.`Value`='Yes'";
    view = database.OpenView(sql);
    view.Execute();
    view.Close();

    sql = "UPDATE `RadioButton` SET `RadioButton`.`Text` = '{\\VSI_MS_Sans_Serif13.0_0_0}I &Do Not Accept' "  +
        "WHERE `RadioButton`.`Property`='EulaForm_Property'  AND " +
        " `RadioButton`.`Value`='No'";
    view = database.OpenView(sql);
    view.Execute();
    view.Close();


    // Folder Selection
    UpdateText("FolderForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Select the Installation Folder...",
               "BannerText");


    // Progress
    UpdateText("ProgressForm",
               "{\\VSI_MS_Sans_Serif13.0_0_0}This will take just a moment...",
               "InstalledBody");

    UpdateText("ProgressForm",
               "{\\VSI_MS_Sans_Serif13.0_0_0}This will take just a moment...",
               "RemoveedBody");

    // Complete  
    UpdateText("FinishedForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}The setup for XPathVisualizer is complete.",
               "BannerText");

    database.Commit();

    WScript.Echo("done.");

}
catch(e)
{
    WScript.StdErr.WriteLine("Exception");
    for (var x in e)
        WScript.StdErr.WriteLine("e[" + x + "] = " + e[x]);
    WScript.Quit(1);
}

