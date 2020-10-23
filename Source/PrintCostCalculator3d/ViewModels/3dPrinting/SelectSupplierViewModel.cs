using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    class SelectSupplierViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            private set
            {
                if (value == _suppliers)
                    return;

                _suppliers = value;
                OnPropertyChanged();
            }
        }

        private Supplier _SelectedSupplier;
        public Supplier SelectedSupplier
        {
            get => _SelectedSupplier;
            set
            {
                if (value == _SelectedSupplier)
                    return;

                _SelectedSupplier = value;
                OnPropertyChanged();
            }
        }

        private IList _SelectedSuppliers = new ArrayList();
        public IList SelectedSuppliers
        {
            get => _SelectedSuppliers;
            set
            {
                if (value == _SelectedSuppliers)
                    return;

                _SelectedSuppliers = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public SelectSupplierViewModel(Action<SelectSupplierViewModel> saveCommand, Action<SelectSupplierViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            LoadSettings();
        }
        public SelectSupplierViewModel(Action<SelectSupplierViewModel> saveCommand, Action<SelectSupplierViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            this._dialogCoordinator = dialogCoordinator;

            LoadSettings();
        }

        private void LoadSettings()
        {
            Suppliers = SettingsManager.Current.Suppliers;
        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
