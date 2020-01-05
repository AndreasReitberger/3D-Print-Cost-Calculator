using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfFramework.Models.Slicer;
using WpfFramework.Resources.Localization;
using log4net;
using WpfFramework.Models.GCode;

namespace WpfFramework.Models
{
    /*
    public class GcodeFile_DeleteClass
    {
        #region Variables
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        
        public GCodeFile GCode
        { get; set; }

        public SlicerName SlicerName
        {
            get; private set;
        }
        public bool IsValid
        {
            get => SlicerName != SlicerName.Unkown;
        }
        public decimal NozzleDiameter
        {
            get; private set;
        }

        public decimal Diameter
        {
            get; private set;
        }
        public decimal Volume
        {
            get; private set;
            //get => Convert.ToDecimal(getVolume(), new CultureInfo("en-US"));
        }
        public decimal PrintTime
        {
            get; private set;
        }
        public decimal PrintTimeSilent
        {
            get; private set;
        }
        public string FilamentType
        {
            get; private set;
        }
        public decimal FilamentDensity
        {
            get; private set;
        }
        public string FilePath
        {
            get; set;
        }

        public string FileName
        {
            get => string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileName(FilePath);
        }
        #endregion

        #region Constructors
        public GcodeFile_DeleteClass()
        { }
        public GcodeFile_DeleteClass(string path)
        {

            this.GCode = Load(path);
            this.SlicerName = getSlicerName();
            // Volume
            try
            {
                this.Volume = Convert.ToDecimal(getParameterFromSlicer(SlicerParameter.Volume), CultureInfo.GetCultureInfo("en-US"));
            }
            catch(Exception)
            {
                this.Volume = 0;
            }
            // PrintTime
            try { 
                this.PrintTime = Convert.ToDecimal(convertPrintTimeToDec(
                    getParameterFromSlicer(SlicerParameter.PrintTime)), CultureInfo.GetCultureInfo("en-US"));
            }
            catch (Exception)
            {
                this.PrintTime = 0;
            }
            // PrintTimeSilent
            try { 
            this.PrintTimeSilent = Convert.ToDecimal(convertPrintTimeToDec(
                    getParameterFromSlicer(SlicerParameter.PrintTimeSilent)), CultureInfo.GetCultureInfo("en-US"));
            }
            catch (Exception)
            {
                this.PrintTimeSilent = 0;
            }
            // Diameter
            try { 
                this.Diameter = Convert.ToDecimal(getParameterFromSlicer(SlicerParameter.FilamentDiameter), CultureInfo.GetCultureInfo("en-US"));
            }
            catch (Exception)
            {
                this.Diameter = 0;
            }
            // Nozzle
            try { 
                this.NozzleDiameter = Convert.ToDecimal(getParameterFromSlicer(SlicerParameter.NozzleDiameter), CultureInfo.GetCultureInfo("en-US"));
            }
            catch (Exception)
            {
                this.NozzleDiameter = 0;
            }
            // FilamentType
            try { 
                this.FilamentType = getParameterFromSlicer(SlicerParameter.FilamentType);
            }
            catch (Exception)
            {
                this.FilamentType = Strings.Unknown;
            }
            // FilamentDensity
            try { 
                this.FilamentDensity = Convert.ToDecimal(getParameterFromSlicer(SlicerParameter.FilamentDensity), CultureInfo.GetCultureInfo("en-US"));
            }
            catch (Exception)
            {
                this.FilamentDensity = 0;
            }

            //getVolume();
        }
        #endregion

        #region Methods
        public gs.GCodeFile Load(string path)
        {
            try
            {
                FilePath = path;
                GenericGCodeParser parse = new GenericGCodeParser();
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                return parse.Parse(sr);
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return null;
            }
        }
        private string getGcodeValue(string name)
        {
            string value = string.Empty;
            var line = GCode.AllLines().FirstOrDefault(l => l.orig_string != null && l.orig_string.Contains(name));
            if (line != null)
            {
                if(line.comment.Contains("="))
                    value = line.comment.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries)[1];
                else
                    value = line.comment;
            }
            return value;
        }
        private SlicerName getSlicerName()
        {
            try
            {
                string slicerLine = GCode.AllLines().ElementAt(0).orig_string;
                if (slicerLine.Contains("Slic3r"))
                    return SlicerName.Slic3r;
                else if (slicerLine.Contains("PrusaSlicer"))
                    return SlicerName.PrusaSlicer;
                else if (slicerLine.Contains("KISSlicer"))
                    return SlicerName.KISSlicer;
                else if (slicerLine.Contains("skeinforge"))
                    return SlicerName.Skeinforge;
                else if (slicerLine.Contains("CURA_PROFILE_STRING"))
                    return SlicerName.Cura;
                else if (slicerLine.Contains("Miracle"))
                    return SlicerName.Makerbot;
                else if (slicerLine.Contains("ffslicer"))
                    return SlicerName.FlashForge;
                else if (slicerLine.Contains("Simplify3D"))
                    return SlicerName.Simplify3D;
                else if (slicerLine.Contains("Snapmaker"))
                    return SlicerName.Snapmakerjs;
                else
                {
                    // Snapmaker
                    slicerLine = GCode.AllLines().ElementAt(5).orig_string;
                    if (slicerLine.Contains("Cura_SteamEngine"))
                        return SlicerName.Snapmakerjs;
                    else
                    {
                        logger.Error(string.Format(Strings.EventUnknownSlicerFormated, slicerLine));
                        return SlicerName.Unkown;
                    }
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return SlicerName.Unkown;
            }
        }
        private string getParameterFromSlicer(SlicerParameter Parameter)
        {
            try
            {
                Regex myregex;
                List<GCodeLine> lines = new List<GCodeLine>();
                switch (SlicerName)
                {
                    // Old Prusa Slicer
                    case SlicerName.Slic3r:
                        switch (Parameter)
                        {
                            case SlicerParameter.Volume:
                                myregex = new Regex(@"[;]\s*filament used\s*=\s*(\d*\.\d+)mm\s*[(](\d*\.\d+)cm3[)]");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"\(([^)]*)cm3\)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"((\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"[;]\s*estimated printing time \(silent mode\)\s*=*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"((\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filament_diameter\s*=\s*\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ =\s](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*nozzle_diameter\s*=\s*\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*filament_type\s*=\s*([A-Z])*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ =\s]([A-Z]\w))").Groups[1].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filament_density\s*=\s*\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ =\s](\d*.\d{1,2}))|([^ =\s]([0-9]*$))").Groups[1].Value;
                            default:
                                return "Unkown parameter";
                        }
                    // New Prusa Slicer
                    case SlicerName.PrusaSlicer:
                        switch (Parameter)
                        {
                             
                            case SlicerParameter.Volume:
                                myregex = new Regex(@"[;]\s*filament used\s*\[cm3\]\s*=\s*\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"(\s\d*.\d)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                //myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=\s*\d*h\s*\d*m\s*\d*s");
                                myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"((\d*d\s\d*h\s*\d*m\s\d*s)|(\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"[;]\s*estimated printing time \(silent mode\)\s*=*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"((\d*d\s\d*h\s*\d*m\s\d*s)|(\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filament_diameter\s*=\s*\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ =\s](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*nozzle_diameter\s*=\s*\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*filament_type\s*=\s*([A-Z])*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ =\s]([A-Z]\w))").Groups[1].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filament_density\s*=\s*\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            default:
                                return "Unkown parameter";
                        }     
                    // Simplify3D
                    case SlicerName.Simplify3D:
                        switch (Parameter)
                        {                            
                            case SlicerParameter.Volume:
                                myregex = new Regex(@"([;]\s*Plastic volume:\s*)");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                //var s = Regex.Match(lines[0].orig_string, @"(\d*.\d{1,2})\s*(cc)").Groups[1].Value;
                                return Regex.Match(lines[0].orig_string, @"(\d*.\d{1,2})\s*(cc)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                //myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=\s*\d*h\s*\d*m\s*\d*s");
                                myregex = new Regex(@"([;]\s*Build time:*)");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                //var s2 = Regex.Match(lines[0].orig_string, @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                                return Regex.Match(lines[0].orig_string, @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"([;]\s*Build time:*)");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                //var s2 = Regex.Match(lines[0].orig_string, @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                                return Regex.Match(lines[0].orig_string, @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filamentDiameters,\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*extruderDiameter,\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*printMaterial,\w*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                var s = Regex.Match(lines[0].orig_string, @"(,)([^,]*)").Groups[2].Value;
                                return Regex.Match(lines[0].orig_string, @"(,)([^,]*)").Groups[2].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filamentDensities,\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            default:
                                return "Unkown parameter";
                        }      
                    // Snapmaker
                    /*
                    case SlicerName.Snapmakerjs:
                    case SlicerName.Cura:
                        switch (Parameter)
                        {                            
                            case SlicerParameter.Volume:
                                myregex = new Regex(@"([;]\s*Plastic volume:\s*)");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                //var s = Regex.Match(lines[0].orig_string, @"(\d*.\d{1,2})\s*(cc)").Groups[1].Value;
                                return Regex.Match(lines[0].orig_string, @"(\d*.\d{1,2})\s*(cc)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                //myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=\s*\d*h\s*\d*m\s*\d*s");
                                myregex = new Regex(@"([;]\s*Build time:*)");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                //var s2 = Regex.Match(lines[0].orig_string, @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                                return Regex.Match(lines[0].orig_string, @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"([;]\s*Build time:*)");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                //var s2 = Regex.Match(lines[0].orig_string, @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                                return Regex.Match(lines[0].orig_string, @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filamentDiameters,\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*extruderDiameter,\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*printMaterial,\w*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                var s = Regex.Match(lines[0].orig_string, @"(,)([^,]*)").Groups[2].Value;
                                return Regex.Match(lines[0].orig_string, @"(,)([^,]*)").Groups[2].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filamentDensities,\d*.\d*");
                                lines = GCode.AllLines().Where(line => !string.IsNullOrEmpty(line.orig_string) && myregex.IsMatch(line.orig_string)).ToList();
                                return Regex.Match(lines[0].orig_string, @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            default:
                                return "Unkown parameter";
                        }       
                        
                    default:
                        return "Slicer not tested yet";
                }
                
            }
            catch (Exception)
            {
                return "Error";
            }

        }

        private decimal convertPrintTimeToDec(string printTime)
        {
            try
            {
                string[] time = printTime.Split(' ');
                string timestring = string.Empty;
                if (time.Contains("hour") || time.Contains("hours") 
                    || time.Contains("minutes") || time.Contains("minute")
                    || time.Contains("seconds") || time.Contains("second")
                    )
                {
                    string h = "0", m = "0", s = "0";
                    for(int i = 1; i < time.Length; i+=2)
                    {
                        if(time[i]=="hour" || time[i] == "hours")
                        {
                            h = time[i - 1];
                        }
                        else if(time[i] == "minutes" || time[i] == "minute")
                        {
                            m = time[i - 1];
                        }
                        else if (time[i] == "seconds" || time[i] == "second")
                        {
                            s = time[i - 1];
                        }
                    }
                    timestring = string.Format("{0}:{1}:{2}", h, m, s);
                }
                else {
                        switch (time.Count())
                        {
                            case 4:
                                timestring = string.Format("{0}:{1}:{2}:{3}",
                                time[0].Replace("d", string.Empty),
                                time[1].Replace("h", string.Empty),
                                time[2].Replace("m", string.Empty),
                                time[3].Replace("s", string.Empty)
                                );
                                break;
                            case 3:
                                timestring = string.Format("{0}:{1}:{2}",
                                time[0].Replace("h", string.Empty),
                                time[1].Replace("m", string.Empty),
                                time[2].Replace("s", string.Empty)
                                );
                                break;
                            case 2:
                                timestring = string.Format("{0}:{1}:{2}",
                                "0",
                                time[0].Replace("m", string.Empty),
                                time[1].Replace("s", string.Empty)
                                );
                                break;
                            case 1:
                                timestring = string.Format("{0}:{1}:{2}",
                                "0",
                                "0",
                                time[0].Replace("s", string.Empty)
                                );
                                break;
                            default:
                                throw new Exception(string.Format("Unknown time string format: {0}", printTime));
                        }
                    }
                TimeSpan dt = TimeSpan.Parse(timestring);
                return Convert.ToDecimal(dt.TotalHours);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return -1m;
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return FileName;
        }
        #endregion
    }
    public enum SlicerParameter
    {
        Volume,
        PrintTime,
        PrintTimeSilent,
        FilamentUsed,
        FilamentDiameter,
        FilamentType,
        FilamentDensity,
        NozzleDiameter,
        
    }
    */
}
