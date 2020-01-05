using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFramework.Models
{
    public class Material
    {
        public MaterialKind Kind { get; set; }
        public MaterialType Type { get; set; }
        public MaterialColor Color { get; set; }
    }
    public enum MaterialColor
    {
        Black,
        Blue,
        Orange,
        White,
        Silver,
    }
    public enum MaterialType
    {
        PLA,
        PET,
        PETG,
        ABS,
        ABST,
        EasyABS,
    }
    public enum MaterialKind
    {
        Filament,
        Resin,

    }
}
