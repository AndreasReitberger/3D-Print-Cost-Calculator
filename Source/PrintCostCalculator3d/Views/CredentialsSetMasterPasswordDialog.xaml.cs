﻿namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für CredentialsSetMasterPasswordDialog.xaml
    /// </summary>
    public partial class CredentialsSetMasterPasswordDialog
    {
        public CredentialsSetMasterPasswordDialog()
        {
            InitializeComponent();
        }
        void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Need to be in loaded event, focusmanger won't work...
            PasswordBoxPassword.Focus();
        }
    }
}
