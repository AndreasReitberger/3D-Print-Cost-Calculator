using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    class DonateDialogViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<DonationLevel> _levels = new ObservableCollection<DonationLevel>()
        {
            new DonationLevel() {Name = "Coffee", Amount = 2.50m},
            new DonationLevel() {Name = "Breakfast", Amount = 5.00m},
            new DonationLevel() {Name = "Lunch", Amount = 15.00m},
            new DonationLevel() {Name = "Buy some cool stuff", Amount = 50.00m},
            new DonationLevel() {Name = "Do something cool", Amount = 100.00m},
        };
        public ObservableCollection<DonationLevel> DonationLevels
        {
            get => _levels;
            set
            {
                if(_levels != value)
                {
                    _levels = value;
                    OnPropertyChanged(nameof(DonationLevels));
                }
            }
        }

        private string _mail = string.Empty;
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged(nameof(Mail));
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

        private string _Uri = "https://www.paypal.me/andreasreitberger/{0}";
        public string Uri
        {
            get => _Uri;
            set
            {
                if (_Uri != value)
                {
                    _Uri = value;
                    OnPropertyChanged(nameof(Uri));
                }
            }
        }

        private decimal _amount = 5;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }
        #endregion

        #region Constructor
        public DonateDialogViewModel(Action<DonateDialogViewModel> saveCommand, Action<DonateDialogViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        
        public ICommand SelectedDonationLevelChangedCommand
        {
            get => new RelayCommand(p => SelectedDonationLevelChangedAction(p));
        }
        private void SelectedDonationLevelChangedAction(object p)
        {
            DonationLevel dl = p as DonationLevel;
            if(p != null)
            {
                Amount = dl.Amount;
            }
        }

        public ICommand DonateCommand
        {
            get => new RelayCommand(p => DonateAction());
        }
        private void DonateAction()
        {
            string uri = string.Format(Uri, Amount);
            Process.Start(uri);
            SaveCommand.Execute(null);
        }
        #endregion
    }

    public class DonationLevel
    {
        #region Properties
        public string Name
        { get; set; }
        public decimal Amount
        { get; set; }
        #endregion

        #region Constructors
        public DonationLevel() { }
        #endregion

    }
}
