using Dragablz;
using System.Windows;

namespace PrintCostCalculator3d.Controls
{
    public class DragablzInterTabClient : IInterTabClient
    {
        //private readonly ApplicationName _applicationName;
        private readonly string _applicationName;

        public DragablzInterTabClient(string applicationName)
        {
            _applicationName = applicationName;
        }
        public DragablzInterTabClient(ApplicationName applicationName)
        {
            _applicationName = ApplicationViewManager.GetTranslatedNameByName(applicationName);
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var dragablzTabHostWindow = new DragablzTabHostWindow(_applicationName);
            return new NewTabHost<DragablzTabHostWindow>(dragablzTabHostWindow, dragablzTabHostWindow.TabsContainer);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return window is MainWindow ? TabEmptiedResponse.DoNothing : TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
