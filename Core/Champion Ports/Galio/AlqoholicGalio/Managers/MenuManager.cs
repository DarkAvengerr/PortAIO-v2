// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="LeagueSharp">
//   Copyright (C) 2016 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicGalio.Managers
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = SharpDX.Color;

    #endregion

    internal class MenuManager
    {
        #region Static Fields

        public static Menu AlliesMenu;

        public static Menu ComboMenu;

        public static Menu DrawMenu;

        public static Menu EscapeMenu;

        public static Menu HarassMenu;

        public static Menu LaneClearMenu;

        public static Menu Menu;

        public static Menu MiscMenu;

        public static Orbwalking.Orbwalker Orbwalker;

        public static Menu WMenu;

        private static readonly List<AIHeroClient> AlliesList = HeroManager.Allies.ToList();

        #endregion

        #region Public Properties

        public static bool AutoHarass => HarassMenu.Item("autoHarass").GetValue<KeyBind>().Active;

        public static bool DrawDamage => DrawMenu.Item("drawDamage").GetValue<bool>();

        public static bool DrawE => DrawMenu.Item("drawE").GetValue<bool>();

        public static bool DrawEnabled => DrawMenu.Item("drawEnabled").GetValue<bool>();

        public static bool DrawQ => DrawMenu.Item("drawQ").GetValue<bool>();

        public static bool DrawR => DrawMenu.Item("drawR").GetValue<bool>();

        public static bool DrawW => DrawMenu.Item("drawW").GetValue<bool>();

        public static int FarmMana => LaneClearMenu.Item("laneClearMana").GetValue<Slider>().Value;

        public static int FarmMinions => LaneClearMenu.Item("laneClearMinions").GetValue<Slider>().Value;

        public static bool FarmSpells => LaneClearMenu.Item("laneClearSpells").GetValue<KeyBind>().Active;

        public static int HarassMana => HarassMenu.Item("harassMana").GetValue<Slider>().Value;

        public static int Prediction => ComboMenu.Item("comboPrediction").GetValue<StringList>().SelectedIndex;

        public static int RAmount => ComboMenu.Item("rAmount").GetValue<Slider>().Value;

        public static bool RFlash => MiscMenu.Item("miscRFlash").GetValue<KeyBind>().Active;

        public static bool Test => MiscMenu.Item("test").GetValue<KeyBind>().Active;

        #endregion

        #region Public Methods and Operators

        public static void Init()
        {
            CreateMenu();
        }

        #endregion

        #region Methods

        private static void CreateMenu()
        {
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");

            Menu = new Menu("DrunkGalio", "DrunkGalio", true).SetFontStyle(FontStyle.Underline, Color.Gold);
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu.SetFontStyle(FontStyle.Bold, Color.MediumTurquoise));
            Menu.AddSubMenu(orbwalkerMenu);
            SPrediction.Prediction.Initialize(Menu);

            // Combo Menu
            ComboMenu = new Menu("Drunk Galio - Combo", "comboMenu").SetFontStyle(FontStyle.Bold, Color.MediumTurquoise);
            ComboMenu.AddItem(
                new MenuItem("comboPrediction", "Combo - Prediction Mode").SetValue(
                    new StringList(
                        new[]
                            {
                                HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(),
                                HitChance.VeryHigh.ToString()
                            },
                        2)));
            ComboMenu.AddItem(new MenuItem("comboQ", "Combo - Use Q").SetValue(true));
            ComboMenu.AddItem(new MenuItem("comboW", "Combo - Use W").SetValue(true));
            ComboMenu.AddItem(new MenuItem("comboE", "Combo - Use E").SetValue(true));
            ComboMenu.AddItem(new MenuItem("comboR", "Combo - Use R").SetValue(true));
            ComboMenu.AddItem(new MenuItem("rAmount", "Only R when x >= enemies").SetValue(new Slider(3, 0, 5)));

            // Harass Menu
            HarassMenu = new Menu("Drunk Galio - Harass", "harassMenu").SetFontStyle(FontStyle.Bold, Color.MediumTurquoise);
            HarassMenu.AddItem(
                new MenuItem("autoHarass", "Auto Harass").SetValue(new KeyBind('T', KeyBindType.Toggle, true)))
                .Permashow(true, "Drunk Galio - Auto Harass", new Color(0, 255, 255));

            HarassMenu.AddItem(new MenuItem("harassQ", "Harass - Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("harassE", "Harass - Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("harassMana", "Harass - Mana").SetValue(new Slider(60)));

            // Lane Clear Menu
            LaneClearMenu = new Menu("Drunk Galio - Lane Clear", "laneClearMenu").SetFontStyle(FontStyle.Bold, Color.MediumTurquoise);
            LaneClearMenu.AddItem(
                new MenuItem("laneClearSpells", "Lane Clear - Use Spells").SetValue(
                    new KeyBind('M', KeyBindType.Toggle)))
                .Permashow(true, "Drunk Galio - Lane Clear Spells", new Color(0, 255, 255));

            LaneClearMenu.AddItem(new MenuItem("laneClearQ", "Lane Clear - Use Q").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("laneClearE", "Lane Clear - Use E").SetValue(true));
            LaneClearMenu.AddItem(
                new MenuItem("laneClearMinions", "Lane Clear - Minimum Minions").SetValue(new Slider(3, 1, 6)));

            LaneClearMenu.AddItem(new MenuItem("laneClearMana", "Lane Clear - Mana").SetValue(new Slider(60)));

            // Escape Menu
            EscapeMenu = new Menu("Drunk Galio - Escape", "escapeMenu").SetFontStyle(FontStyle.Bold, Color.MediumTurquoise);
            EscapeMenu.AddItem(
                new MenuItem("escapeKey", "Escape - Escape").SetValue(new KeyBind('A', KeyBindType.Press)));

            EscapeMenu.AddItem(new MenuItem("escapeQ", "Escape - Use Q").SetValue(false));
            EscapeMenu.AddItem(new MenuItem("escapeE", "Escape - Use E").SetValue(true));

            // Draw Menu
            DrawMenu = new Menu("Drunk Galio - Draw", "drawMenu").SetFontStyle(FontStyle.Bold, Color.MediumTurquoise);
            DrawMenu.AddItem(new MenuItem("drawEnabled", "Draw - Enabled").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawDamage", "Draw - Draw Damage").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawQ", "Draw - Q Range").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawW", "Draw - W Range").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawE", "Draw - E Range").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawR", "Draw - R Range").SetValue(true));

            // Misc Menu
            MiscMenu = new Menu("Drunk Galio - Misc", "miscMenu").SetFontStyle(FontStyle.Bold, Color.MediumTurquoise);
            WMenu = new Menu("W", "wMenu");
            AlliesMenu = new Menu("W - Allies", "alliesMenu");
            MiscMenu.AddSubMenu(WMenu);
            MiscMenu.AddItem(new MenuItem("miscRFlash", "Misc - R Flash").SetValue(new KeyBind('G', KeyBindType.Press)));
            //MiscMenu.AddItem(new MenuItem("test", "test").SetValue(new KeyBind('H', KeyBindType.Press)))
            //    .Permashow(true, "Drunk Galio - TEST", new Color(0, 255, 255));

            // W Menu
            WMenu.AddItem(new MenuItem("wAuto", "W - Auto W").SetValue(true))
                .Permashow(true, "Drunk Galio - Auto W", new Color(0, 255, 255));

            WMenu.AddSubMenu(AlliesMenu);
            WMenu.AddItem(new MenuItem("wIncDamage", "W - Incoming Damage %").SetValue(new Slider(10)));
            foreach (var ally in AlliesList)
            {
                AlliesMenu.AddItem(
                    ally.ChampionName == "Galio"
                        ? new MenuItem(ally.ChampionName, ally.ChampionName + " (Me)").SetValue(true)
                        : new MenuItem(ally.ChampionName, ally.ChampionName).SetValue(true));
            }

            // Add to Menu
            Menu.AddSubMenu(ComboMenu);
            Menu.AddSubMenu(HarassMenu);
            Menu.AddSubMenu(LaneClearMenu);
            Menu.AddSubMenu(EscapeMenu);
            Menu.AddSubMenu(DrawMenu);
            Menu.AddSubMenu(MiscMenu);
            Menu.AddToMainMenu();
        }

        #endregion
    }
}