using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko
{
    public static class ManaManager
    {
        private static readonly Dictionary<Orbwalking.OrbwalkingMode, int> ManaSettings = new Dictionary<Orbwalking.OrbwalkingMode, int>();
        public static bool EnableLimitations = true;

        static ManaManager()
        {
            ManaSettings.Add(Orbwalking.OrbwalkingMode.Combo, 0);
            ManaSettings.Add(Orbwalking.OrbwalkingMode.LaneClear, 90);
            ManaSettings.Add(Orbwalking.OrbwalkingMode.LastHit, 60);
            ManaSettings.Add(Orbwalking.OrbwalkingMode.Mixed, 75);
            ManaSettings.Add(Orbwalking.OrbwalkingMode.None, 0);
        }

        public static void Initialize(Menu menu, string menuName = "Manamanager", bool mixed = true, bool lasthit = true, bool laneclear = true)
        {
            //Only using this three, because a manamanager on Combo or None wouldn't make sense
            var mmMenu = new Menu("Manamanager", "Manamanager");
            if (mixed)
                Initialize(mmMenu, Orbwalking.OrbwalkingMode.Mixed);
            if (laneclear)
                Initialize(mmMenu, Orbwalking.OrbwalkingMode.LaneClear);
            if (lasthit)
                Initialize(mmMenu, Orbwalking.OrbwalkingMode.LastHit);
            mmMenu.AddMItem("Enabled", true, (sender, args) => EnableLimitations = args.GetNewValue<bool>());
            menu.AddSubMenu(mmMenu);
        }

        public static void Initialize(Menu menu, Orbwalking.OrbwalkingMode mode)
        {
            menu.AddMItem(mode.ToString(), new Slider(ManaSettings[mode]), (sender, args) => ManaSettings[mode] = args.GetNewValue<Slider>().Value).ProcStoredValueChanged<Slider>();
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
