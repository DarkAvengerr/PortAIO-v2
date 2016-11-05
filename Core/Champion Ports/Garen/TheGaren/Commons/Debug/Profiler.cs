using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheKalista.Commons.Debug
{
    public static class Profiler
    {
        private static Stopwatch _time;
        private static string _name;
        private static readonly Dictionary<string, int> Sections = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> Spikes = new Dictionary<string, int>();

        

        public static void StartSection(string name)
        {
            _name = name;
            _time = Stopwatch.StartNew();
        }

        public static void StartEndSection(string name)
        {
            EndSection();
            _name = name;
            _time = Stopwatch.StartNew();
        }

        public static void EndSection()
        {
            if (_time == null || _name == null) return;
            Sections.Add(_name, (int)_time.ElapsedTicks);
            if (!Spikes.ContainsKey(_name)) Spikes.Add(_name, (int)_time.ElapsedTicks);
            if (Spikes[_name] < (int)_time.ElapsedTicks) Spikes[_name] = (int)_time.ElapsedTicks;
        }

        public static void Clear()
        {
            Sections.Clear();
        }

        public static void ClearMax()
        {
            Spikes.Clear();
        }

        public static void DrawSections(int startX, int startY)
        {
            for (int i = 0; i < Sections.Count; i++)
            {
                Drawing.DrawText(startX, startY + i * 20, Color.Red, Sections.Keys.ToArray()[i] + " (max: " + Spikes[Sections.Keys.ToArray()[i]] + "): " + Sections.Values.ToArray()[i]);
            }
        }

    }
}
