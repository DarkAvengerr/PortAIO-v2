using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using Color = SharpDX.Color;
using EloBuddy;

namespace Irelia.Modes
{
    internal class ModeSettings
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuSkins { get; private set; }
        public static Menu MenuSettingQ { get; private set; }
        public static Menu MenuSettingE { get; private set; }
        public static Menu MenuFlame { get; private set; }
        public static Menu MenuSpellR { get; private set; }

        public static void Init(Menu MenuParent)
        {
            MenuLocal = new Menu("Settings", "Settings");
            MenuParent.AddSubMenu(MenuLocal);

            MenuSettingQ = new Menu("Q:", "SettingsQ").SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor());
            {
                string[] strQ = new string[1000 / 250];
                for (var i = 250; i <= 1000; i += 250)
                {
                    strQ[i / 250 - 1] = i + " ms. ";
                }
                MenuSettingQ.AddItem(new MenuItem("Settings.Q.VisibleDelay", "Instatly Visible Enemy Cast Delay:").SetValue(new StringList(strQ, 2))).SetTooltip("Exp: Rengar, Shaco, Wukong, Kha'Zix, Vayne").SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor());
                MenuSettingQ.AddItem(new MenuItem("Settings.Q.CastDelay", "Humanizer Cast Delay [Lane / Combo]").SetValue(new StringList(strQ, 2))).SetTooltip("Exp: Rengar, Shaco, Wukong, Kha'Zix, Vayne").SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor());
                MenuSettingQ.AddItem(new MenuItem("Settings.Q.Default", "Load Recommended Settings").SetValue(true)).SetFontStyle(FontStyle.Bold, Color.Wheat)
                    .ValueChanged +=
                        (sender, args) =>
                        {
                            if (args.GetNewValue<bool>() == true)
                            {
                                LoadDefaultSettingsQ();
                            }
                        };
            }
            MenuLocal.AddSubMenu(MenuSettingQ);

            MenuSettingE = new Menu("E:", "SettingsE").SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor());
            {
                string[] strE = new string[1000/250];
                for (var i = 250; i <= 1000; i += 250)
                {
                    strE[i/250 - 1] = i + " ms. ";
                }
                MenuSettingE.AddItem(new MenuItem("Settings.E.VisibleDelay", "Instatly Visible Enemy Cast Delay:").SetValue(new StringList(strE, 2))).SetTooltip("Exp: Rengar, Shaco, Wukong, Kha'Zix, Vayne").SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor());
                MenuSettingE.AddItem(new MenuItem("Settings.E.Auto", "Auto-Use (If can stun enemy)").SetValue(new StringList(new []{"Off", "On"}, 1))).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor());
                MenuSettingE.AddItem(new MenuItem("Settings.E.Default", "Load Recommended Settings").SetValue(true)).SetFontStyle(FontStyle.Bold, Color.Wheat)
                    .ValueChanged +=
                        (sender, args) =>
                        {
                            if (args.GetNewValue<bool>() == true)
                            {
                                LoadDefaultSettingsE();
                            }
                        };

            }
            MenuLocal.AddSubMenu(MenuSettingE);

            MenuFlame = new Menu("Flame", "Flame");
            MenuFlame.AddItem(new MenuItem("Flame.Laugh", "After Kill:").SetValue(new StringList(new[] {"Off", "Joke", "Taunt", "Laugh", "Random"}, 4)));
            MenuFlame.AddItem(new MenuItem("Flame.Ctrl6", "After Kill: Show Champion Point Icon (Ctrl + 6)").SetValue(new StringList(new[] { "Off", "On" })));
            
            Modes.ModeJump.Init(MenuLocal);
            //Common.CommonManaManager.Init(MenuLocal);
        }

        static void LoadDefaultSettingsQ()
        {
            string[] str = new string[1000 / 250];
            for (var i = 250; i <= 1000; i += 250)
            {
                str[i / 250 - 1] = i + " ms. ";
            }

            MenuSettingQ.Item("Settings.Q.VisibleDelay").SetValue(new StringList(str, 0));
            MenuSettingQ.Item("Settings.Q.CastDelay").SetValue(new StringList(str, 0));
        }

        static void LoadDefaultSettingsE()
        {
            string[] str = new string[1000 / 250];
            for (var i = 250; i <= 1000; i += 250)
            {
                str[i / 250 - 1] = i + " ms. ";
            }

            MenuSettingE.Item("Settings.E.VisibleDelay").SetValue(new StringList(str, 0));
            MenuSettingE.Item("Settings.E.Auto").SetValue(new StringList(new[] { "Off", "On" }, 1));

        }
    }
}
