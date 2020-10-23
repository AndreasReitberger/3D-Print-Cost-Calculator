using PrintCostCalculator3d.Models._3dprinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PrintCostCalculator3d.Models
{
    public class MaterialStockItem : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        private string _sku;
        public string SKU
        { 
            get => _sku;
            set
            {
                if (_sku == value) return;
                _sku = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<_3dPrinterMaterial> _materials;
        public ObservableCollection<_3dPrinterMaterial> Materials
        {
            get => _materials;
            set
            {
                if (_materials == value) return;
                
                _materials = value;
                OnPropertyChanged();
                
            }
        }

        private int _minimumStock = -1;
        public int MinimalInstock
        {
            get => _minimumStock;
            set
            {
                if (_minimumStock == value) return;
                
                _minimumStock = value;
                OnPropertyChanged();                
            }
        }

        #endregion

        #region Constructor
        public MaterialStockItem() 
        {
            Materials = new ObservableCollection<_3dPrinterMaterial>();
        }
        #endregion
    }

    public class MaterialCatridge
    {
        #region Properties
        public _3dPrinterMaterial Material { get; set; }
        public double RemainingMaterial { get; set; }
        public bool IsSealed { get; set; }
        #endregion

        #region Constructor
        public MaterialCatridge() { }
        #endregion
    }
    public class StockAmount
    {
        #region Properties
        public decimal Amount
        { get; set; }
        public UnitOld Unit
        { get; set; }
        #endregion

        #region Constructor
        public StockAmount() { }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("{0} {1}", Amount, Unit);
        }
        #endregion
    }

    public class StockTransaktion
    {
        #region Properties
        public MaterialStockItem Item
        { get; set; }
        public StockAmount Amount
        { get; set; }
        public DateTime Timestamp
        { get; set; }
        public string Username
        { get; set; }
        #endregion

        #region Constructor
        public StockTransaktion() { }
        #endregion
    }
}
