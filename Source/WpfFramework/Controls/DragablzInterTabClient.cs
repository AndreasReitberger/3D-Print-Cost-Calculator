﻿using Dragablz;
using System.Windows;

namespace WpfFramework.Controls
{
    public class DragablzInterTabClient : IInterTabClient
    {
        private readonly ApplicationViewManager.Name _applicationName;

        public DragablzInterTabClient(ApplicationViewManager.Name applicationName)
        {
            _applicationName = applicationName;
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
