using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Misc
{
#pragma warning disable RECS0014 // If all fields, properties and methods members are static, the class can be made static.
    internal class Core
#pragma warning restore RECS0014 // If all fields, properties and methods members are static, the class can be made static.
    {
        private const string AssemblyName = "Geass Tristana [B]";

        //Global External Classes and Variables
        public static Orbwalking.Orbwalker CommonOrbwalker { get; set; }
        public static Damage DamageLib = new Damage();
        public static Menu SMenu { get; set; } = new Menu(AssemblyName, AssemblyName, true);
        public static GeassLib.Functions.Logging.Logger Logger = new GeassLib.Functions.Logging.Logger("Geass Tristana");
        //Hold Global Data and Functions
        public static Libaries.Champion Champion = new Libaries.Champion(550f, 900f, 625f, 700f);

        public static GeassLib.Humanizer.TickManager TickManager = new GeassLib.Humanizer.TickManager();

        public static readonly BuffType[] Bufftype = GeassLib.Data.Buffs.GetTypes;
    }
}