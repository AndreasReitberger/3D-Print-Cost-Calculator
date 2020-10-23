using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using PrintCostCalculator3d.Models.GCode.Helper;
using PrintCostCalculator3d.Models.Slicer;
using PrintCostCalculator3d.Models.Slicer.Voxelizer;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Models.GCode
{
    public static class GCodeHelper
    {
        #region Properties
        private static GCode temp;
        private static SlicerPrinterConfiguration Config;
        public static TimeSpan Duration;

        #endregion

        #region Public
        public static async Task<GCode> FromFile(string file, SlicerPrinterConfiguration config, IProgress<int> prog, CancellationToken cancellationToken, bool useCommentRead = false)
        {
            try
            {
                Config = config;
                return await ParseAsync(new GCode(file), prog, cancellationToken, useCommentRead);
            }
            catch(Exception exc)
            {
                return null;
            }
        }
        public static async Task<GCode> FromGCodeFile(GCode file, SlicerPrinterConfiguration config, IProgress<int> prog, CancellationToken cancellationToken, bool useCommentRead = false)
        {
            try
            {
                Config = config;
                return await ParseAsync(file, prog, cancellationToken, useCommentRead);
            }
            catch(Exception exc)
            {
                return null;
            }
        }
        public static async Task<List<LinesVisual3D>> Create2dGcodeLayerModelListAsync(GCode gcode, IProgress<int> prog)
        {
            gcode.IsWorking = true;
            var list = new List<LinesVisual3D>();
            //var line = new LinesVisual3D();
            SortedDictionary<float, LinesVisual3D> gcodeLayers = new SortedDictionary<float, LinesVisual3D>();
            //ConcurrentBag<LinesVisual3D> layers = new ConcurrentBag<LinesVisual3D>();
            try
            {
                var Commands = gcode.Commands;
                await Task.Run(async () =>
                {
                    try
                    {
                        var temp = new List<LinesVisual3D>();
                        int i = 0;
                        foreach (List<GCodeCommand> commands in Commands)
                        {
                            float z = gcode.ZHeights.Keys.ElementAt(i);
                            var pointsPerLayer = getLayerPointsCollection(commands, z);
                            if (pointsPerLayer.Count > 0)
                            {
                                Application.Current.Dispatcher.Invoke((() =>
                                {
                                    gcodeLayers.Add(z, new LinesVisual3D() { Points = new Point3DCollection(pointsPerLayer) });
                                //layers.Add(new LinesVisual3D() { Points = new Point3DCollection(pointsPerLayer) });
                            }));
                            }
                            if (prog != null)
                            {
                                float test = (((float)i / Commands.Count) * 100f);
                                if (i < Commands.Count - 1)
                                    prog.Report(Convert.ToInt32(test));
                                else
                                    prog.Report(100);
                            }
                            i++;
                        }
                    }
                    catch(Exception exc)
                    {

                    }
                });
                /**/
                gcode.LayerModelGenerated = true;
                list = gcodeLayers.Select(pair => pair.Value).ToList();
                //list = layers.ToList();
            }
            catch (Exception exc)
            {
                
            }
            gcode.IsWorking = false;
            return list;
        }
        public static async Task<List<LineBuilder>> BuildGcodeLayerModelListAsync(GCode gcode, IProgress<int> prog)
        {
            gcode.IsWorking = true;
            var lineBuilders = new List<LineBuilder>();
            SortedDictionary<float, LineBuilder> gcodeLayers = new SortedDictionary<float, LineBuilder>();
            //ConcurrentBag<LinesVisual3D> layers = new ConcurrentBag<LinesVisual3D>();
            try
            {
                var Commands = gcode.Commands;
                await Task.Run(async () =>
                {
                    try
                    {
                        var temp = new List<LinesVisual3D>();
                        int i = 0;
                        foreach (List<GCodeCommand> commands in Commands)
                        {
                            float z = gcode.ZHeights.Keys.ElementAt(i);

                            LineBuilder builder = buildLineFromCommands(commands, z);
                            gcodeLayers.Add(z, builder);

                            if (prog != null)
                            {
                                float test = (((float)i / Commands.Count) * 100f);
                                if (i < Commands.Count - 1)
                                    prog.Report(Convert.ToInt32(test));
                                else
                                    prog.Report(100);
                            }
                            i++;
                        }
                    }
                    catch(Exception exc)
                    {

                    }
                });
                /**/
                gcode.LayerModelGenerated = true;
                lineBuilders = gcodeLayers.Select(pair => pair.Value).ToList();
                //list = layers.ToList();
            }
            catch (Exception exc)
            {
                
            }
            gcode.IsWorking = false;
            return lineBuilders;
        }
        public static async Task<List<LinesVisual3D>> Create3dGcodeLayerModelListAsync(GCode gcode, IProgress<int> prog)
        {
            gcode.IsWorking = true;
            var list = new List<LinesVisual3D>();
            //var line = new LinesVisual3D();
            SortedDictionary<float, LinesVisual3D> gcodeLayers = new SortedDictionary<float, LinesVisual3D>();
            //ConcurrentBag<LinesVisual3D> layers = new ConcurrentBag<LinesVisual3D>();
            try
            {
                var Commands = gcode.Commands;
                LinesVisual3D normalmoves = new LinesVisual3D();
                LinesVisual3D rapidmoves = new LinesVisual3D();
                LinesVisual3D wirebox = new LinesVisual3D();

                await Task.Run(async () =>
                {
                    var temp = new List<LinesVisual3D>();
                    int i = 0;
                    
                    foreach (List<GCodeCommand> commands in Commands)
                    {
                        float z = gcode.ZHeights.Keys.ElementAt(i); 
                        for (int j = 0; j < commands.Count; j++)
                        {
                            GCodeCommand cmd = commands[j];
                            if (cmd.Command != "g0" && cmd.Command != "g1" && cmd.Command != "g2" && cmd.Command != "g3")
                                continue;

                            float x_prev = !float.IsInfinity(cmd.PreviousX) ? cmd.PreviousX : 0;
                            float y_prev = !float.IsInfinity(cmd.PreviousY) ? cmd.PreviousY : 0;
                            float z_prev = !float.IsInfinity(cmd.PreviousZ) ? cmd.PreviousZ : 0;
                            float x = !float.IsInfinity(cmd.X) ? cmd.X : x_prev;
                            float y = !float.IsInfinity(cmd.Y) ? cmd.Y : y_prev;
                            //z = cmd.Z;

                            switch (cmd.Command)
                            {
                                case "g0":
                                    Application.Current.Dispatcher.Invoke((() =>
                                    {
                                        DrawLine(rapidmoves, x_prev, y_prev, z_prev, x, y, z);
                                    }));
                                    break;
                                case "g1":
                                    Application.Current.Dispatcher.Invoke((() =>
                                    {
                                        DrawLine(normalmoves, x_prev, y_prev, z_prev, x, y, z);
                                    }));
                                    break;
                                case "g2":
                                case "g3":
                                    bool clockwise = false;
                                    if (cmd.Command == "g2")
                                        clockwise = true;
                                    throw new Exception("Not supported!");
                                    //DrawArc(x_prev, y_prev, z_prev, x, y, z, j_pos, i_pos, false, clockwise);
                                    break;
                            }

                            x_prev = x;
                            y_prev = y;
                            z_prev = z;
                        }
                        Application.Current.Dispatcher.Invoke((() =>
                        {
                            //gcodeLayers.Add(z, new LinesVisual3D() { Points = new Point3DCollection(boundingBox) });
                            gcodeLayers.Add(z, normalmoves);
                            //layers.Add(new LinesVisual3D() { Points = new Point3DCollection(pointsCollection.Sel) });
                        }));
                        /*
                        float z =gcode.ZHeights.Keys.ElementAt(i);
                        var pointsPerLayer = getLayerPointsCollection(commands, z);
                        var sorted = pointsPerLayer.OrderBy(point => point.X);
                        var pointsPerXcoord = pointsPerLayer.GroupBy(point => point.X);

                        List<Point3D> boundingBox = new List<Point3D>();
                        foreach(var points in pointsPerXcoord)
                        {
                            if (points.Count() == 0) continue;
                            var min = points.Min(p => p.Y);
                            var max = points.Max(p => p.Y);
                            boundingBox.Add(new Point3D(points.Key, min, z));
                            boundingBox.Add(new Point3D(points.Key, max, z));
                        }
                        boundingBox = boundingBox.Distinct().ToList();
                        if (boundingBox.Count > 0)
                        {
                            SortedDictionary<int, Point3D> pointsCollection = new SortedDictionary<int, Point3D>();
                            foreach(Point3D point in boundingBox)
                            {
                                var index = pointsPerLayer.IndexOf(point);
                                if (index > -1)
                                {
                                    pointsCollection.Add(index, point);
                                }
                            }
                            pointsCollection.OrderBy(pair => pair.Key);

                            Application.Current.Dispatcher.Invoke((() =>
                            {
                                //gcodeLayers.Add(z, new LinesVisual3D() { Points = new Point3DCollection(boundingBox) });
                                gcodeLayers.Add(z, new LinesVisual3D() { Points = new Point3DCollection(pointsCollection.Select(pair => pair.Value).ToList()) });
                                //layers.Add(new LinesVisual3D() { Points = new Point3DCollection(pointsCollection.Sel) });
                            }));
                        }
                        */
                        if (prog != null)
                        {
                            float test = (((float)i / Commands.Count) * 100f);
                            if (i < Commands.Count - 1)
                                prog.Report(Convert.ToInt32(test));
                            else
                                prog.Report(100);
                        }
                        i++;
                    }
                });
                /**/
                //gcode.LayerModelGenerated = true;
                //list = gcodeLayers.Select(pair => pair.Value).ToList();
                //list = layers.ToList();
                list = new List<LinesVisual3D>();
                list.Add(normalmoves);
            }
            catch (Exception exc)
            {
                
            }
            gcode.IsWorking = false;
            return list;
        }
        #endregion

        #region Private
        private static async Task<GCode> ParseAsync(GCode gcode, IProgress<int> prog, CancellationToken cancellationToken, bool useCommentRead)
        {
            try
            {
                gcode.IsWorking = true;
                TimeSpan start = DateTime.Now.TimeOfDay;
                var progress = prog;

                gcode.IsValid = false;

                var lines = await ReadFileAsync(gcode, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return null;

                gcode.SlicerName = getSlicerNameFromLines(lines.Take(20).ToList());
                var slicers = Slicer.Slicer.SupportedSlicers.FirstOrDefault(slicer => slicer.SlicerName == gcode.SlicerName);

                if (lines != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return null;
                    var model = await DoParseAsync(lines, cancellationToken);
                    if (model != null)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return null;
                        var result  = await Task.Run(() => AnalyzeAsync(model, progress, cancellationToken));
                        if(result != null)
                        {
                            model = result.Model;
                            gcode.Lines = lines.Count;

                            gcode.Commands = model.Commands;
                            gcode.PrintTime = Math.Round(model.PrintTime / 3600, 2);

                            gcode.Layers = model.Layers;
                            gcode.ZHeights = model.zHeights;
                            gcode.Width = model.Width;
                            gcode.Height = model.Height;
                            gcode.Depth = model.Depth;

                            gcode.FilamentUsed = Math.Round(model.TotalFilament, 2);
                            gcode.ExtrudedFilamentVolume = Math.Round((((Math.PI * gcode.Diameter * gcode.Diameter) / 4f) * gcode.FilamentUsed / 1000f), 2);
                            gcode.IsValid = true;
                            if (useCommentRead && slicers != null)
                            {
                                try
                                {
                                    var parameter = getParameterFromSlicer(gcode.SlicerName, SlicerParameter.Volume, lines);
                                    if (parameter != "unkown_parameter")
                                    {
                                        var value = Convert.ToDouble(parameter, CultureInfo.GetCultureInfo("en-US"));
                                        if (value != -1)
                                            gcode.ExtrudedFilamentVolume = Math.Round(value, 2);
                                    }
                                }
                                catch (Exception)
                                {
                                    gcode.ExtrudedFilamentVolume = 0;
                                }
                                // PrintTime
                                try
                                {
                                    var parameter = getParameterFromSlicer(gcode.SlicerName, SlicerParameter.PrintTime, lines);
                                    if (parameter != "unkown_parameter")
                                    {
                                        var value = Convert.ToDouble(convertPrintTimeToDec(
                                            parameter), CultureInfo.GetCultureInfo("en-US"));
                                        if (value != -1)
                                            gcode.PrintTime = Math.Round(value, 2);
                                    }
                                }
                                catch (Exception)
                                {
                                    gcode.PrintTime = 0;
                                }
                                // Filament used [mm]
                                try
                                {
                                    var parameter = getParameterFromSlicer(gcode.SlicerName, SlicerParameter.FilamentUsed, lines);
                                    if (parameter != "unkown_parameter")
                                    {
                                        var value = Convert.ToDouble(parameter, CultureInfo.GetCultureInfo("en-US"));
                                        if (value != -1)
                                            gcode.FilamentUsed = Math.Round(value, 2);
                                    }
                                }
                                catch (Exception)
                                {
                                    gcode.FilamentUsed = 0;
                                }
                                // Filament Diameter
                                try
                                {
                                    var parameter = getParameterFromSlicer(gcode.SlicerName, SlicerParameter.FilamentDiameter, lines);
                                    if (parameter != "unkown_parameter")
                                    {
                                        var value = Convert.ToDouble(parameter, CultureInfo.GetCultureInfo("en-US"));
                                        if (value != -1)
                                            gcode.Diameter = Convert.ToDouble(getParameterFromSlicer(gcode.SlicerName, SlicerParameter.FilamentDiameter, lines), CultureInfo.GetCultureInfo("en-US"));
                                    }
                                }
                                catch (Exception)
                                {
                                    gcode.Diameter = 0;
                                }
                                // Nozzle Diameter
                                try
                                {
                                    var parameter = getParameterFromSlicer(gcode.SlicerName, SlicerParameter.NozzleDiameter, lines);
                                    if (parameter != "unkown_parameter")
                                    {
                                        var value = Convert.ToDouble(parameter, CultureInfo.GetCultureInfo("en-US"));
                                        if (value != -1)
                                            gcode.NozzleDiameter = value;
                                    }
                                }
                                catch (Exception)
                                {
                                    gcode.NozzleDiameter = 0;
                                }
                                // FilamentType
                                try
                                {
                                    var parameter = getParameterFromSlicer(gcode.SlicerName, SlicerParameter.FilamentType, lines);
                                    if (parameter != "unkown_parameter")
                                    {
                                        gcode.FilamentType = parameter;
                                    }
                                    else
                                        gcode.FilamentType = Strings.Unknown;
                                }
                                catch (Exception)
                                {
                                    gcode.FilamentType = Strings.Unknown;
                                }
                                // FilamentDensity
                                try
                                {
                                    var parameter = getParameterFromSlicer(gcode.SlicerName, SlicerParameter.FilamentDensity, lines);
                                    if (parameter != "unkown_parameter")
                                    {
                                        var value = Convert.ToDouble(parameter, CultureInfo.GetCultureInfo("en-US"));
                                        if (value != -1)
                                            gcode.FilamentDensity = value;
                                    }
                                }
                                catch (Exception)
                                {
                                    gcode.FilamentDensity = 0;
                                }
                                gcode.IsValid = true;
                            }

                        }
                        TimeSpan end = DateTime.Now.TimeOfDay;
                        gcode.ParsingDuration = Duration = end - start;
                        
                    }
                }
                gcode.IsWorking = false;
                return gcode;
            }
            catch (Exception exc)
            {
                gcode.IsWorking = false;
                return null;
            }
        }
        private static async Task<List<string>> ReadFileAsync(string Filename, CancellationToken cancellationToken)
        {
            try
            {
                var Lines = new List<string>();
                var Gcode = new List<string>();

                using (StreamReader reader = new StreamReader(Filename))
                {
                    string lines = await reader.ReadToEndAsync();
                    Lines = lines.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
                    var linesArray = Lines.ToArray();
                    for (int i = 0; i < linesArray.Count(); i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return null;

                        if (Regex.IsMatch(linesArray[i], @"/^(G0|G1|G90|G91|G92|M82|M83|G28)/i"))
                        {
                            Gcode.Add(linesArray[i]);
                        }
                    }
                    return Lines;
                }
            }
            catch (Exception exc)
            {
                return new List<string>();
            }
        }
        private static async Task<List<string>> ReadFileAsync(GCode Gcode, CancellationToken cancellationToken)
        {
            try
            {
                return await ReadFileAsync(Gcode.FilePath, cancellationToken);
            }
            catch (Exception exc)
            {
                return new List<string>();
            }
        }
        private static async Task<GCodeModel> DoParseAsync(List<string> GcodeLines, CancellationToken cancellationToken)
        {
            try
            {
                GCodeModel Model = new GCodeModel();
                int layer = 0;
                float X = 0;
                float Y = 0;
                float Z = 0;
                float previousX = float.NegativeInfinity;
                float previousY = float.NegativeInfinity;
                float previousZ = float.NegativeInfinity;

                bool extruding = false;
                bool extrudeRelative = false;
                char extruder;

                string sExtruder = "";
                int retract = 0;

                float extrusion;
                Dictionary<string, float> previousExtrusion = new Dictionary<string, float>()
            {
                {"a", 0 },
                {"b", 0 },
                {"c", 0 },
                {"e", 0 },
                {"abs", 0 },
            };
                Dictionary<string, float> previousRetraction = new Dictionary<string, float>()
            {
                {"a", 0 },
                {"b", 0 },
                {"c", 0 },
                {"e", 0 },
            };
                // "Last" represents the last time the print head speed was requested to be changed
                float lastSpeed = 4000;

                bool dcExtrude = false;
                bool assumeNonDC = false;

                float volumePerMM = 0;

                for (int i = 0; i < GcodeLines.Count; i++)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return null;
                        

                        string gcode = GcodeLines[i];
                        var test = Regex.Split(gcode, @"\;");
                        gcode = Regex.Split(gcode, @"\;")[0].TrimEnd();
                        //string[] args = gcode.ToLower().TrimEnd().Split(' ');
                        string[] args = gcode.ToLower().TrimEnd().Split(' ');

                        X = float.NegativeInfinity;
                        Y = float.NegativeInfinity;
                        Z = float.NegativeInfinity;
                        //Z = 0;// float.NegativeInfinity;
                        volumePerMM = 0;
                        retract = 0;

                        extruding = false;
                        sExtruder = "";
                        previousExtrusion["abs"] = 0;


                        if (args[0] == "g0" || args[0] == "g1")
                        {
                            for (int j = 1; j < args.Length; j++)
                            {
                                string arg = args[j];
                                if (string.IsNullOrEmpty(arg))
                                    continue;
                                switch (arg[0])
                                {
                                    case 'x':
                                        X = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        break;
                                    case 'y':
                                        Y = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        break;
                                    case 'z':
                                        Z = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        if (Z == previousZ)
                                        {
                                            continue;
                                        }
                                        if (Model.zHeights.ContainsKey(Z))
                                        {
                                            layer = Model.zHeights[Z];
                                        }
                                        else
                                        {
                                            layer = Model.Commands.Count;
                                            Model.zHeights[Z] = layer;
                                        }
                                        //previousZ = Z;
                                        break;
                                    case 'e':
                                    case 'a':
                                    case 'b':
                                    case 'c':
                                        assumeNonDC = true;
                                        // These 4 cases appear to map to different extruders
                                        //extruder = arg[0];
                                        sExtruder = arg[0].ToString();
                                        var t = arg.TrimStart(arg[0]);
                                        extrusion = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        if (!extrudeRelative)
                                        {
                                            // Absolute positioning
                                            previousExtrusion["abs"] = extrusion - previousExtrusion[sExtruder];
                                        }
                                        else
                                        {
                                            previousExtrusion["abs"] = extrusion;
                                        }

                                        extruding = previousExtrusion["abs"] > 0;

                                        if (previousExtrusion["abs"] < 0)
                                        {
                                            //We're retracting...
                                            previousRetraction[sExtruder] = -1;
                                            retract = -1;
                                        }
                                        else if (previousExtrusion["abs"] == 0)
                                        {
                                            retract = 0;
                                        }
                                        else if (previousExtrusion["abs"] > 0 && previousRetraction[sExtruder] < 0)
                                        {
                                            previousRetraction[sExtruder] = 0;
                                            retract = 1;
                                        }
                                        else
                                        {
                                            retract = 0;
                                        }

                                        //previousExtrusion[sExtruder] = extrusion;
                                        previousExtrusion[sExtruder] = extrusion;
                                        break;
                                    case 'f':
                                        extrusion = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        lastSpeed = extrusion;
                                        break;
                                    default:
                                        break;
                                }

                            }

                            if (dcExtrude && !assumeNonDC)
                            {
                                extruding = true;
                                previousExtrusion["abs"] = (float)Math.Sqrt((previousX - X) * (previousX - X) + (previousY - Y) * (previousY - Y));
                            }

                            if (extruding && retract == 0)
                            {

                                // 
                                volumePerMM = previousExtrusion["abs"] / (float)Math.Sqrt(
                                    ((double)(previousX - X)) * ((double)(previousX - X)) +
                                    ((double)(previousY - Y)) * ((double)(previousY - Y))

                                    );
                            }

                            if (Model.Commands.Count == 0 || Model.Commands.Count - 1 < layer || Model.Commands[layer] == null)
                            {
                                Model.Commands.Add(new List<GCodeCommand>());
                            }

                            // {x: Number(x), y: Number(y), z: Number(z), extrude: extrude, retract: Number(retract), noMove: false, 
                            // extrusion: (extrude||retract)?Number(prev_extrude["abs"]):0, extruder: extruder, prevX: Number(prevX), 
                            // prevY: Number(prevY), prevZ: Number(prevZ), speed: Number(lastF), gcodeLine: Number(i), 
                            // volPerMM: typeof(volPerMM)==='undefined'?-1:volPerMM};
                            if (float.IsNegativeInfinity(previousZ)) previousZ = 0;
                            GCodeCommand command = new GCodeCommand()
                            {
                                OriginalLine = gcode,
                                Command = args[0],
                                X = X,
                                Y = Y,
                                Z = Z,
                                Speed = lastSpeed,
                                Extrude = extruding,
                                Retract = retract,
                                NoMove = false,
                                Extrusion = (extruding || retract != 0) ? previousExtrusion["abs"] : 0,
                                Extruder = sExtruder,
                                PreviousX = previousX,
                                PreviousY = previousY,
                                PreviousZ = previousZ,
                                VolumePerMM = float.IsNaN(volumePerMM) || float.IsInfinity(volumePerMM) ? -1 : volumePerMM,
                                GCodeLine = i,
                            };
                            Model.Commands[layer].Add(command);

                            if (!float.IsNegativeInfinity(X)) previousX = X;
                            if (!float.IsNegativeInfinity(Y)) previousY = Y;
                            previousZ = Z;
                        }
                        else if (args[0] == "m82")
                        {
                            extrudeRelative = false;
                        }
                        else if (args[0] == "g91")
                        {
                            extrudeRelative = true;
                        }
                        else if (args[0] == "g90")
                        {
                            extrudeRelative = false;
                        }
                        else if (args[0] == "m83")
                        {
                            extrudeRelative = true;
                            /*
                            if (!previousG90Found)
                                extrudeRelative = true;
                            else
                            {
                                extrudeRelative = false;
                                previousG90Found = false;
                            }
                            */
                        }
                        else if (args[0] == "m101")
                        {
                            dcExtrude = true;
                            //throw new NotImplementedException();
                        }
                        else if (args[0] == "M103")
                        {
                            dcExtrude = false;
                            //throw new NotImplementedException();
                        }

                        else if (args[0] == "g92")
                        {
                            for (int j = 1; j < args.Length; j++)
                            {
                                string arg = args[j];
                                if (string.IsNullOrEmpty(arg))
                                    continue;
                                switch (arg[0])
                                {
                                    case 'x':
                                        X = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        break;
                                    case 'y':
                                        Y = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        break;
                                    case 'z':
                                        Z = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        previousZ = Z;
                                        break;
                                    case 'e':
                                    case 'a':
                                    case 'b':
                                    case 'c':
                                        // These 4 cases appear to map to different extruders
                                        extruder = arg[0];
                                        sExtruder = arg[0].ToString();
                                        var t = arg.TrimStart(arg[0]);
                                        extrusion = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        if (!extrudeRelative)
                                        {
                                            // Absolute positioning
                                            previousExtrusion[sExtruder] = 0;
                                        }
                                        else
                                        {
                                            previousExtrusion[sExtruder] = extrusion;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                // if(typeof(x) !== 'undefined' || typeof(y) !== 'undefined' ||typeof(z) !== 'undefined')
                                if (!float.IsNegativeInfinity(X) || !float.IsNegativeInfinity(Y) || !float.IsNegativeInfinity(Z))
                                {
                                    if (Model.Commands.Count == 0 || Model.Commands.Count - 1 < layer || Model.Commands[layer] == null)
                                    {
                                        Model.Commands.Add(new List<GCodeCommand>());
                                    }
                                    // {x: parseFloat(x), y: parseFloat(y), z: parseFloat(z), extrude: extrude, retract: parseFloat(retract), noMove: true, 
                                    // extrusion: 0, extruder: extruder, prevX: parseFloat(prevX), prevY: parseFloat(prevY), prevZ: parseFloat(prevZ), speed: parseFloat(lastF), 
                                    // gcodeLine: parseFloat(i)};
                                    GCodeCommand command = new GCodeCommand()
                                    {
                                        Command = args[0],
                                        OriginalLine = gcode,
                                        X = X,
                                        Y = Y,
                                        Z = Z,
                                        Speed = lastSpeed,
                                        Extrude = extruding,
                                        Retract = retract,
                                        NoMove = true,
                                        Extrusion = 0,
                                        Extruder = sExtruder,
                                        PreviousX = previousX,
                                        PreviousY = previousY,
                                        PreviousZ = previousZ,
                                        //VolumePerMM = float.IsNaN(volumePerMM) || float.IsInfinity(volumePerMM) ? -1 : volumePerMM,
                                        GCodeLine = i,
                                    };
                                    Model.Commands[layer].Add(command);
                                }
                            }
                        }
                        else if (args[0] == "g28")
                        {
                            for (int j = 1; j < args.Length; j++)
                            {
                                string arg = args[j];
                                if (string.IsNullOrEmpty(arg))
                                    continue;
                                switch (arg[0])
                                {
                                    case 'x':
                                        //X = float.Parse(arg.TrimStart('x'));
                                        X = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        break;
                                    case 'y':
                                        Y = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        break;
                                    case 'z':
                                        Z = float.Parse(arg.TrimStart(arg[0]), CultureInfo.InvariantCulture);
                                        if (Z == previousZ)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            layer = Model.Commands.Count;
                                        }
                                        previousZ = Z;
                                        break;
                                    case 'e':
                                    case 'a':
                                    case 'b':
                                    case 'c':

                                        break;
                                    default:
                                        break;
                                }
                            }
                            // G28 with no arguments
                            if (args.Length == 1)
                            {
                                //need to init values to default here
                            }
                            // if it's the first layer and G28 was without
                            if (layer == 0 && float.IsNegativeInfinity(Z))
                            {
                                Z = 0;
                                if (Model.zHeights.ContainsKey(Z))
                                {
                                    layer = Model.zHeights[Z];
                                }
                                else
                                {
                                    layer = Model.Commands.Count;
                                    Model.zHeights[Z] = layer;
                                }
                                previousZ = Z;
                            }
                            if (Model.Commands.Count == 0 || Model.Commands.Count - 1 < layer || Model.Commands[layer] == null)
                            {
                                Model.Commands.Add(new List<GCodeCommand>());
                            }
                            // model[layer][model[layer].length] = {x: Number(x), y: Number(y), z: Number(z), extrude: extrude, retract: Number(retract), noMove: false, 
                            // extrusion: (extrude||retract)?Number(prev_extrude["abs"]):0, 
                            // extruder: extruder, prevX: Number(prevX), prevY: Number(prevY), prevZ: Number(prevZ), speed: Number(lastF), gcodeLine: Number(i)};
                            GCodeCommand command = new GCodeCommand()
                            {
                                Command = args[0],
                                OriginalLine = gcode,
                                X = X,
                                Y = Y,
                                Z = Z,
                                Speed = lastSpeed,
                                Extrude = extruding,
                                Retract = retract,
                                NoMove = false,
                                Extrusion = (extruding || retract != 0) ? previousExtrusion["abs"] : 0,
                                Extruder = sExtruder,
                                PreviousX = previousX,
                                PreviousY = previousY,
                                PreviousZ = previousZ,
                                //VolumePerMM = float.IsNaN(volumePerMM) || float.IsInfinity(volumePerMM) ? -1 : volumePerMM,
                                GCodeLine = i,
                            };
                            Model.Commands[layer].Add(command);
                        }
                    }
                    catch (Exception exc)
                    {
                        continue;
                    }
                }

                Model.Layers = layer;
                return Model;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        private static async Task<GCodeProcessResult> AnalyzeAsync(GCodeModel Model, IProgress<int> progress, CancellationToken cancellationToken)
        {
            try
            {
                //GCodeObjectSize min = new GCodeObjectSize() { X = float.NegativeInfinity, Y = float.NegativeInfinity, Z = float.NegativeInfinity };
                //GCodeObjectSize max = new GCodeObjectSize() { X = float.NegativeInfinity, Y = float.NegativeInfinity, Z = float.NegativeInfinity };
                GCodeProcessResult gcodeResult = new GCodeProcessResult();

                var cmds = Model.Commands.Where(cmd => cmd.Count > 0).ToList();
                long totalCount = cmds.SelectMany(cmd => cmd).ToList().Count;
                var rangePartitioner = Partitioner.Create(0, totalCount);


                ConcurrentBag<GCodeProcessResult> Results = new ConcurrentBag<GCodeProcessResult>();

                for (int i = 0; i < cmds.Count; i++)
                {
                    var cmd = cmds[i];
                    //https://stackoverflow.com/questions/6977218/parallel-foreach-can-cause-a-out-of-memory-exception-if-working-with-a-enumera
                    foreach (var command in cmd)
                    {
                        //var command = cmd[j];
                        var result = await ProcessSingleGcodeCommandAsync(command, cancellationToken);
                        Results.Add(result);
                        result = null;
                    }

                    if (progress != null)
                    {
                        try
                        {
                            float test = (((float)i / cmds.Count) * 100f);
                            if (i < cmds.Count - 1)
                                progress.Report(Convert.ToInt32(test));
                            else
                                progress.Report(100);
                        }

                        catch (Exception exc)
                        {

                        }
                    }
                }

                gcodeResult.PrintTimeAddition = Results.Sum(model => model.Model.PrintTime);
                gcodeResult.TotalFilament = Results.Sum(model => model.Model.TotalFilament);
                Model.PrintTime = Results.Sum(model => model.Model.PrintTime);
                Model.TotalFilament = Results.Sum(model => model.Model.TotalFilament);

                gcodeResult.MaxX = Results.Max(model => model.MaxX);
                gcodeResult.MaxY = Results.Max(model => model.MaxY);
                gcodeResult.MaxZ = Results.Max(model => model.MaxZ);

                gcodeResult.MinX = Results.Min(model => model.MinX);
                gcodeResult.MinY = Results.Min(model => model.MinY);
                gcodeResult.MinZ = Results.Min(model => model.MinZ);

                Model.Width = Math.Abs(gcodeResult.MaxX - gcodeResult.MinX);
                Model.Depth = Math.Abs(gcodeResult.MaxY - gcodeResult.MinY);
                Model.Height = Math.Abs(gcodeResult.MaxZ - gcodeResult.MinZ);
                Model.LayerHeight = (gcodeResult.MaxZ - gcodeResult.MinZ) / (Model.Layers - 1);

                gcodeResult.Model = Model;

                // Clear memory
                cmds.Clear();
                //Results.Clear();
                cmds = null;
                Results = null;

                return gcodeResult;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        private static async Task<GCodeProcessResult> AnalyzeParallelAsync(GCodeModel Model, IProgress<int> progress, CancellationToken cancellationToken)
        {
            try
            {
                //GCodeObjectSize min = new GCodeObjectSize() { X = float.NegativeInfinity, Y = float.NegativeInfinity, Z = float.NegativeInfinity };
                //GCodeObjectSize max = new GCodeObjectSize() { X = float.NegativeInfinity, Y = float.NegativeInfinity, Z = float.NegativeInfinity };
                GCodeProcessResult gcodeResult = new GCodeProcessResult();

                var cmds = Model.Commands.Where(cmd => cmd.Count > 0).ToList();
                long totalCount = cmds.SelectMany(cmd => cmd).ToList().Count;
                var rangePartitioner = Partitioner.Create(0, totalCount);


                ConcurrentBag<GCodeProcessResult> Results = new ConcurrentBag<GCodeProcessResult>();

                long i = 0;
                /**/
                Parallel.ForEach(cmds, async (cmd) =>
                {
                    //https://stackoverflow.com/questions/6977218/parallel-foreach-can-cause-a-out-of-memory-exception-if-working-with-a-enumera
                    Parallel.ForEach(cmd, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (command) =>
                    {
                        var result = await ProcessSingleGcodeCommandAsync(command, cancellationToken);
                        Results.Add(result);
                        result = null;
                    });
                    if (progress != null)
                    {
                        try
                        {
                            float test = (((float)i / cmds.Count) * 100f);
                            if (i < cmds.Count - 1)
                                progress.Report(Convert.ToInt32(test));
                            else
                                progress.Report(100);
                        }

                        catch (Exception exc)
                        {

                        }
                    }
                    i++;
                });

                gcodeResult.PrintTimeAddition = Results.Sum(model => model.Model.PrintTime);
                gcodeResult.TotalFilament = Results.Sum(model => model.Model.TotalFilament);

                gcodeResult.MaxX = Results.Max(model => model.MaxX);
                gcodeResult.MaxY = Results.Max(model => model.MaxY);
                gcodeResult.MaxZ = Results.Max(model => model.MaxZ);

                gcodeResult.MinX = Results.Min(model => model.MinX);
                gcodeResult.MinY = Results.Min(model => model.MinY);
                gcodeResult.MinZ = Results.Min(model => model.MinZ);

                Model.Width = Math.Abs(gcodeResult.MaxX - gcodeResult.MinX);
                Model.Depth = Math.Abs(gcodeResult.MaxY - gcodeResult.MinY);
                Model.Height = Math.Abs(gcodeResult.MaxZ - gcodeResult.MinZ);
                Model.LayerHeight = (gcodeResult.MaxZ - gcodeResult.MinZ) / (Model.Layers - 1);

                gcodeResult.Model = Model;
                // Clear memory
                cmds.Clear();
                //Results.Clear();
                cmds = null;
                Results = null;

                return gcodeResult;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        private static async Task<GCodeProcessResult> ProcessSingleGcodeCommandAsync(GCodeCommand GcodeCommand, CancellationToken cancellationToken)
        {
            GCodeProcessResult result = new GCodeProcessResult();
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                float tmp1 = 0;
                float tmp2 = 0;
                float speedDivider = 60 * Config.PrintDurationCorrection;

                GCodeCommand command = GcodeCommand;
                result.ValidX = false;
                result.ValidY = false;
                result.LastSpeed = command.Speed;
                // if(typeof(cmds[j].x) !== 'undefined'&&typeof(cmds[j].prevX) !== 'undefined'&&typeof(cmds[j].extrude) !== 'undefined'&&cmds[j].extrude&&!isNaN(cmds[j].x))
                if (!float.IsInfinity(command.X) && !float.IsInfinity(command.PreviousX) && !float.IsNaN(command.X))
                {
                    if (command.Extrude)
                    {
                        result.MaxX = (result.MaxX > command.X) ? result.MaxX : command.X;
                        result.MaxX = (result.MaxX > command.PreviousX) ? result.MaxX : command.PreviousX;
                        result.MinX = (result.MinX < command.X) ? result.MinX : command.X;
                        result.MinX = (result.MinX < command.PreviousX) ? result.MinX : command.PreviousX;
                        result.ValidX = true;
                    }
                    // Calculate travel speed
                }

                if (!float.IsInfinity(command.Y) && !float.IsInfinity(command.PreviousY) && !float.IsNaN(command.Y))
                {
                    if (command.Extrude)
                    {
                        result.MaxY = (result.MaxY > command.Y) ? result.MaxY : command.Y;
                        result.MaxY = (result.MaxY > command.PreviousY) ? result.MaxY : command.PreviousY;
                        result.MinY = (result.MinY < command.Y) ? result.MinY : command.Y;
                        result.MinY = (result.MinY < command.PreviousY) ? result.MinY : command.PreviousY;
                        result.ValidY = true;
                    }

                }

                if (!float.IsInfinity(command.Z) && !float.IsInfinity(command.PreviousZ) && !command.Extrude && !float.IsNaN(command.Z))
                {
                    // Calculate time for z movements
                    result.ValidZ = true;
                    var s = (float)(command.Z - command.PreviousZ);
                    var temp = CalculateTime(s, command.Speed / speedDivider, Config.AMax_z);
                    //var temp = s / command.Speed / speedDivider;
                    if (float.IsNaN(temp) || float.IsInfinity(temp))
                    {

                    }
                    else
                    {
                        result.PrintTimeAddition += temp;
                    }
                }
                if ((result.ValidX && !result.ValidY) || (result.ValidY && !result.ValidX))
                {

                }
                if (!float.IsInfinity(command.PreviousZ) && command.Extrude && !float.IsNaN(command.PreviousZ))
                {
                    result.MaxZ = (result.MaxZ > command.PreviousZ) ? result.MaxZ : command.PreviousZ;
                    result.MinZ = (result.MinZ < command.PreviousZ) ? result.MinZ : command.PreviousZ;

                }

                if (command.Extrude || command.Retract != 0)
                {
                    //Model.TotalFilament += command.Extrusion;
                    result.Model.TotalFilament = command.Extrusion;
                }
                // Calculate time for X, Y movements (with and without extrusion)
                if (result.ValidX && result.ValidY)
                {
                    //  printTimeAdd = Math.sqrt(Math.pow(parseFloat(cmds[j].x)-parseFloat(cmds[j].prevX),2)+Math.pow(parseFloat(cmds[j].y)-parseFloat(cmds[j].prevY),2))/(cmds[j].speed/60);
                    //var temp = (float)Math.Sqrt(Math.Pow((command.X) - (command.PreviousX), 2) + Math.Pow((command.Y) - (command.PreviousY), 2)) / (command.Speed / speedDivider);
                    // distance to travel in mm
                    var s = (float)Math.Sqrt(Math.Pow((command.X) - (command.PreviousX), 2) + Math.Pow((command.Y) - (command.PreviousY), 2));
                    /*
                    var tAcceleration = (command.Speed) / speedDivider / aMax_xy;
                    var sAcceleration = (float)((aMax_xy / 2) * Math.Sqrt(tAcceleration));

                    var tConstant = (float)((s - sAcceleration) / (command.Speed / speedDivider));
                    temp = tConstant > 0 ? tAcceleration + tConstant : (float)Math.Sqrt(2 * s / aMax_xy) ;
                    */
                    var temp = CalculateTime(s, command.Speed / speedDivider, Config.AMax_xy);
                    if (float.IsNaN(temp) || float.IsInfinity(temp))
                    {

                    }
                    else
                        result.PrintTimeAddition += temp; // (float)Math.Sqrt(Math.Pow((command.X) - (command.PreviousX), 2) + Math.Pow((command.Y) - (command.PreviousY), 2)) / (command.Speed / 60);
                }
                // Calculate time for extrusion
                else if (command.Retract == 0 && command.Extrusion != 0)
                {
                    // tmp1 = Math.sqrt(Math.pow(parseFloat(cmds[j].x)-parseFloat(cmds[j].prevX),2)+Math.pow(parseFloat(cmds[j].y)-parseFloat(cmds[j].prevY),2))/(cmds[j].speed/60);
                    //tmp1 = (float)Math.Sqrt(Math.Pow((command.X) - (command.PreviousX), 2) + Math.Pow((command.Y) - (command.PreviousY), 2)) / (command.Speed / speedDivider);
                    var s = (float)Math.Sqrt(Math.Pow((command.X) - (command.PreviousX), 2) + Math.Pow((command.Y) - (command.PreviousY), 2));
                    tmp1 = CalculateTime(s, command.Speed / speedDivider, Config.AMax_e);
                    //tmp2 = (float)Math.Abs((command.Extrusion) / (command.Speed / speedDivider));
                    tmp2 = CalculateTime(Math.Abs(command.Extrusion), command.Speed / speedDivider, Config.AMax_eExtrude);
                    // If both value are invalid
                    if ((float.IsNaN(tmp1) || float.IsInfinity(tmp1)) && (float.IsNaN(tmp2) || float.IsInfinity(tmp2)))
                    {

                    }
                    else
                    {
                        // If movement is invalid
                        if (float.IsNaN(tmp1) || float.IsInfinity(tmp1))
                        {
                            result.PrintTimeAddition += tmp2;
                        }
                        // If extrusion is invalid
                        else if (float.IsNaN(tmp2) || float.IsInfinity(tmp2))
                        {
                            result.PrintTimeAddition += tmp1;
                        }
                        // If both valid, take the one who takes longer.
                        else
                            result.PrintTimeAddition += tmp1 > tmp2 ? tmp1 : tmp2;
                    }

                }
                // Calculate time for retract
                else if (command.Retract != 0)
                {
                    // printTimeAdd = Math.abs(parseFloat(cmds[j].extrusion)/(cmds[j].speed/60));
                    //tmp1 = (float)Math.Sqrt(Math.Pow((command.X) - (command.PreviousX), 2) + Math.Pow((command.Y) - (command.PreviousY), 2)) / (command.Speed / speedDivider);
                    var s = (float)Math.Sqrt(Math.Pow((command.X) - (command.PreviousX), 2) + Math.Pow((command.Y) - (command.PreviousY), 2));
                    tmp1 = CalculateTime(s, command.Speed / speedDivider, Config.AMax_e);
                    //tmp2 = Math.Abs(command.Extrusion / (command.Speed / speedDivider));
                    tmp2 = CalculateTime(Math.Abs(command.Extrusion), command.Speed / speedDivider, Config.AMax_eRetract);
                    // If both value are invalid
                    if ((float.IsNaN(tmp1) || float.IsInfinity(tmp1)) && (float.IsNaN(tmp2) || float.IsInfinity(tmp2)))
                    {

                    }
                    else
                    {
                        // If movement is invalid
                        if (float.IsNaN(tmp1) || float.IsInfinity(tmp1))
                        {
                            result.PrintTimeAddition += tmp2;
                        }
                        // If extrusion is invalid
                        else if (float.IsNaN(tmp2) || float.IsInfinity(tmp2))
                        {
                            result.PrintTimeAddition += tmp1;
                        }
                        // If both valid, take the one who takes longer.
                        else
                            result.PrintTimeAddition += tmp1 > tmp2 ? tmp1 : tmp2;
                    }
                }

                //Model.PrintTime += result.PrintTimeAddition;
                result.Model.PrintTime += result.PrintTimeAddition;

                if (command.Extrude && command.Retract == 0 && result.ValidX && result.ValidY)
                {
                    /*
                    // we are extruding
                    var volPerMM = command.VolumePerMM;

                    //volPerMM = float.Parse(volPerMM);

                    var volIndex = Model.volSpeeds.IndexOf(volPerMM);
                    if (volIndex == -1)
                    {
                        result.Model.volSpeeds.Add(volPerMM);
                        volIndex = result.Model.volSpeeds.IndexOf(volPerMM);
                    }
                    //if (!Model.volSpeedsByLayer.ContainsKey(command.PreviousZ) )
                    //var t = Model.volSpeedsByLayer[command.PreviousZ];
                    if (!result.Model.volSpeedsByLayer.ContainsKey(command.PreviousZ) || result.Model.volSpeedsByLayer[command.PreviousZ] == null)
                    {
                        result.Model.volSpeedsByLayer.Add(command.PreviousZ, new List<float>() { -1 });
                    }
                    if (result.Model.volSpeedsByLayer[command.PreviousZ].IndexOf(volPerMM) == -1)
                    {
                        //Model.volSpeedsByLayer[command.PreviousZ][volIndex] = volPerMM;
                        result.Model.volSpeedsByLayer[command.PreviousZ].Add(volPerMM);
                    }

                    var extrusionSpeed = command.VolumePerMM * (command.Speed / 60);

                    volIndex = result.Model.extrusionSpeeds.IndexOf(extrusionSpeed);
                    if (volIndex == -1)
                    {
                        result.Model.extrusionSpeeds.Add(extrusionSpeed);
                        volIndex = result.Model.extrusionSpeeds.IndexOf(extrusionSpeed);
                    }
                    if (!result.Model.extrusionSpeedsByLayer.ContainsKey(command.PreviousZ) || result.Model.extrusionSpeedsByLayer[command.PreviousZ] == null)
                    {
                        result.Model.extrusionSpeedsByLayer.Add(command.PreviousZ, new List<float>() { -1 });
                    }
                    if (result.Model.extrusionSpeedsByLayer[command.PreviousZ].IndexOf(extrusionSpeed) == -1)
                    {
                        //Model.extrusionSpeedsByLayer[command.PreviousZ][volIndex] = extrusionSpeed;
                        result.Model.extrusionSpeedsByLayer[command.PreviousZ].Add(extrusionSpeed);
                    }
                    */
                }


            }
            catch (Exception exc)
            {

            }
            return result;
        }
        private static float CalculateTime(float distance, float speed, float maxAcceleration)
        {
            float temp = 0;
            if (distance == 0) return temp;
            if (speed > 200)
            {

            }
            var tAcceleration = speed / maxAcceleration;
            var sAcceleration = (float)((maxAcceleration / 2) * Math.Pow(tAcceleration, 2));

            var tConstant = (float)((distance - sAcceleration) / speed);
            temp = tConstant > 0 ? tAcceleration + tConstant : (float)Math.Sqrt(2 * distance / maxAcceleration);
            return temp;
        }
        private static SlicerName getSlicerName(string line)
        {
            try
            {
                string slicerLine = line;
                if (slicerLine.Contains("Slic3r"))
                    return SlicerName.Slic3r;
                else if (slicerLine.Contains("PrusaSlicer"))
                    return SlicerName.PrusaSlicer;
                else if (slicerLine.Contains("KISSlicer"))
                    return SlicerName.KISSlicer;
                else if (slicerLine.Contains("skeinforge"))
                    return SlicerName.Skeinforge;
                else if (slicerLine.Contains("CURA_PROFILE_STRING") || slicerLine.Contains("Cura"))
                    return SlicerName.Cura;
                else if (slicerLine.Contains("Miracle"))
                    return SlicerName.Makerbot;
                else if (slicerLine.Contains("ffslicer"))
                    return SlicerName.FlashForge;
                else if (slicerLine.Contains("Simplify3D"))
                    return SlicerName.Simplify3D;
                else if (slicerLine.Contains("Snapmaker"))
                    return SlicerName.Snapmakerjs;
                else if (slicerLine.Contains("ideaMaker"))
                    return SlicerName.ideaMaker;
                else if (slicerLine.Contains("Voxelizer 2"))
                    return SlicerName.Voxelizer2;
                else
                {
                    return SlicerName.Unkown;

                }
            }
            catch (Exception exc)
            {
                return SlicerName.Unkown;
            }
        }
        private static SlicerName getSlicerNameFromLines(List<string> lines)
        {
            try
            {
                foreach (string line in lines)
                {
                    string slicerLine = line;
                    if (slicerLine.Contains("Slic3r"))
                        return SlicerName.Slic3r;
                    else if (slicerLine.Contains("PrusaSlicer"))
                        return SlicerName.PrusaSlicer;
                    else if (slicerLine.Contains("KISSlicer"))
                        return SlicerName.KISSlicer;
                    else if (slicerLine.Contains("skeinforge"))
                        return SlicerName.Skeinforge;
                    else if (slicerLine.Contains("CURA_PROFILE_STRING") || slicerLine.Contains("Cura"))
                        return SlicerName.Cura;
                    else if (slicerLine.Contains("Miracle"))
                        return SlicerName.Makerbot;
                    else if (slicerLine.Contains("ffslicer"))
                        return SlicerName.FlashForge;
                    else if (slicerLine.Contains("Simplify3D"))
                        return SlicerName.Simplify3D;
                    else if (slicerLine.Contains("Snapmaker"))
                        return SlicerName.Snapmakerjs;
                    else if (slicerLine.Contains("ideaMaker"))
                        return SlicerName.ideaMaker;
                    else if (slicerLine.Contains("Voxelizer 2"))
                        return SlicerName.Voxelizer2;
                    else
                    {
                        continue;
                    }
                }
                return SlicerName.Unkown;


            }
            catch (Exception exc)
            {
                return SlicerName.Unkown;
            }
        }
        private static string getParameterFromSlicer(SlicerName Slicer, SlicerParameter Parameter, List<string> Lines)
        {
            try
            {
                Regex myregex;
                List<string> lines = new List<string>();
                string unknown = "unkown_parameter";
                switch (Slicer)
                {
                    // Old Prusa Slicer
                    case SlicerName.Slic3r:
                        switch (Parameter)
                        {
                            case SlicerParameter.Volume:
                                myregex = new Regex(@"[;]\s*filament used\s*=\s*(\d*\.\d+)mm\s*[(](\d*\.\d+)cm3[)]");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"\(([^)]*)cm3\)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"((\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"[;]\s*estimated printing time \(silent mode\)\s*=*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"((\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filament_diameter\s*=\s*\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*nozzle_diameter\s*=\s*\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*filament_type\s*=\s*([A-Z])*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s]([A-Z]\w))").Groups[1].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filament_density\s*=\s*\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d{1,2}))|([^ =\s]([0-9]*$))").Groups[1].Value;
                            default:
                                return unknown;
                        }
                    // New Prusa Slicer
                    case SlicerName.PrusaSlicer:
                        switch (Parameter)
                        {

                            case SlicerParameter.Volume:
                                myregex = new Regex(@"[;]\s*filament used\s*\[cm3\]\s*=\s*\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"(\s\d*.\d)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                //myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=\s*\d*h\s*\d*m\s*\d*s");
                                myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"((\d*d\s\d*h\s*\d*m\s\d*s)|(\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"[;]\s*estimated printing time \(silent mode\)\s*=*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"((\d*d\s\d*h\s*\d*m\s\d*s)|(\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filament_diameter\s*=\s*\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*nozzle_diameter\s*=\s*\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*filament_type\s*=\s*([A-Z])*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s]([A-Z]\w))").Groups[1].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filament_density\s*=\s*\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            default:
                                return unknown;
                        }
                    // Simplify3D
                    case SlicerName.Simplify3D:
                        switch (Parameter)
                        {
                            case SlicerParameter.Volume:
                                myregex = new Regex(@"([;]\s*Plastic volume:\s*)");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                //var s = Regex.Match(lines[0], @"(\d*.\d{1,2})\s*(cc)").Groups[1].Value;
                                return Regex.Match(lines[0], @"(\d*.\d{1,2})\s*(cc)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                //myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=\s*\d*h\s*\d*m\s*\d*s");
                                myregex = new Regex(@"([;]\s*Build time:*)");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                //var s2 = Regex.Match(lines[0], @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                                return Regex.Match(lines[0], @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"([;]\s*Build time:*)");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                //var s2 = Regex.Match(lines[0], @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                                return Regex.Match(lines[0], @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filamentDiameters,\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*extruderDiameter,\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*printMaterial,\w*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                var s = Regex.Match(lines[0], @"(,)([^,]*)").Groups[2].Value;
                                return Regex.Match(lines[0], @"(,)([^,]*)").Groups[2].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filamentDensities,\d*.\d*");
                                lines = Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            default:
                                return unknown;
                        }
                    // Voxelizer 2
                    case SlicerName.Voxelizer2:

                        VoxelizerSingleGcodeInfo gInfo = new VoxelizerSingleGcodeInfo();
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 1; i < Lines.Count; i++)
                            {
                                string curLine = Lines[i];
                                if (curLine.StartsWith(";;"))
                                    sb.Append(curLine.Replace(";;", "").Replace("{", "").Replace("}", ""));
                                else
                                    break;

                            }
                            string json = sb.ToString();
                            string infoBlock = string.Format("{{ \"info\": [{{{0}}}]}}", Regex.Match(json, "(?<=\"info\": [[])(.*)(?=],)").Groups[0].Value);
                            gInfo = JsonConvert.DeserializeObject<VoxelizerSingleGcodeInfo>(infoBlock);
                            //var gcodeInfo = VoxelizerSingleGcodeInfo.FromJson(infoBlock);
                        }
                        catch (Exception exc)
                        {
                            return "Exception";

                        }
                        switch (Parameter)
                        {
                            /*
                            case SlicerParameter.Volume:
                                string vol = (gInfo.Info[0].VoxelSize * 1000f).ToString().Replace(",", ".");
                                return vol;
                                */
                            case SlicerParameter.FilamentUsed:
                                var filamentUsed = gInfo.Info[0].FilamentUsage.Sum();
                                return (filamentUsed * 1000f).ToString().Replace(",", ".");

                            case SlicerParameter.FilamentDiameter:
                                string[] toolhead = gInfo.Info[0].Toolhead.Split(' ');
                                return toolhead[1];

                            case SlicerParameter.PrintTime:
                                string printTime = gInfo.Info[0].PrintingTime;
                                string[] parts = printTime.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                string days = "0";
                                string hours = "0";
                                string mins = "0";
                                string seconds = "0";
                                foreach (string part in parts)
                                {
                                    if (Regex.IsMatch(part, @"(.*)(?=d)"))
                                    {
                                        days = Regex.Match(part, @"(.*)(?=d)").Groups[0].Value;
                                    }
                                    else if (Regex.IsMatch(part, @"(.*)(?=h)"))
                                    {
                                        hours = Regex.Match(part, @"(.*)(?=h)").Groups[0].Value;
                                    }
                                    else if (Regex.IsMatch(part, @"(.*)(?=min)"))
                                    {
                                        mins = Regex.Match(part, @"(.*)(?=min)").Groups[0].Value;
                                    }
                                    else if (Regex.IsMatch(part, @"(.*)(?=s)"))
                                    {
                                        seconds = Regex.Match(part, @"(.*)(?=s)").Groups[0].Value;
                                    }
                                }
                                string timeString = string.Format("{0}:{1}:{2}:{3}",
                                    days,
                                    hours,
                                    mins,
                                    seconds
                                    );
                                //TimeSpan ts = new TimeSpan(Convert.ToInt32(days), Convert.ToInt32(hours), Convert.ToInt32(mins), Convert.ToInt32(seconds));
                                return timeString; // ts.TotalHours.ToString();
                            default:
                                return unknown;
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
        private static decimal convertPrintTimeToDec(string printTime)
        {
            try
            {
                // If already passed as time string
                if (Regex.IsMatch(printTime, @"(\d+:\d+:\d+:\d+)"))
                {
                    string[] time = printTime.Split(':');
                    TimeSpan ts = new TimeSpan(
                        Convert.ToInt32(time[0]),   // days
                        Convert.ToInt32(time[1]),   // hours
                        Convert.ToInt32(time[2]),   // minutes
                        Convert.ToInt32(time[3]));  // seconds
                    return Convert.ToDecimal(ts.TotalHours); // ts.TotalHours.ToString();
                }
                else
                {
                    string[] time = printTime.Split(' ');
                    string timestring = string.Empty;
                    if (time.Contains("hour") || time.Contains("hours")
                        || time.Contains("minutes") || time.Contains("minute")
                        || time.Contains("seconds") || time.Contains("second")
                        )
                    {
                        string h = "0", m = "0", s = "0";
                        for (int i = 1; i < time.Length; i += 2)
                        {
                            if (time[i] == "hour" || time[i] == "hours")
                            {
                                h = time[i - 1];
                            }
                            else if (time[i] == "minutes" || time[i] == "minute")
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
                    else
                    {
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
            }
            catch (Exception exc)
            {
                return -1m;
            }
        }
        private static List<Point3D> getLayerPointsCollection(List<GCodeCommand> Commands, float z)
        {
            // https://github.com/hudbrog/gCodeViewer/blob/master/js/renderer.js
            List<Point3D> Points = new List<Point3D>();
            try
            {
                int i = 0;
                float prevX = 0;
                float prevY = 0;
                float prevZ = z;

                var cmd = Commands;
                float x = float.NegativeInfinity;
                float y = float.NegativeInfinity;


                for (i = 0; i < Commands.Count; i++)
                {
                    // ctx.lineWidth = 1;
                    try
                    {
                        var test = cmd[i];
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (!float.IsInfinity(cmd[i].PreviousX) && !float.IsInfinity(cmd[i].PreviousY))
                    {
                        prevX = cmd[i].PreviousX;
                        prevY = cmd[i].PreviousY;
                    }

                    if (float.IsInfinity(cmd[i].X) || float.IsNaN(cmd[i].X))
                        x = prevX;
                    else
                        x = cmd[i].X;

                    if (float.IsInfinity(cmd[i].Y) || float.IsNaN(cmd[i].Y))
                        y = prevY;
                    else
                        y = cmd[i].Y;



                    if (!cmd[i].Extrude && !cmd[i].NoMove)
                    {
                        if (cmd[i].Retract == -1)
                        {
                            // show retracts
                            if (true)
                            {
                                // void ctx.arc(x, y, radius, startAngle, endAngle [, anticlockwise]);
                                // (prevX, prevY, renderOptions["sizeRetractSpot"], 0, Math.PI*2, true);
                                //var m = GetCircleModel(2, new Vector3D(0, 0, 0), new Point3D(x, y, prevZ), 20);
                                //lines.Add(m);
                                //ctx.strokeStyle = renderOptions["colorRetract"];
                                //ctx.fillStyle = renderOptions["colorRetract"];
                                //ctx.beginPath();
                                //ctx.arc(prevX, prevY, renderOptions["sizeRetractSpot"], 0, Math.PI * 2, true);
                                //ctx.stroke();
                                //ctx.fill()
                            }
                            // show moves
                            if (false)
                            {
                                Points.Add(new Point3D(prevX, prevY, prevZ));
                                Points.Add(new Point3D(x, y, prevZ));

                                //ctx.strokeStyle = renderOptions["colorMove"];
                                //ctx.beginPath();
                                //ctx.moveTo(prevX, prevY);
                                //ctx.lineTo(x * zoomFactor, y * zoomFactor);
                                //ctx.stroke();
                            }
                        }
                    }
                    else if (cmd[i].Extrude)
                    {

                        Points.Add(new Point3D(prevX, prevY, prevZ));
                        Points.Add(new Point3D(x, y, prevZ));

                    }
                    else
                    {
                        // show retracts
                        if (true)
                        {
                            //ctx.strokeStyle = renderOptions["colorRestart"];
                            //ctx.fillStyle = renderOptions["colorRestart"];
                            //ctx.beginPath();
                            //ctx.arc(prevX, prevY, renderOptions["sizeRetractSpot"], 0, Math.PI * 2, true);
                            //ctx.stroke();
                            //ctx.fill();

                        }
                    }
                    prevX = x;
                    prevY = y;
                }
                return Points;
            }
            catch (Exception exc)
            {
                return Points;
            }
        }
        private static LineBuilder buildLineFromCommands(List<GCodeCommand> Commands, float z)
        {
            // https://github.com/hudbrog/gCodeViewer/blob/master/js/renderer.js
            var lineBuilder = new LineBuilder();
            try
            {
                int i = 0;
                float prevX = 0;
                float prevY = 0;
                float prevZ = z;

                var cmd = Commands;
                float x = float.NegativeInfinity;
                float y = float.NegativeInfinity;


                for (i = 0; i < Commands.Count; i++)
                {
                    // ctx.lineWidth = 1;
                    try
                    {
                        var test = cmd[i];
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (!float.IsInfinity(cmd[i].PreviousX) && !float.IsInfinity(cmd[i].PreviousY))
                    {
                        prevX = cmd[i].PreviousX;
                        prevY = cmd[i].PreviousY;
                    }

                    if (float.IsInfinity(cmd[i].X) || float.IsNaN(cmd[i].X))
                        x = prevX;
                    else
                        x = cmd[i].X;

                    if (float.IsInfinity(cmd[i].Y) || float.IsNaN(cmd[i].Y))
                        y = prevY;
                    else
                        y = cmd[i].Y;



                    if (!cmd[i].Extrude && !cmd[i].NoMove)
                    {
                        if (cmd[i].Retract == -1)
                        {
                            // show retracts
                            if (true)
                            {
                                // void ctx.arc(x, y, radius, startAngle, endAngle [, anticlockwise]);
                                // (prevX, prevY, renderOptions["sizeRetractSpot"], 0, Math.PI*2, true);
                                //var m = GetCircleModel(2, new Vector3D(0, 0, 0), new Point3D(x, y, prevZ), 20);
                                //lines.Add(m);
                                //ctx.strokeStyle = renderOptions["colorRetract"];
                                //ctx.fillStyle = renderOptions["colorRetract"];
                                //ctx.beginPath();
                                //ctx.arc(prevX, prevY, renderOptions["sizeRetractSpot"], 0, Math.PI * 2, true);
                                //ctx.stroke();
                                //ctx.fill()
                            }
                            // show moves
                            if (false)
                            {
                                //Points.Add(new Point3D(prevX, prevY, prevZ));
                                //Points.Add(new Point3D(x, y, prevZ));

                                //ctx.strokeStyle = renderOptions["colorMove"];
                                //ctx.beginPath();
                                //ctx.moveTo(prevX, prevY);
                                //ctx.lineTo(x * zoomFactor, y * zoomFactor);
                                //ctx.stroke();
                            }
                        }
                    }
                    else if (cmd[i].Extrude)
                    {
                        lineBuilder.Add(true, new SharpDX.Vector3[] { new SharpDX.Vector3(prevX, prevY, prevZ), new SharpDX.Vector3(x, y, prevZ) });
                        //Points.Add(new Point3D(prevX, prevY, prevZ));
                        //Points.Add(new Point3D(x, y, prevZ));

                    }
                    else
                    {
                        // show retracts
                        if (true)
                        {
                            //ctx.strokeStyle = renderOptions["colorRestart"];
                            //ctx.fillStyle = renderOptions["colorRestart"];
                            //ctx.beginPath();
                            //ctx.arc(prevX, prevY, renderOptions["sizeRetractSpot"], 0, Math.PI * 2, true);
                            //ctx.stroke();
                            //ctx.fill();

                        }
                    }
                    prevX = x;
                    prevY = y;
                }
                return lineBuilder;
            }
            catch (Exception exc)
            {
                return lineBuilder;
            }
        }

        private static void DrawLine(LinesVisual3D lines, double x_start, double y_start, double z_start, double x_stop, double y_stop, double z_stop)
        {
            lines.Points.Add(new Point3D(x_start, y_start, z_start));
            lines.Points.Add(new Point3D(x_stop, y_stop, z_stop));
        }
        private static void DrawLine(LinesVisual3D lines, Point3D start, Point3D end)
        {
            lines.Points.Add(start);
            lines.Points.Add(end);
        }
        #endregion
    }
}
