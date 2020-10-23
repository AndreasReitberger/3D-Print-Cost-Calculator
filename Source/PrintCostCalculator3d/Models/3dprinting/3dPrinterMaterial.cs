using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AndreasReitberger.Enums;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Models._3dprinting
{

    public class _3dPrinterMaterial : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #region Properties
        public Guid Id
        { get; set; }
        public string Name
        { get; set; }
        public string SKU
        { get; set; }
        public Unit Unit
        { get; set; }
        public int TemperatureNozzle
        { get; set; }
        public int TemperatureHeatbed
        { get; set; }
        public decimal PackageSize
        { get; set; }
        public decimal Density
        { get; set; }
        public _3dPrinterMaterialTypes TypeOfMaterial
        { get; set; }
        // Delete later
        /*
        public _3dPrinterMaterialType Type
        { get;set; }
        public _3dPrinterMaterialKind Kind
        { get; set; }
        */
        //
        [XmlElement(Namespace = "PrintCostCalculator3d.Models._3dprinting")]
        public ManufacturerOld Manufacturer
        { get; set; }
        [XmlElement(Namespace = "PrintCostCalculator3d.Models._3dprinting")]
        public SupplierOld Supplier
        { get; set; }
        public decimal UnitPrice
        { get; set; }
        public string LinkToReorder
        { get; set; }
        public string ColorCode
        { get; set; }
        #endregion

        #region Constructor
        public _3dPrinterMaterial() { }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return this.Name;
        }
        public override bool Equals(object obj)
        {
            var item = obj as _3dPrinterMaterial;
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
    public class UnitFactor
    {
        public static Dictionary<UnitOld, int> UnitFactors = new Dictionary<UnitOld, int>()
        {
            {UnitOld.g, 1 },
            {UnitOld.kg, 1000 },
            {UnitOld.ml, 1 },
            {UnitOld.l, 1000 },
        };
        public static int getUnitFactor(UnitOld unit)
        {
            if (UnitFactors.ContainsKey(unit))
                return UnitFactors[unit];
            else return 1;
        }
    }
    public enum UnitOld
    {
        [LocalizedDescription("UnitGram", typeof(Strings))]
        g,
        [LocalizedDescription("UnitKilogram", typeof(Strings))]
        kg,
        [LocalizedDescription("UnitMililiter", typeof(Strings))]
        ml,
        [LocalizedDescription("UnitLiter", typeof(Strings))]
        l,
    }

    public class _3dPrinterMaterialTypes
    {
        #region Properties 
        public Guid Id
        { get; set; }
        public _3dPrinterMaterialKind Kind
        { get; set; }
        public string Material
        { get; set; }
        public string Polymer
        { get; set; }
        #endregion

        #region Constructors
        public _3dPrinterMaterialTypes() { }
        public _3dPrinterMaterialTypes(_3dPrinterMaterialKind Kind, string Material)
        {
            this.Kind = Kind;
            this.Material = Material;
        }
        public _3dPrinterMaterialTypes(_3dPrinterMaterialKind Kind, string Material, string Polymer)
        {
            this.Kind = Kind;
            this.Material = Material;
            this.Polymer = Polymer;
        }
        #endregion

        #region Override
        public override string ToString()
        {
            return string.IsNullOrEmpty(Kind.ToString()) ? Material : string.Format("{0} ({1})", Material, Kind.ToString());
        }
        public override bool Equals(object obj)
        {
            var item = obj as _3dPrinterMaterialTypes;
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

    public enum _3dPrinterMaterialKind
    {
        [LocalizedDescription("Filament", typeof(Strings))]
        Filament,
        [LocalizedDescription("Resin", typeof(Strings))]
        Resin,
        [LocalizedDescription("Powder", typeof(Strings))]
        Powder,
        [LocalizedDescription("Misc", typeof(Strings))]
        Misc,
    }

    public class ManufacturerOld : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #region Properties 
        public Guid Id
        { get; set; }
        public string Name
        { get; set; }
        public string DebitorNumber
        { get; set; }
        public bool isActive
        { get; set; }
        public string Website
        { get; set; }
        #endregion

        #region Constructor
        public ManufacturerOld() { }
        #endregion

        #region Override
        public override string ToString()
        {
            return string.IsNullOrEmpty(DebitorNumber) ? Name : string.Format("{0} ({1})", Name, DebitorNumber);
        }
        public override bool Equals(object obj)
        {
            var item = obj as ManufacturerOld;
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
    public class SupplierOld : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #region Properties 
        public Guid Id 
        { get; set; }
        public string Name
        { get; set; }
        public string DebitorNumber
        { get; set; }
        public bool isActive
        { get; set; }
        public string Website
        { get; set; }
        #endregion

        #region Constructor
        public SupplierOld() { }
        #endregion

        #region Override
        public override string ToString()
        {
            return string.IsNullOrEmpty(DebitorNumber) ? Name : string.Format("{0} ({1})", Name, DebitorNumber);
        }
        public override bool Equals(object obj)
        {
            var item = obj as SupplierOld;
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
}
