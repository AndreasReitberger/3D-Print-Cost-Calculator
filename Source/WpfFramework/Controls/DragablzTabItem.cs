﻿using WpfFramework.Utilities;
using System.Windows.Controls;

namespace WpfFramework.Controls
{
    public class DragablzTabItem : ViewModelBase
    {
        private string _header;
        public string Header
        {
            get => _header;
            set
            {
                if (value == _header)
                    return;

                _header = value;
                OnPropertyChanged();
            }
        }

        public UserControl View { get; set; }
        public int Id { get; set; }

        public DragablzTabItem(string header, UserControl view)
        {
            Header = header;
            View = view;         
        }

        public DragablzTabItem(string header, UserControl view, int id)
        {
            Header = header;
            View = view;
            Id = id;
        }
    }
}
