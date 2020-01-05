using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WpfFramework.Resources.Localization;

namespace WpfFramework.Models._3dprinting
{
    public class _3dPrinterModel
    {
        #region Properties
        public Guid Id { get; set; }
        public _3dPrinterType Type
        { get; set; }

        public Manufacturer Manufacturer
        { get; set; }

        public string Model
        { get; set; }

        public Supplier Supplier
        { get; set; }

        public decimal Price
        { get;set; }
        public string ShopUri
        { get; set; }
        public _3dPrinterMaterialKind Kind
        { get;set; }
        public bool hasHeatbed
        { get; set; }
        public int MaxHeatbedTemperature
        { get; set; }
        public int MaxNozzleTemperature
        { get; set; }

        public int PowerConsumption
        { get; set; }

        public BuildVolume BuildVolume
        { get; set; }

        private decimal _mhr = 0;
        public decimal MachineHourRate
        {
            get
            {
                if (MachineHourRateCalc == null)
                    return _mhr;
                else
                    return MachineHourRateCalc.CalcMachineHourRate;
            }
            set
            {
                if(_mhr != value)
                {
                    _mhr = value;
                }
            }
        }
        public string MachineHourRateFormatedString
        {
            get => String.Format("{0:C}", MachineHourRate);
        }
        public MachineHourRate MachineHourRateCalc
        { get; set; }
        public string Name
        {
            get =>  Manufacturer != null ? string.Format("{0}, {1}", Manufacturer.Name, this.Model) : this.Model ;
        }
        #endregion

        #region Constructor
        public _3dPrinterModel()
        {
            //Id = Guid.NewGuid();
        }
        public _3dPrinterModel(_3dPrinterType Type)
        {
            this.Type = Type;
            //Id = Guid.NewGuid();
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return Manufacturer != null ? string.Format("{0}, {1}", Manufacturer.Name, this.Model) : this.Model ;
        }
        public override bool Equals(object obj)
        {
            var item = obj as _3dPrinterModel;
            if (item == null)
                return false;
            return this.Id.Equals(item.Id);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        #endregion
    }
    public class BuildVolume : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properies
        private decimal _x = 1;
        public decimal X
        {
            get => _x;
            set
            {
                if(_x != value)
                {
                    _x = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }
        private decimal _y = 1;
        public decimal Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }
        private decimal _z = 1;
        public decimal Z
        {
            get => _z;
            set
            {
                if (_z != value)
                {
                    _z = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        public decimal Volume
        {
            get => Math.Round(X * Y * Z, 2);
        }
        #endregion

        #region Constructors
        public BuildVolume() { }
        public BuildVolume(decimal x, decimal y, decimal z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        #endregion
    }
    public enum _3dPrinterType
    {
        [LocalizedDescription("FDM", typeof(Strings))]
        FDM,
        [LocalizedDescription("SLA", typeof(Strings))]
        SLA,
        [LocalizedDescription("DLP", typeof(Strings))]
        DLP,
        [LocalizedDescription("CLDP", typeof(Strings))]
        CLDP,
        [LocalizedDescription("MJ", typeof(Strings))]
        MJ,
        [LocalizedDescription("NPJ", typeof(Strings))]
        NPJ,
        [LocalizedDescription("DOD", typeof(Strings))]
        DOD,
        [LocalizedDescription("BJ", typeof(Strings))]
        BJ,
        [LocalizedDescription("MJF", typeof(Strings))]
        MJF,
        [LocalizedDescription("SLS", typeof(Strings))]
        SLS,
        [LocalizedDescription("SLM", typeof(Strings))]
        SLM,
        [LocalizedDescription("DMLS", typeof(Strings))]
        DMLS,
        [LocalizedDescription("EBM", typeof(Strings))]
        EBM,
        [LocalizedDescription("LENS", typeof(Strings))]
        LENS,
        [LocalizedDescription("EBAM", typeof(Strings))]
        EBAM,
    }
    /*
    public enum PrinterAttribute
    {
        Nozzle_Diameter,
        Nozzle_Temperature,
        
    }
    */
}
