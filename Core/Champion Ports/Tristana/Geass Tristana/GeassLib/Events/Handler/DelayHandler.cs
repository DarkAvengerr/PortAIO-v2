using GeassLib.Humanizer;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Events
{
    class DelayHandler
    {
        public static TickManager MyTicker = new TickManager();
        public static bool Loaded;
        public static void Load()
        {
            MyTicker.AddTick("GeassLib.OnLevel",50,100);
            MyTicker.AddTick("GeassLib.TrinketBuy", 75, 125);
            MyTicker.AddTick("GeassLib.Items", 100, 150);
            Loaded = true;
        }

        public static bool CheckOnLevel() => MyTicker.CheckTick("GeassLib.OnLevel");
        public static void UseOnLevel() => MyTicker.UseTick("GeassLib.OnLevel");

        public static bool CheckTrinket() => MyTicker.CheckTick("GeassLib.TrinketBuy");
        public static void UseTrinket() => MyTicker.UseTick("GeassLib.TrinketBuy");

        public static bool CheckItems() => MyTicker.CheckTick("GeassLib.Items");
        public static void UseItems() => MyTicker.UseTick("GeassLib.Items");
    }
}
