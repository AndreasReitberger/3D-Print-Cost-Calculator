using PrintCostCalculator3d.Utilities;
using System;
using System.Security;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    public class CredentialsMasterPasswordViewModel : ViewModelBase
    {
        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        SecureString _password = new SecureString();
        public SecureString Password
        {
            get => _password;
            set
            {
                if (value == _password)
                    return;

                _password = value;

                ValidatePassword();

                OnPropertyChanged();
            }
        }

        bool _isPasswordEmpty;
        public bool IsPasswordEmpty
        {
            get => _isPasswordEmpty;
            set
            {
                if (value == _isPasswordEmpty)
                    return;

                _isPasswordEmpty = value;
                OnPropertyChanged();
            }
        }

        public CredentialsMasterPasswordViewModel(Action<CredentialsMasterPasswordViewModel> okCommand, Action<CredentialsMasterPasswordViewModel> cancelHandler)
        {
            OkCommand = new RelayCommand(p => okCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
        }

        void ValidatePassword()
        {
            IsPasswordEmpty = (Password == null || Password.Length == 0);
        }
    }
}
