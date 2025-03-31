using Autodesk.Revit.UI;
using System;
using System.Reflection;

namespace AutoTagSheetViews
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "AutoTag";
            string panelName = "Sheet Tools";
            application.CreateRibbonTab(tabName);
            RibbonPanel panel = application.CreateRibbonPanel(tabName, panelName);

            string dllPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData buttonData = new PushButtonData(
                "AutoTag",
                "AutoTagViews",
                dllPath,
                "AutoTagSheetViews.AutoTagCommand"
            );

            panel.AddItem(buttonData);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}