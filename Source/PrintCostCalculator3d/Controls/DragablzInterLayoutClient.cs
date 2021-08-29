using Dragablz;
using System;
using System.Windows;
using System.Windows.Data;

namespace PrintCostCalculator3d.Controls
{
    public class DragablzInterLayoutClient : IInterLayoutClient
    {
        //readonly ApplicationName _applicationName;
        readonly string _applicationName;

        public DragablzInterLayoutClient(string applicationName)
        {
            _applicationName = applicationName;
        }
        public DragablzInterLayoutClient(ApplicationName applicationName)
        {
            _applicationName = ApplicationViewManager.GetTranslatedNameByName(applicationName);
        }

        public INewTabHost<UIElement> GetNewHost(object partition, TabablzControl source)
        {
            var tabablzControl = new TabablzControl { DataContext = source.DataContext };

            Clone(source, tabablzControl);

            if (source.InterTabController == null)
                throw new InvalidOperationException("Source tab does not have an InterTabCOntroller set.  Ensure this is set on initial, and subsequently generated tab controls.");

            var newInterTabController = new InterTabController
            {
                Partition = source.InterTabController.Partition
            };
            Clone(source.InterTabController, newInterTabController);
            tabablzControl.SetCurrentValue(TabablzControl.InterTabControllerProperty, newInterTabController);

            return new NewTabHost<UIElement>(tabablzControl, tabablzControl);
        }

        static void Clone(DependencyObject from, DependencyObject to)
        {
            var localValueEnumerator = from.GetLocalValueEnumerator();
            while (localValueEnumerator.MoveNext())
            {
                if (localValueEnumerator.Current.Property.ReadOnly ||
                    localValueEnumerator.Current.Value is FrameworkElement) continue;

                if (!(localValueEnumerator.Current.Value is BindingExpressionBase))
                    to.SetCurrentValue(localValueEnumerator.Current.Property, localValueEnumerator.Current.Value);
            }
        }
    }
}
