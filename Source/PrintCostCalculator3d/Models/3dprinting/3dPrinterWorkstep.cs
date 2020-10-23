using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AndreasReitberger.Models.WorkstepAdditions;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Models._3dprinting
{
    public class _3dPrinterWorkstep : ICloneable
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
        public decimal Price
        { get; set; }
        public WorkstepCategory Category
        { get; set; }
        [XmlElement(Namespace = "PrintCostCalculator3d.Models._3dprinting")]
        public CalculationType CalculationType
        { get; set; }
        [XmlElement(Type = typeof(XmlTimeSpan))]
        public TimeSpan Duration
        { get; set; }
        [XmlElement(Namespace = "PrintCostCalculator3d.Models._3dprinting")]
        public WorkstepType Type
        { get; set; }
        #endregion

        #region Constructors
        public _3dPrinterWorkstep() { }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("{0} ({1}) - {2:C2}", this.Name, this.Type, this.Price);
        }
        public override bool Equals(object obj)
        {
            var item = obj as _3dPrinterWorkstep;
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
    public class WorkstepCategoryOld
    {
        #region Properties
        public string Name
        { get; set; }
        #endregion

        #region Constructors
        public WorkstepCategoryOld() { }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return Name;
        }
        public override bool Equals(object obj)
        {
            var item = obj as WorkstepCategoryOld;
            if (item == null)
                return false;
            return this.Name.Equals(item.Name);
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        #endregion
    }
    public enum CalculationType
    {
        [LocalizedDescription("PerHour", typeof(Strings))]
        per_hour,
        [LocalizedDescription("PerJob", typeof(Strings))]
        per_job,
        [LocalizedDescription("PerPiece", typeof(Strings))]
        per_piece,
    }
    public enum WorkstepType
    {
        [LocalizedDescription("Pre", typeof(Strings))]
        PRE,
        [LocalizedDescription("Post", typeof(Strings))]
        POST,
        [LocalizedDescription("Misc", typeof(Strings))]
        MISC,
    }
    public class XmlTimeSpan
    {
        private const long TICKS_PER_MS = TimeSpan.TicksPerMillisecond;

        private TimeSpan m_value = TimeSpan.Zero;

        public XmlTimeSpan() { }
        public XmlTimeSpan(TimeSpan source) { m_value = source; }

        public static implicit operator TimeSpan?(XmlTimeSpan o)
        {
            return o == null ? default(TimeSpan?) : o.m_value;
        }

        public static implicit operator XmlTimeSpan(TimeSpan? o)
        {
            return o == null ? null : new XmlTimeSpan(o.Value);
        }

        public static implicit operator TimeSpan(XmlTimeSpan o)
        {
            return o == null ? default(TimeSpan) : o.m_value;
        }

        public static implicit operator XmlTimeSpan(TimeSpan o)
        {
            return o == default(TimeSpan) ? null : new XmlTimeSpan(o);
        }

        [XmlText]
        public long Default
        {
            get { return m_value.Ticks / TICKS_PER_MS; }
            set { m_value = new TimeSpan(value * TICKS_PER_MS); }
        }
    }

}
