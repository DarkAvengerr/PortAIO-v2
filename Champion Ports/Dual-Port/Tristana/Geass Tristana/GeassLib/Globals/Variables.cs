using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Globals
{
    public class Variables
    {
        public static float AssemblyTime() => (float)DateTime.Now.Subtract(Objects.AssemblyLoadTime).TotalMilliseconds;
        public static string AssemblyName;
    
        public static bool InCombo = false;
    }
}
