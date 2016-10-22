using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using PortAIO.Properties;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Utils.Drawings
{
    internal static class Crosshair
    {
        private static readonly Dictionary<string, SpellSlot> SupportedHeros = new Dictionary<string, SpellSlot>
        {
            { "Caitlyn", SpellSlot.R },
            { "Ezreal", SpellSlot.R },
            { "Graves", SpellSlot.R },
            { "Jinx", SpellSlot.R },
            { "Varus", SpellSlot.Q },
            { "Ashe", SpellSlot.R }
        };
    }
}