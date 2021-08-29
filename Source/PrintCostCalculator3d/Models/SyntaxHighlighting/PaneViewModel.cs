// https://www.codeproject.com/Articles/719143/AvalonDock-Tutorial-Part-Load-Save-Layout

using System.Windows.Media;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.Models.SyntaxHighlighting
{
    public class PaneViewModel : ViewModelBase
    {
        public PaneViewModel()
        { }


        #region Title

        string _title = null;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public ImageSource IconSource
        {
            get;
            protected set;
        }

        #region ContentId

        string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region IsSelected

        bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region IsActive

        bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion


    }
}
