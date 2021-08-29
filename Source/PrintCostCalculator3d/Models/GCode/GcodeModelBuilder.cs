using AndreasReitberger.Interfaces;
using AndreasReitberger.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using PrintCostCalculator3d.Resources.Localization;
using log4net;

namespace PrintCostCalculator3d.Models.GCode
{
    public class GcodeModelBuilder : IGcodeModelBuilder
    {
        #region Logger
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Instance
        static GcodeModelBuilder _instance = null;
        static readonly object Lock = new object();
        public static GcodeModelBuilder Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = new GcodeModelBuilder();
                }
                return _instance;
            }

            set
            {
                if (_instance == value) return;
                lock (Lock)
                {
                    _instance = value;
                }
            }

        }
        #endregion

        #region Public
        public async Task<List<LineBuilder>> BuildGcodeLayerModelListAsync(Gcode gcode, IProgress<int> prog)
        {
            gcode.IsWorking = true;
            var lineBuilders = new List<LineBuilder>();
            SortedDictionary<double, LineBuilder> gcodeLayers = new SortedDictionary<double, LineBuilder>();

            try
            {
                var Commands = gcode.Commands;
                await Task.Run(() =>
                {
                    try
                    {
                        var temp = new List<LinesVisual3D>();
                        int i = 0;
                        foreach (List<GcodeCommandLine> commands in Commands)
                        {
                            double z = gcode.ZHeights.Keys.ElementAt(i);

                            Application.Current.Dispatcher.Invoke((() =>
                            {
                                LineBuilder builder = BuildLineFromCommands(commands, z);
                                gcodeLayers.Add(z, builder);
                            }));

                            if (prog != null)
                            {
                                double test = (((double)i / Commands.Count) * 100f);
                                if (i < Commands.Count - 1)
                                    prog.Report(Convert.ToInt32(test));
                                else
                                    prog.Report(100);
                            }
                            i++;
                        }
                    }
                    catch (Exception exc)
                    {
                        logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                    }
                });
                Application.Current.Dispatcher.Invoke(() =>
                {
                    gcode.LayerModelGenerated = true;
                    lineBuilders = gcodeLayers.Select(pair => pair.Value).ToList();
                });
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
            gcode.IsWorking = false;
            return lineBuilders;
        }

        public async Task<List<LinesVisual3D>> Create2dGcodeLayerModelListAsync(Gcode gcode, IProgress<int> prog)
        {
            gcode.IsWorking = true;
            var list = new List<LinesVisual3D>();
            //var line = new LinesVisual3D();
            SortedDictionary<double, LinesVisual3D> gcodeLayers = new SortedDictionary<double, LinesVisual3D>();
            //ConcurrentBag<LinesVisual3D> layers = new ConcurrentBag<LinesVisual3D>();
            try
            {
                var Commands = gcode.Commands;
                await Task.Run(() =>
                {
                    try
                    {
                        var temp = new List<LinesVisual3D>();
                        int i = 0;
                        //foreach (List<GCodeCommand> commands in Commands)
                        foreach (List<GcodeCommandLine> commands in Commands)
                        {
                            double z = gcode.ZHeights.Keys.ElementAt(i);
                            var pointsPerLayer = GetLayerPointsCollection(commands, z);
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
                    catch (Exception exc)
                    {
                        logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                    }
                });
                /**/
                Application.Current.Dispatcher.Invoke(() =>
                {
                    gcode.LayerModelGenerated = true;
                    list = gcodeLayers.Select(pair => pair.Value).ToList();
                });
                //list = layers.ToList();
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
            gcode.IsWorking = false;
            return list;
        }

        public async Task<List<LinesVisual3D>> Create3dGcodeLayerModelListAsync(Gcode gcode, IProgress<int> prog)
        {
            gcode.IsWorking = true;
            var list = new List<LinesVisual3D>();

            SortedDictionary<double, LinesVisual3D> gcodeLayers = new SortedDictionary<double, LinesVisual3D>();

            try
            {
                var Commands = gcode.Commands;
                LinesVisual3D normalmoves = new LinesVisual3D();
                LinesVisual3D rapidmoves = new LinesVisual3D();
                LinesVisual3D wirebox = new LinesVisual3D();

                await Task.Run(() =>
                {
                    var temp = new List<LinesVisual3D>();
                    int i = 0;

                    foreach (List<GcodeCommandLine> commands in Commands)
                    {
                        double z = gcode.ZHeights.Keys.ElementAt(i);
                        for (int j = 0; j < commands.Count; j++)
                        {
                            GcodeCommandLine cmd = commands[j];
                            if (cmd.Command != "g0" && cmd.Command != "g1" && cmd.Command != "g2" && cmd.Command != "g3")
                                continue;

                            double x_prev = !double.IsInfinity(cmd.PrevX) ? cmd.PrevX : 0;
                            double y_prev = !double.IsInfinity(cmd.PrevY) ? cmd.PrevY : 0;
                            double z_prev = !double.IsInfinity(cmd.PrevZ) ? cmd.PrevZ : 0;
                            double x = !double.IsInfinity(cmd.X) ? cmd.X : x_prev;
                            double y = !double.IsInfinity(cmd.Y) ? cmd.Y : y_prev;
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
                                    /*
                                    bool clockwise = false;
                                    if (cmd.Command == "g2")
                                        clockwise = true;
                                    */
                                    throw new Exception("Not supported!");
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
                            double test = (((double)i / Commands.Count) * 100f);
                            if (i < Commands.Count - 1)
                                prog.Report(Convert.ToInt32(test));
                            else
                                prog.Report(100);
                        }
                        i++;
                    }
                });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    list = new List<LinesVisual3D>();
                    list.Add(normalmoves);
                });
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
            gcode.IsWorking = false;
            return list;
        }
        #endregion

        #region Private
        List<Point3D> GetLayerPointsCollection(List<GcodeCommandLine> Commands, double z)
        {
            // https://github.com/hudbrog/gCodeViewer/blob/master/js/renderer.js
            List<Point3D> Points = new List<Point3D>();
            try
            {
                int i = 0;
                double prevX = 0;
                double prevY = 0;
                double prevZ = z;

                var cmd = Commands;
                double x = double.NegativeInfinity;
                double y = double.NegativeInfinity;


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

                    if (!double.IsInfinity(cmd[i].PrevX) && !double.IsInfinity(cmd[i].PrevY))
                    {
                        prevX = cmd[i].PrevX;
                        prevY = cmd[i].PrevY;
                    }

                    if (double.IsInfinity(cmd[i].X) || double.IsNaN(cmd[i].X))
                        x = prevX;
                    else
                        x = cmd[i].X;

                    if (double.IsInfinity(cmd[i].Y) || double.IsNaN(cmd[i].Y))
                        y = prevY;
                    else
                        y = cmd[i].Y;



                    if (!cmd[i].IsExtruding && !cmd[i].NoMove)
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
                    else if (cmd[i].IsExtruding)
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
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return Points;
            }
        }
        LineBuilder BuildLineFromCommands(List<GcodeCommandLine> Commands, double z)
        {
            // https://github.com/hudbrog/gCodeViewer/blob/master/js/renderer.js
            var lineBuilder = new LineBuilder();
            try
            {
                int i = 0;
                double prevX = 0;
                double prevY = 0;
                double prevZ = z;

                var cmd = Commands;
                double x = double.NegativeInfinity;
                double y = double.NegativeInfinity;


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

                    if (!double.IsInfinity(cmd[i].PrevX) && !double.IsInfinity(cmd[i].PrevY))
                    {
                        prevX = cmd[i].PrevX;
                        prevY = cmd[i].PrevY;
                    }

                    if (double.IsInfinity(cmd[i].X) || double.IsNaN(cmd[i].X))
                        x = prevX;
                    else
                        x = cmd[i].X;

                    if (double.IsInfinity(cmd[i].Y) || double.IsNaN(cmd[i].Y))
                        y = prevY;
                    else
                        y = cmd[i].Y;



                    if (!cmd[i].IsExtruding && !cmd[i].NoMove)
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
                    else if (cmd[i].IsExtruding)
                    {
                        lineBuilder.Add(true, new SharpDX.Vector3[] { new SharpDX.Vector3((float)prevX, (float)prevY, (float)prevZ), new SharpDX.Vector3((float)x, (float)y, (float)prevZ) });
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
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return lineBuilder;
            }
        }

        void DrawLine(LinesVisual3D lines, double x_start, double y_start, double z_start, double x_stop, double y_stop, double z_stop)
        {
            lines.Points.Add(new Point3D(x_start, y_start, z_start));
            lines.Points.Add(new Point3D(x_stop, y_stop, z_stop));
        }
        #endregion
    }
}
