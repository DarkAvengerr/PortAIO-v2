using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace TheCassiopeia.Commons
{
    public static class ManaManager
    {
        private static readonly Dictionary<Orbwalking.OrbwalkingMode, int> ManaSettings = new Dictionary<Orbwalking.OrbwalkingMode, int>();
        public static bool EnableLimitations = true;

        static ManaManager()
        {
            ManaSettings.Add(Orbwalking.OrbwalkingMode.Combo, 0);
            ManaSettings.Add(Orbwalking.OrbwalkingMode.LaneClear, 0);
            ManaSettings.Add(Orbwalking.OrbwalkingMode.LastHit, 60);
            ManaSettings.Add(Orbwalking.OrbwalkingMode.Mixed, 50);
            ManaSettings.Add(Orbwalking.OrbwalkingMode.None, 0);
        }

        public static void Initialize(Menu manamanagerMenu)
        {
            //Only using this three, because a manamanager on Combo or None wouldn't make sense
            Initialize(manamanagerMenu, Orbwalking.OrbwalkingMode.Mixed, "Harass");
            Initialize(manamanagerMenu, Orbwalking.OrbwalkingMode.LaneClear);
            Initialize(manamanagerMenu, Orbwalking.OrbwalkingMode.LastHit, "Lasthit");
            manamanagerMenu.AddMItem("Enabled", false, (sender, args) => EnableLimitations = args.GetNewValue<bool>());
            manamanagerMenu.ProcStoredValueChanged<bool>();
        }

        public static void Initialize(Menu menu, Orbwalking.OrbwalkingMode mode)
        {
            menu.AddMItem(mode.ToString(), new Slider(ManaSettings[mode]), (sender, args) => ManaSettings[mode] = args.GetNewValue<Slider>().Value).ProcStoredValueChanged<Slider>();
        }

        public static void Initialize(Menu menu, Orbwalking.OrbwalkingMode mode, string name)
        {
            menu.AddMItem(name, new Slider(ManaSettings[mode]), (sender, args) => ManaSettings[mode] = args.GetNewValue<Slider>().Value).ProcStoredValueChanged<Slider>();
        }

        public static List<KeyValuePair<Orbwalking.OrbwalkingMode, int>> GetSettings()
        {
            return ManaSettings.ToList();
        }

        public static bool CanUseMana(Orbwalking.OrbwalkingMode mode)
        {
            //  Console.WriteLine(mode + " " + ObjectManager.Player.ManaPercent + " > " + ManaSettings[mode]);
            return ObjectManager.Player.ManaPercent >= ManaSettings[mode] || !EnableLimitations;
        }

    }
}
