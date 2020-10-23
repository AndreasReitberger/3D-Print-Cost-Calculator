using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Models.Exporter
{
    public class ExporterTemplate
    {
        #region Properties
        public Guid Id { get; set; }
        public bool IsDefault { get;set; }
        public string Name { get; set; }
        public ExporterTarget ExporterTarget { get; set; }
        public string TemplatePath { get; set; }
        public ObservableCollection<ExporterSettings> Settings { get; set; }
        #endregion

        #region Constructor
        public ExporterTemplate()
        {
            Settings = new ObservableCollection<ExporterSettings>();
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("{0}{1}", this.Name, this.IsDefault ?  string.Format(" [{0}]", Strings.Default) : string.Empty);
        }
        #endregion
    }
    public class ExporterSettings
    {
        #region Properties
        public Guid Id { get; set; }
        public ExcelCoordinates Coordinates { get; set; }
        //public ExporterProperty Property { get; set; }
        public ExporterAttribute Attribute { get; set; }
        //public bool IsNumericValue { get; set; }
        //public string FormatString { get; set; }
        public string WorkSheetName { get; set; }
        #endregion

        #region Constructor
        public ExporterSettings() { }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("{0} ({1}:{2}@{3})", this.Attribute.Property, this.Coordinates.Column, this.Coordinates.Row, this.WorkSheetName);
        }
        public override bool Equals(object obj)
        {
            var item = obj as ExporterSettings;
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
    public class ExcelCoordinates
    {
        #region Properties
        public string Column { get; set; }
        public int Row { get; set; }
        #endregion

        #region Constructor
        public ExcelCoordinates() { }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Column, this.Row);
        }
        #endregion
    }
    #region Enums
    public enum ExporterTarget
    {
        [LocalizedDescription("Single", typeof(Strings))]
        Single,
        [LocalizedDescription("List", typeof(Strings))]
        List,
    }
    
    public class ExporterAttribute
    {
        #region Properties
        public Guid Id { get; set; }
        public ExporterProperty Property { get; set; }
        public ExporterTarget Target { get; set; }
        #endregion

        #region Constructor
        public ExporterAttribute() { }
        #endregion

        #region Static
        public static List<ExporterAttribute> Attributes = new List<ExporterAttribute>()
        {
            // List
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CalculationList},
            /* Add with later feature
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CompanyName},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CustomerAddressCity},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CustomerAddressNumber},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CustomerAddressStreet},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CustomerCountry},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CustomerFirstName},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CustomerId},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.CustomerLastName},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.OfferId},
            new ExporterAttribute() { Target = ExporterTarget.List, Property = ExporterProperty.UstId},
            */
            // Single
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationFailrate},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationMargin},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationMaterial},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPriceEnergy},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPriceHandling},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPricePrinter},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPriceMaterial},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPriceTax},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPriceTotal},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPriceWorksteps},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPrinter},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationPrintTime},
            new ExporterAttribute() { Target = ExporterTarget.Single, Property = ExporterProperty.CalculationQuantity},

        };
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("{0}", this.Property);
        }

        public override bool Equals(object obj)
        {
            var item = obj as ExporterAttribute;
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
    public enum ExporterProperty
    {
        [LocalizedDescription("CalculationList", typeof(Strings))]
        CalculationList,
        [LocalizedDescription("CalculationPriceTotal", typeof(Strings))]
        CalculationPriceTotal,
        [LocalizedDescription("CalculationPriceTax", typeof(Strings))]
        CalculationPriceTax,
        [LocalizedDescription("CalculationPriceEnergy", typeof(Strings))]
        CalculationPriceEnergy,
        [LocalizedDescription("CalculationPriceMaterial", typeof(Strings))]
        CalculationPriceMaterial,
        [LocalizedDescription("CalculationPricePrinter", typeof(Strings))]
        CalculationPricePrinter,
        [LocalizedDescription("CalculationPriceWorksteps", typeof(Strings))]
        CalculationPriceWorksteps,
        [LocalizedDescription("CalculationPriceHandling", typeof(Strings))]
        CalculationPriceHandling,
        [LocalizedDescription("CalculationMargin", typeof(Strings))]
        CalculationMargin,
        [LocalizedDescription("CalculationFailrate", typeof(Strings))]
        CalculationFailrate,
        [LocalizedDescription("CalculationQuantity", typeof(Strings))]
        CalculationQuantity,
        [LocalizedDescription("CalculationPrinter", typeof(Strings))]
        CalculationPrinter,
        [LocalizedDescription("CalculationMaterial", typeof(Strings))]
        CalculationMaterial,
        [LocalizedDescription("CalculationPrintTime", typeof(Strings))]
        CalculationPrintTime,
        
        [LocalizedDescription("CompanyName", typeof(Strings))]
        CompanyName,
        [LocalizedDescription("UstId", typeof(Strings))]
        UstId,
        [LocalizedDescription("CustomerFirstName", typeof(Strings))]
        CustomerFirstName,
        [LocalizedDescription("CustomerLastName", typeof(Strings))]
        CustomerLastName,
        [LocalizedDescription("CustomerId", typeof(Strings))]
        CustomerId,
        [LocalizedDescription("CustomerAddressStreet", typeof(Strings))]
        CustomerAddressStreet,
        [LocalizedDescription("CustomerAddressNumber", typeof(Strings))]
        CustomerAddressNumber,
        [LocalizedDescription("CustomerAddressCity", typeof(Strings))]
        CustomerAddressCity,
        [LocalizedDescription("CustomerCountry", typeof(Strings))]
        CustomerCountry,
        [LocalizedDescription("OfferId", typeof(Strings))]
        OfferId, 
        /**/
    }
    #endregion
}
