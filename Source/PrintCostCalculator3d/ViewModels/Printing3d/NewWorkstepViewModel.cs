using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Models.WorkstepAdditions;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class NewWorkstepViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                if (value == _isEdit)
                    return;

                _isEdit = value;
                OnPropertyChanged();
            }
        }

        Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
                OnPropertyChanged();           
            }
        }

        string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        CalculationType _calcType = CalculationType.PerPiece;
        public CalculationType CalculationType
        {
            get => _calcType;
            set
            {
                if (_calcType == value) return;              
                _calcType = value;
                OnPropertyChanged();  
            }
        }

        WorkstepType _type = WorkstepType.Post;
        public WorkstepType Type
        {
            get => _type;
            set
            {
                if (_type == value) return;
                _type = value;
                OnPropertyChanged();                
            }
        }

        WorkstepCategory _category;
        public WorkstepCategory Category
        {
            get => _category;
            set
            {
                if (_category == value) return;
                _category = value;
                OnPropertyChanged();
            }
        }

        double _amount = 0;
        public double Amount
        {
            get => _amount; 
            set
            {
                if (_amount == value) return;
                _amount = value;
                OnPropertyChanged();
            }
        }

        TimeSpan _duration = TimeSpan.Zero;
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                if (_duration == value) return;
                _duration = value;
                OnPropertyChanged();
                
            }
        }
        #endregion

        #region EnumCollections

        #region WorkstepTypes
        ObservableCollection<WorkstepType> _workstepTypes = new ObservableCollection<WorkstepType>(
            Enum.GetValues(typeof(WorkstepType)).Cast<WorkstepType>().ToList()
            );
        public ObservableCollection<WorkstepType> WorkstepTypes
        {
            get => _workstepTypes;
            set
            {
                if (_workstepTypes == value) return;
                _workstepTypes = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region CalculationTypes
        ObservableCollection<CalculationType> _calculationTypes = new ObservableCollection<CalculationType>(
            Enum.GetValues(typeof(CalculationType)).Cast<CalculationType>().ToList()
            );
        public ObservableCollection<CalculationType> CalculationTypes
        {
            get => _calculationTypes;
            set
            {
                if (_calculationTypes == value) return;

                _calculationTypes = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #endregion

        #region Settings
        ObservableCollection<WorkstepCategory> _categories = new ObservableCollection<WorkstepCategory>();
        public ObservableCollection<WorkstepCategory> Categories
        {
            get => _categories;
            set
            {
                if (_categories == value) return;
                if(!IsLoading)
                    SettingsManager.Current.WorkstepCategories = value;
                _categories = value;
                OnPropertyChanged();
            }

        }
        #endregion

        #region Constructor, LoadSettings
        public NewWorkstepViewModel(Action<NewWorkstepViewModel> saveCommand, Action<NewWorkstepViewModel> cancelHandler, Workstep workstep = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            Categories.CollectionChanged += Categories_CollectionChanged;

            IsEdit = workstep != null;
            try
            {
                LoadItem(workstep ?? new Workstep());
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public NewWorkstepViewModel(Action<NewWorkstepViewModel> saveCommand, Action<NewWorkstepViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, Workstep workstep = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            _dialogCoordinator = dialogCoordinator;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            Categories.CollectionChanged += Categories_CollectionChanged;

            IsEdit = workstep != null;
            try
            {
                LoadItem(workstep ?? new Workstep());
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        void LoadSettings()
        {
            Categories = SettingsManager.Current.WorkstepCategories;
        }
        #endregion

        #region Events
        void Categories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Save();
            OnPropertyChanged(nameof(Categories));
        }
        #endregion

        #region ICommands & Actions
        public ICommand NewCategoryCommand
        {
            get => new RelayCommand(async(p) => await NewCategoryAction());
        }
        async Task NewCategoryAction()
        {
            try { 
                var _dialog = new CustomDialog() { Title = Strings.NewCategory };
                var newWorkstepCategoryViewModel = new NewWorkstepCategoryViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Categories.Add(new WorkstepCategory()
                    {
                        Name = instance.Name,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Categories[Categories.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                });

                _dialog.Content = new Views._3dPrinting.NewWorkstepCategoryDialog()
                {
                    DataContext = newWorkstepCategoryViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
                }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion

        #region Methods
        void LoadItem(Workstep workstep)
        {
            // Load Id if material is not null
            if (workstep != null && workstep.Id != Guid.Empty)
                Id = workstep.Id;

            Name = workstep.Name;
            Amount = workstep.Price;
            CalculationType = workstep.CalculationType;
            Type = workstep.Type;
            Category = workstep.Category;
        }
        #endregion
    }
}
