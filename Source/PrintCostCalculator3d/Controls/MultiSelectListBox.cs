using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Controls
{
    public class MultiSelectListBox : ListBox
    {
        public MultiSelectListBox()
        {
            SelectionChanged -= DataGridMultiItemSelect_SelectionChanged;
            SelectionChanged += DataGridMultiItemSelect_SelectionChanged;
        }

        private void DataGridMultiItemSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItemsList = SelectedItems;
        }

        public IList SelectedItemsList
        {
            get => (IList)GetValue(SelectedItemsListProperty);
            set => SetValue(SelectedItemsListProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", 
            typeof(IList), 
            typeof(MultiSelectListBox), 
            new PropertyMetadata(new ArrayList(), new PropertyChangedCallback(OnSelectionChanged)));
        public static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectListBox clb = d as MultiSelectListBox;
            var selectedItems = e.NewValue as IList;
            if (selectedItems != null)
            {
                clb.SetSelectedItems(selectedItems);
            }
        }
    }
}
