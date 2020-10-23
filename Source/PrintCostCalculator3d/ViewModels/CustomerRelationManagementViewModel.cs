using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.ViewModels
{
    public class CustomerRelationManagementViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading;
        #endregion

        #region Properties
        public bool isLicenseValid
        {
            get => false;
        }

        private ObservableCollection<Customer> _customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                if (_customers == value) return;
                if(!_isLoading)
                {

                }
                _customers = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Constructor
        public CustomerRelationManagementViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        #endregion


        #region ICommand & Actions

        public ICommand GoProCommand
        {
            get => new RelayCommand(p => GoProAction());
        }
        private async void GoProAction()
        {
            try
            {
                string uri = GlobalStaticConfiguration.goProUri;
                if (!string.IsNullOrEmpty(uri))
                {
                    var res = await this._dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogGoProHeadline,
                        Strings.DialogGoProContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Process.Start(uri);
                        logger.Info(string.Format(Strings.EventOpenUri, uri));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #endregion

        #region Methods
        public void OnViewVisible()
        {
            OnPropertyChanged(nameof(isLicenseValid));
        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
