using SoloDZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SOLOVayne.Utility.General;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Utility
{
    class MenuGenerator
    {
        /// <summary>
        /// The main menu
        /// </summary>
        private Menu MainMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuGenerator"/> class.
        /// </summary>
        public MenuGenerator()
        {
            if (Variables.Menu == null)
            {
                Variables.Menu = new Menu("[SOLO] Vayne", "solo.vayne", true);
            }

            MainMenu = Variables.Menu;
        }

        /// <summary>
        /// Generates the menu.
        /// </summary>
        public void GenerateMenu()
        {
            var owMenu = new Menu("[SOLO] Orbwalker", "solo.vayne.orbwalker");
            {
                Variables.Orbwalker = new Orbwalking.Orbwalker(owMenu);
                MainMenu.AddSubMenu(owMenu);
            }

            var comboMenu = MainMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Combo);
            {
                comboMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Combo, true, 10);
                comboMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Combo, true, 5);
                comboMenu.AddSkill(SpellSlot.R, Orbwalking.OrbwalkingMode.Combo, true, 5);
            }

            var harassMenu = MainMenu.AddModeMenu(Orbwalking.OrbwalkingMode.Mixed);
            {
                harassMenu.AddStringList("solo.vayne.mixed.mode", "Harass Mode", new[] { "Passive", "Agressive" }, 1);
                harassMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.Mixed, true, 10);
                harassMenu.AddSkill(SpellSlot.E, Orbwalking.OrbwalkingMode.Mixed, true, 5);
            }

            var farmMenu = MainMenu.AddModeMenu(Orbwalking.OrbwalkingMode.LaneClear);
            {
                farmMenu.AddSkill(SpellSlot.Q, Orbwalking.OrbwalkingMode.LaneClear, true, 50);
                farmMenu.AddBool("solo.vayne.laneclear.condemn.jungle", "Condemn Jungle Mobs", true);
            }

            var miscMenu = MainMenu.AddSubMenu(new Menu("[SOLO] Miscellaneous", "solo.vayne.misc"));
            {
                var QMenu = miscMenu.AddSubMenu(new Menu("Tumble", "solo.vayne.misc.tumble"));
                {
                    QMenu.AddBool("solo.vayne.misc.tumble.noqintoenemies", "Don't Q into enemies", true);
                    QMenu.AddBool("solo.vayne.misc.tumble.qks", "Q for Killsteal", true);
                    QMenu.AddBool("solo.vayne.misc.tumble.smartQ", "Use SOLO Vayne Q Logic", true);
                }

                var EMenu = miscMenu.AddSubMenu(new Menu("Condemn", "solo.vayne.misc.condemn"));
                {
                    EMenu.AddBool("solo.vayne.misc.condemn.autoe", "Auto E");
                    EMenu.AddBool("solo.vayne.misc.condemn.current", "Only E Current Target", true);
                    EMenu.AddBool("solo.vayne.misc.condemn.save", "SOLO: Save Me", true).SetTooltip("Saves you using E if there are enemies who are going to kill you.");
                }

                var MiscMenu = miscMenu.AddSubMenu(new Menu("Miscellaneous", "solo.vayne.misc.miscellaneous"));
                {
                    MiscMenu.AddBool("solo.vayne.misc.miscellaneous.antigapcloser", "Antigapcloser", true);
                    MiscMenu.AddBool("solo.vayne.misc.miscellaneous.interrupter", "Interrupter", true);
                    MiscMenu.AddBool("solo.vayne.misc.miscellaneous.noaastealth", "Don't AA while stealthed");
                    MiscMenu.AddSlider("solo.vayne.misc.miscellaneous.delay", "Antigapcloser / Interrupter Delay", 300, 0, 1000);
                }
            }

            MainMenu.AddToMainMenu();
        }
    }
}
