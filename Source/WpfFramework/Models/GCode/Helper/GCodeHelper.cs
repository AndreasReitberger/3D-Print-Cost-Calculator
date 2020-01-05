using System.Collections.Generic;

namespace WpfFramework.Models.GCode.Helper
{
    public class GCodeModel
    {
        public string Slicer = "Unknown";
        public float PrintTime = 0;
        public float TotalFilament = 0;
        public int SpeedIndex = 0;
        public List<float> volSpeeds = new List<float>();
        public List<float> extrusionSpeeds = new List<float>();
        public float Width = 0;
        public float Height = 0;
        public float Depth = 0;
        public int Layers = 0;
        public float LayerHeight = 0;

        public List<List<GCodeCommand>> Commands = new List<List<GCodeCommand>>();

        public Dictionary<float, List<float>> volSpeedsByLayer = new Dictionary<float, List<float>>();
        public Dictionary<float, List<float>> extrusionSpeedsByLayer = new Dictionary<float, List<float>>();

        public Dictionary<string, float> speedsByLayer = new Dictionary<string, float>();
        public Dictionary<float, int> zHeights = new Dictionary<float, int>();
    }
    public struct GCodeCommand
    {
        public float X;
        public float Y;
        public float Z;
        public bool Extrude;
        public float Retract;
        public bool NoMove;
        public float Extrusion;
        public string Extruder;
        public float PreviousX;
        public float PreviousY;
        public float PreviousZ;
        public float Speed;
        public int GCodeLine;
        public float VolumePerMM;
        public string Command;
        public string OriginalLine;
    }

    public struct GCodeObjectSize
    {
        public float X;
        public float Y;
        public float Z;

    }
}
