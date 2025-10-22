using System;
using System.Diagnostics;
using log4net;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace PKHL.ProjectSweeper
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class AboutBoxCommand : IExternalCommand
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(AboutBoxCommand));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            PKHL.ProjectSweeper.AboutBox aboutBox = null;
            try
            {
                aboutBox = new AboutBox(System.Reflection.Assembly.GetExecutingAssembly());
                System.Windows.Interop.WindowInteropHelper x = new System.Windows.Interop.WindowInteropHelper(aboutBox);
                x.Owner = commandData.Application.MainWindowHandle;
                aboutBox.ShowDialog();
            }
            catch (Exception err)
            {
                _log.Error("About dialog", err);
                if (aboutBox != null)
                    aboutBox.Close();
                TaskDialog td = new TaskDialog(LocalizationProvider.GetLocalizedValue<string>("ErrorDialog_Title"));
                td.MainInstruction = Constants.GROUP_NAME + LocalizationProvider.GetLocalizedValue<string>("ErrorDialog_MainInst");
                td.MainContent = LocalizationProvider.GetLocalizedValue<string>("ErrorDialog_MainCont");
                td.ExpandedContent = err.ToString();
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                TaskDialogResult tdr = td.Show();
            }

            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ApplicationHelp : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ProjectSweeper._contextualHelp.HelpTopicUrl = @"Welcome.htm";
            ProjectSweeper._contextualHelp.Launch();
            return Result.Succeeded;
        }
    }

    public class AlwaysAvailableCheck : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication appdata, CategorySet selectCatagories)
        {
            return true;
        }
    }
}