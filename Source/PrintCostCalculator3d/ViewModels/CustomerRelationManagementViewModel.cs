using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    public class CustomerRelationManagementViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties

        ObservableCollection<Customer> _customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                if (_customers == value) return;
                if(!IsLoading)
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
        async void GoProAction()
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
            OnPropertyChanged(nameof(IsLicenseValid));
        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
