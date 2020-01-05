using WpfFramework.Utilities;
using System;
using System.Windows.Input;
using log4net;
using WpfFramework.Resources.Localization;

namespace WpfFramework.ViewModels._3dPrinting
{
    class NewSupplierViewModel : ViewModelBase
    {
        #region Variables
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _debitorNumber = string.Empty;
        public string DebitorNumber
        {
            get => _debitorNumber;
            set
            {
                if (_debitorNumber != value)
                {
                    _debitorNumber = value;
                    OnPropertyChanged(nameof(DebitorNumber));
                }
            }
        }

        private string _shopUri = string.Empty;
        public string ShopUri
        {
            get => _shopUri;
            set
            {
                if (_shopUri != value)
                {
                    _shopUri = value;
                    OnPropertyChanged(nameof(ShopUri));
                }
            }
        }

        private bool _isActive = true;
        public bool isActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(isActive));
                }
            }
        }
        #endregion

        #region Constructor
        public NewSupplierViewModel(Action<NewSupplierViewModel> saveCommand, Action<NewSupplierViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
