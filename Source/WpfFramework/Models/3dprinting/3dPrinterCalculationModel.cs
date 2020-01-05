
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

//Thirdparty


namespace WpfFramework.Models._3dprinting
{
    public class _3dPrinterCalculationModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        [XmlIgnore] public bool CalculationChanged { get; set; }

        private string _calculationVersion = "0.0.0.0";
        public string CalculationVersion
        {
            get => _calculationVersion;
            set
            {
                if (value == _calculationVersion)
                    return;

                _calculationVersion = value;
                CalculationChanged = true;
            }
        }

        private Guid _id;
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

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private _3dPrinterModel _printer;
        public _3dPrinterModel Printer
        {
            get => _printer;
            set
            {
                if(_printer != value)
                {
                    _printer = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalculatedPrice));
                    OnPropertyChanged(nameof(CalculatedPrintTime));
                    OnPropertyChanged(nameof(CalculatedTax));
                    OnPropertyChanged(nameof(CalculatedMaterialCosts));
                    OnPropertyChanged(nameof(CalculatedEnergyCosts));
                    OnPropertyChanged(nameof(CalculatedMargin));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        private ObservableCollection<_3dPrinterModel> _printers = new ObservableCollection<_3dPrinterModel>();
        public ObservableCollection<_3dPrinterModel> Printers
        {
            get => _printers;
            set
            {
                if (_printers != value)
                {
                    _printers = value;
                    OnPropertyChanged();
                    if (_printers.Count > 0) _printer = _printers[0];
                    OnPropertyChanged(nameof(CalculatedPrice));
                }
            }
        }

        private _3dPrinterMaterial _material;
        public _3dPrinterMaterial Material
        {
            get => _material;
            set
            {
                if (_material != value)
                {
                    _material = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CalculatedPrice));
                    OnPropertyChanged(nameof(CalculatedPrintTime));
                    OnPropertyChanged(nameof(CalculatedTax));
                    OnPropertyChanged(nameof(CalculatedMaterialCosts));
                    OnPropertyChanged(nameof(CalculatedEnergyCosts));
                    OnPropertyChanged(nameof(CalculatedMargin));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }


        private ObservableCollection<_3dPrinterMaterial> _materials = new ObservableCollection<_3dPrinterMaterial>();
        public ObservableCollection<_3dPrinterMaterial> Materials
        {
            get => _materials;
            set
            {
                if (_materials != value)
                {
                    _materials = value;
                    if (_materials.Count > 0) _material = _materials[0];
                    OnPropertyChanged();
                }
            }
        }

        private bool _applyTax = true;
        public bool ApplyTax
        {
            get => _applyTax;
            set
            {
                if (_applyTax != value)
                {
                    _applyTax = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _applyEnergyCosts = true;
        public bool ApplyEnergyCosts
        {
            get => _applyEnergyCosts;
            set
            {
                if (_applyEnergyCosts != value)
                {
                    _applyEnergyCosts = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _powerLevel;
        public int PowerLevel
        {
            get => _powerLevel;
            set
            {
                if (_powerLevel != value)
                {
                    _powerLevel = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _profit;
        public int Profit
        {
            get => _profit;
            set
            {
                if (_profit != value)
                {
                    _profit = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _taxRate;
        public decimal TaxRate
        {
            get => _taxRate;
            set
            {
                if (_taxRate != value)
                {
                    _taxRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _failRate;
        public int FailRate
        {
            get => _failRate;
            set
            {
                if (_failRate != value)
                {
                    _failRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _handlingFee;
        public decimal HandlingFee
        {
            get => _handlingFee;
            set
            {
                if (_handlingFee != value)
                {
                    _handlingFee = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _energyPrice;
        public decimal EnergyPrice
        {
            get => _energyPrice;
            set
            {
                if (_energyPrice != value)
                {
                    _energyPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _duration;
        public decimal Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _consumedMaterial;
        public decimal ConsumedMaterial
        {
            get => _consumedMaterial;
            set
            {
                if (_consumedMaterial != value)
                {
                    _consumedMaterial = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _volume;
        public decimal Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _stlPath;
        public string StlPath
        {
            get => _stlPath;
            set
            {
                if (_stlPath != value)
                {
                    _stlPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _gcodePath;
        public string GcodePath
        {
            get => _gcodePath;
            set
            {
                if (_gcodePath != value)
                {
                    _gcodePath = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public decimal CalculatedPrice
        {
            get => calcualtePrice();
        }
        public decimal CalculatedTax
        {
            get => calculateTax();
        }
        public decimal CalculatedMaterialCosts
        {
            get => calculateMaterialCosts();
        }
        public decimal CalculatedEnergyCosts
        {
            get => calculateEnergyCosts();
        }
        public decimal CalculatedMachineCosts
        {
            get => calculateMachineCosts();
        }
        public decimal Total
        {
            get => CalculatedPrice + CalculatedTax + CalculatedMargin + CalculatedWorkstepCosts;
        }

        public decimal CalculatedPrintTime
        {
            get => Math.Round((Duration + CalculatedFailedPrintTime) * Quantity, 2);
        }

        public decimal CalculatedMargin
        {
            get => calculateMargin();
        }
        
        public decimal CalculatedWorkstepCosts
        {
            get => calculateWorkstepCosts();
        }

        public decimal CalculatedFailedPrintTime
        {
            get => Math.Round(Duration * (FailRate / 100m),2);
        }

        #endregion

        #region Constructor
        public _3dPrinterCalculationModel() { }
        #endregion

        #region Private Methods
        private decimal calcualtePrice()
        {
            if (Printer == null || Material == null)
                return 0;

            decimal price = 0;
            price += HandlingFee;
            price += calculateMachineCosts();
            price += calculateEnergyCosts();
            price += calculateMaterialCosts();
            //Apply Profit
            //price = price + calculateMargin();
            //Add workstep prices
            //price += calculateWorkstepCosts();
            return price;
        }
        private decimal calculateEnergyCosts()
        {
            if (Printer == null || !ApplyEnergyCosts)
                return 0;
            //((TIME x POWER) / 1000) x QUANTITY x (FAILRATE / 100)
            var consumption = (((Duration * Printer.PowerConsumption) / 1000m) * Quantity * (1m + FailRate / 100m)) / 100m * PowerLevel;
            return consumption * EnergyPrice;
        }
        private decimal calculateMachineCosts()
        {
            if (Printer == null)
                return 0;
            //(TIME x MHR) x QUANTITY x (FAILRATE / 100)
            return ((Duration * Printer.MachineHourRate)) * Quantity * (1m + FailRate / 100m);
        }
        private decimal calculateMaterialCosts()
        {
            if (Material == null)
                return 0;
            //((CONSUMED_MATERIAL[g] x PRICE[$/kg]) / 1000) x QUANTITY x (FAILRATE / 100)
            return ((Volume * Material.Density * Material.UnitPrice) / (Material.PackageSize * Convert.ToDecimal(UnitFactor.getUnitFactor(Material.Unit)))) * Quantity * (1m + FailRate / 100m);
        }
        
        private decimal calculateTax()
        {
            return ApplyTax ? (CalculatedPrice + CalculatedMargin + CalculatedWorkstepCosts) * (TaxRate / 100m) : 0;
        }
        private decimal calculateWorkstepCosts()
        {
            decimal price = 0m;
            return price;
        }
        private decimal calculateMargin()
        {
            return (CalculatedPrice) * (Profit / 100m);
        }
        #endregion

        #region StaticMethods
        public static void ParseGcodeFile(string filename)
        {
            /*
            string src = File.ReadAllText(filename);

            var lexer = new Lexer(src);
            List<Token> tokens = lexer.Tokenize().ToList();

            var parser = new Parser(tokens);
            List<Code> gcodes = parser.Parse().ToList();
            // do something with the gcodes
            */
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return this.Name;
        }
        public override bool Equals(object obj)
        {
            var item = obj as _3dPrinterCalculationModel;
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
    public class ProfitInfo
    {
        #region Properties
        /// <summary>Gets or sets the profit factor (2 = 100% profit).</summary>
        /// <value>The profit factor.</value>
        public decimal ProfitFactor
        { get; set; }
        #endregion
    }

    public enum ProfitCalculation
    {
        for_total,
    }

    public class CalculationFile
    {
        public static DESCryptoServiceProvider key = new DESCryptoServiceProvider();
        private static protected string secString = "U4fRwU^K#.fA+$8y";
        public static bool Save(_3dPrinterCalculationModel calc, string path)
        {
            try
            {
                string appFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                XmlSerializer x = new XmlSerializer(typeof(_3dPrinterCalculationModel));
                DirectoryInfo tempDir = new DirectoryInfo(path);
                Directory.CreateDirectory(tempDir.Parent.FullName);
                TextWriter writer = new StreamWriter(tempDir.FullName);
                x.Serialize(writer, calc);
                writer.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                //logger.Error(ex.Message);
            }
        }

        public static bool Save(_3dPrinterCalculationModel[] calcs, string path)
        {
            try
            {
                string appFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                XmlSerializer x = new XmlSerializer(typeof(_3dPrinterCalculationModel[]));
                DirectoryInfo tempDir = new DirectoryInfo(path);
                Directory.CreateDirectory(tempDir.Parent.FullName);
                TextWriter writer = new StreamWriter(tempDir.FullName);
                x.Serialize(writer, calcs);
                writer.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                //logger.Error(ex.Message);
            }
        }
        public static bool Load(string path, out _3dPrinterCalculationModel calc)
        {
            try
            {
                // Construct an instance of the XmlSerializer with the type  
                // of object that is being deserialized.  
                XmlSerializer mySerializer =
                new XmlSerializer(typeof(_3dPrinterCalculationModel));
                // To read the file, create a FileStream.  

                FileStream myFileStream = new FileStream(path, FileMode.Open);
                // Call the Deserialize method and cast to the object type.  
                _3dPrinterCalculationModel retval = (_3dPrinterCalculationModel)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                calc = retval;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                //logger.Error(ex.Message);
                return true;
            }
        }
        public static bool Load(string path, out _3dPrinterCalculationModel[] calcs)
        {
            try
            {
                // Construct an instance of the XmlSerializer with the type  
                // of object that is being deserialized.  
                XmlSerializer mySerializer =
                new XmlSerializer(typeof(_3dPrinterCalculationModel[]));
                // To read the file, create a FileStream.  

                FileStream myFileStream = new FileStream(path, FileMode.Open);
                // Call the Deserialize method and cast to the object type.  
                _3dPrinterCalculationModel[] retval = (_3dPrinterCalculationModel[])mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                calcs = retval;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                //logger.Error(ex.Message);
                return false;
            }
        }

        // https://stackoverflow.com/questions/965042/c-sharp-serializing-deserializing-a-des-encrypted-file-from-a-stream
        public static bool EncryptAndSerialize(string filename, _3dPrinterCalculationModel obj)
        {
            try
            {
                var key = new DESCryptoServiceProvider();
                var e = key.CreateEncryptor(Encoding.ASCII.GetBytes("64bitPas"), Encoding.ASCII.GetBytes(secString));
                using (FileStream fs = File.Open(filename, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fs, e, CryptoStreamMode.Write))
                    {
                        XmlSerializer xmlser = new XmlSerializer(typeof(_3dPrinterCalculationModel));
                        xmlser.Serialize(cs, obj);
                        return true;
                    }
                }
            }
            catch(Exception exc)
            {
                return false;
            }
        }

        public static _3dPrinterCalculationModel DecryptAndDeserialize(string filename)
        {
            try
            {
                var key = new DESCryptoServiceProvider();
                var d = key.CreateDecryptor(Encoding.ASCII.GetBytes("64bitPas"), Encoding.ASCII.GetBytes(secString));
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(fs, d, CryptoStreamMode.Read))
                    {
                        XmlSerializer xmlser = new XmlSerializer(typeof(_3dPrinterCalculationModel));
                        return (_3dPrinterCalculationModel)xmlser.Deserialize(cs);
                    }
                }
            }
            catch(Exception exc)
            {
                return null;
            }
        }
    }
}
