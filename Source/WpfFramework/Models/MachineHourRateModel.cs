using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfFramework.Models
{
    public class MachineHourRate : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _perYear = true;
        public bool PerYear
        {
            get => _perYear;
            set
            {
                if (_perYear != value)
                {
                    _perYear = value;
                    OnPropertyChanged();
                    //Dependencies
                    OnPropertyChanged(nameof(CalcDepreciation));
                    OnPropertyChanged(nameof(CalcInterest));
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        private long _machineHours = 0;
        public long MaschineHours
        {
            get => _machineHours;
            set
            {
                if (_machineHours != value)
                {
                    _machineHours = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                }
            }
        }

        private decimal _replacementCosts = 0;
        public decimal ReplacementCosts
        {
            get => _replacementCosts;
            set
            {
                if (_replacementCosts != value)
                {
                    _replacementCosts = value;
                    OnPropertyChanged();
                    //Dependencies
                    OnPropertyChanged(nameof(CalcDepreciation));
                    OnPropertyChanged(nameof(CalcInterest));
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        private int _usefulLife = 4;
        public int UsefulLifeYears
        {
            get => _usefulLife;
            set
            {
                if (_usefulLife != value)
                {
                    _usefulLife = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcDepreciation));
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        public decimal CalcDepreciation
        {
            get
            {
                if (ReplacementCosts == 0 || UsefulLifeYears == 0)
                    return 0;
                else
                    return ReplacementCosts / UsefulLifeYears;
            }
        }

        private decimal _interestRate = 3;
        public decimal InterestRate
        {
            get => _interestRate;
            set
            {
                if (_interestRate != value)
                {
                    _interestRate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcInterest));
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }
        public decimal CalcInterest
        {
            get
            {
                if (ReplacementCosts == 0 || InterestRate == 0)
                    return 0;
                else
                {
                    return (ReplacementCosts / 2) / 100 * InterestRate;
                }
            }
        }

        private decimal _maintenanceCosts = 0;
        public decimal MaintenanceCosts
        {
            get => _maintenanceCosts;
            set
            {
                if (_maintenanceCosts != value)
                {
                    _maintenanceCosts = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }

        }

        private decimal _locationCosts = 0;
        public decimal LocationCosts
        {
            get => _locationCosts;
            set
            {
                if (_locationCosts != value)
                {
                    _locationCosts = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        private decimal _energyCosts = 0;
        public decimal EnergyCosts
        {
            get => _energyCosts;
            set
            {
                if (_energyCosts != value)
                {
                    _energyCosts = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        //Additonal
        private decimal _additionalCosts = 0;
        public decimal AdditionalCosts
        {
            get => _additionalCosts;
            set
            {
                if (_additionalCosts != value)
                {
                    _additionalCosts = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        private decimal _maintenanceCostsVariable = 0;
        public decimal MaintenanceCostsVariable
        {
            get => _maintenanceCostsVariable;
            set
            {
                if (_maintenanceCostsVariable != value)
                {
                    _maintenanceCostsVariable = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        private decimal _energyCostsVariable = 0;
        public decimal EnergyCostsVariable
        {
            get => _energyCostsVariable;
            set
            {
                if (_energyCostsVariable != value)
                {
                    _energyCostsVariable = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        private decimal _additionalCostsVariable = 0;
        public decimal AdditionalCostsVariable
        {
            get => _additionalCostsVariable;
            set
            {
                if (_additionalCostsVariable != value)
                {
                    _additionalCostsVariable = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalcMachineHourRate));
                    OnPropertyChanged(nameof(TotalCosts));
                }
            }
        }

        public decimal CalcMachineHourRate
        {
            get
            {
                return getMachineHourRate();
            }
        }
        public decimal TotalCosts
        {
            get
            {
                return getTotalCosts();
            }
        }
        public string CurrencySymbol { get; set; }
        #endregion

        #region Constructor
        public MachineHourRate() { }
        #endregion

        #region PrivateMethods
        private decimal getMachineHourRate()
        {
            decimal res = 0;
            try
            {
                res = (CalcDepreciation + CalcInterest + (MaintenanceCosts + LocationCosts + EnergyCosts + AdditionalCosts) * (PerYear ? 1 : 12)
                    + (MaintenanceCostsVariable + EnergyCostsVariable + AdditionalCostsVariable) * (PerYear ? 1 : 12))  / (MaschineHours * (PerYear ? 1 : 12));
                return res;
            }
            catch(Exception)
            {
                return 0;
            }
        }
        private decimal getTotalCosts()
        {
            decimal res = 0;
            try
            {
                res = (ReplacementCosts + (CalcInterest + 
                    ((MaintenanceCosts + LocationCosts + EnergyCosts + AdditionalCosts)
                    + (MaintenanceCostsVariable + EnergyCostsVariable + AdditionalCostsVariable)) * (PerYear ? 1 : 12)) 
                    * UsefulLifeYears) ;
                return res;
            }
            catch(Exception)
            {
                return 0;
            }
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return string.Format("{0} {1}", CalcMachineHourRate, CurrencySymbol);
        }
        #endregion
    }
}
