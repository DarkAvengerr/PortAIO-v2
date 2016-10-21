
using CjShuJinx.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CjShuJinx.Modes
{

    using System.Drawing;
    using LeagueSharp.Common;


    internal class ModeSettings
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuSkins { get; private set; }
        public static Menu MenuFlame { get; private set; }
        public static Menu MenuSpellQ { get; private set; }
        public static Menu MenuSpellW { get; private set; }
        public static Menu MenuSpellE { get; private set; }
        public static Menu MenuSpellR { get; private set; }

        public static void Init(Menu MenuParent)
        {
            MenuLocal = new Menu("Settings", "Settings");
            MenuParent.AddSubMenu(MenuLocal);

            MenuSpellW = new Menu("W:", "SettingsW").SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor());
            {
                MenuSpellW.AddItem(new MenuItem("Set.W.Hitchance", "Hitchance:").SetValue(new StringList(new[] {"Medium", "High", "Veryhigh"}, 2)));
                MenuLocal.AddSubMenu(MenuSpellW);
            }

            MenuSpellR = new Menu("R:", "SettingsR").SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor());
            {
                MenuSpellR.AddItem(new MenuItem("Set.R.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "Medium", "High", "Veryhigh" }, 2)));
                MenuSpellR.AddItem(new MenuItem("Set.R.JungleSteal", "Baron / Dragon Steal:").SetValue(true));
                MenuSpellR.AddItem(new MenuItem("Set.R.BaseUlti", "Base Ulti:").SetValue(true));
                MenuSpellR.AddItem(new MenuItem("Set.R.Ping", "Ping for Killable Enemy (Local):").SetValue(new StringList(new[] { "Off", "Fallback", "Danger" }, 2)));
                MenuLocal.AddSubMenu(MenuSpellR);
            }

            MenuFlame = new Menu("Flame", "Flame");
            MenuFlame.AddItem(new MenuItem("Flame.Laugh", "After Kill:").SetValue(new StringList(new[] {"Off", "Joke", "Taunt", "Laugh", "Random"}, 4)));
            MenuFlame.AddItem(new MenuItem("Flame.Ctrl6", "After Kill: Show Champion Point Icon (Ctrl + 6)").SetValue(new StringList(new[] { "Off", "On" })));
            
            Modes.ModeBaseUlti.Init(MenuLocal);
            // Common.CommonManaManager.Init(MenuLocal);
        }

        static void LoadDefaultSettingsQ()
        {
            string[] str = new string[1000 / 250];
            for (var i = 250; i <= 1000; i += 250)
            {
                str[i / 250 - 1] = i + " ms. ";
            }

            MenuSpellQ.Item("Settings.Q.VisibleDelay").SetValue(new StringList(str, 0));
            MenuSpellQ.Item("Settings.Q.CastDelay").SetValue(new StringList(str, 0));
        }

        static void LoadDefaultSettingsE()
        {
            string[] str = new string[1000 / 250];
            for (var i = 250; i <= 1000; i += 250)
            {
                str[i / 250 - 1] = i + " ms. ";
            }

            MenuSpellE.Item("Settings.E.VisibleDelay").SetValue(new StringList(str, 0));
            MenuSpellE.Item("Settings.E.Auto").SetValue(new StringList(new[] { "Off", "On" }, 1));

        }
    }
}