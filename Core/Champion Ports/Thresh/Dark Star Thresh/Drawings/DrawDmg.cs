using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Drawings
{
    using System;
    using System.Linq;

    using Dark_Star_Thresh.Core;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class DrawDmg : Core
    {
        
        private static readonly Dmg Dmg = new Dmg();

        public static void OnEndScene(EventArgs args)
        {
        }
    }
}
