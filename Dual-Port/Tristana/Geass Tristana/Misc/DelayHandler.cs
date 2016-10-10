using GeassLib.Humanizer;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Misc
{
    class DelayHandler
    {

        public static TickManager MyTicker = new TickManager();
        public static bool Loaded;
        public static void Load()
        {
            MyTicker.AddTick("GeassTristana.OrbwalkDelay", 50, 100);
            MyTicker.AddTick("GeassTristana.KSDelay", 100, 150);
            MyTicker.AddTick("GeassTristana.InterrupterDelay", 0, 50);
            MyTicker.AddTick("GeassTristana.GapCloseDelay",25, 75);
            Loaded = true;
        }

        public static bool CheckOrbwalk() => MyTicker.CheckTick("GeassTristana.OrbwalkDelay");
        public static void UseOrbwalk() => MyTicker.UseTick("GeassTristana.OrbwalkDelay");

        // ReSharper disable once InconsistentNaming
        public static bool CheckKS() => MyTicker.CheckTick("GeassTristana.KSDelay");
        // ReSharper disable once InconsistentNaming
        public static void UseKS() => MyTicker.UseTick("GeassTristana.KSDelay");

        public static bool CheckInterrption() => MyTicker.CheckTick("GeassTristana.InterrupterDelay");
        public static void UseInterrption() => MyTicker.UseTick("GeassTristana.InterrupterDelay");


        public static bool CheckGapClose() => MyTicker.CheckTick("GeassTristana.GapCloseDelay");
        public static void UseGapClose() => MyTicker.UseTick("GeassTristana.GapCloseDelay");
    }
}
