using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using BadaoMissFortune;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoMissFortune
{
    public static class BadaoMissFortuneConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 650);
            BadaoMainVariables.Q.SetTargetted(0.25f, 1400);
            BadaoMainVariables.W = new Spell(SpellSlot.W); 
            BadaoMainVariables.E = new Spell(SpellSlot.E, 1000); // radius 200
            BadaoMainVariables.R = new Spell(SpellSlot.R, 1400); // chua biet goc

            // main menu
            config = new Menu("BadaoKingdom " + ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);
            config.SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.YellowGreen);

            // orbwalker menu
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            BadaoMainVariables.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            config.AddSubMenu(orbwalkerMenu);

            // TS
            Menu ts = config.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            // Combo
            Menu Combo = config.AddSubMenu(new Menu("Combo", "Combo"));
            BadaoMissFortuneVariables.ComboQ1 = Combo.AddItem(new MenuItem("ComboQ1", "Q1")).SetValue(true);
            BadaoMissFortuneVariables.ComboQ2 = Combo.AddItem(new MenuItem("ComboQ2", "Q2")).SetValue(true);
            BadaoMissFortuneVariables.ComboW = Combo.AddItem(new MenuItem("ComboW", "W")).SetValue(true);
            BadaoMissFortuneVariables.ComboE = Combo.AddItem(new MenuItem("ComboE", "E")).SetValue(true);
            BadaoMissFortuneVariables.ComboR = Combo.AddItem(new MenuItem("ComboR", "R Killable")).SetValue(true);
            BadaoMissFortuneVariables.ComboRWise = Combo.AddItem(new MenuItem("ComboRWise", "R Killable Wisely")).SetValue(true);
            BadaoMissFortuneVariables.ComboRifhit = Combo.AddItem(new MenuItem("ComboRifhit", "R if hit")).SetValue(true);
            BadaoMissFortuneVariables.ComboRifwillhit = Combo.AddItem(new MenuItem("ComboRifwillhit", "R if will hit")).SetValue(new Slider(3, 1, 5));

            // Harass
            Menu Harass = config.AddSubMenu(new Menu("Harass", "Harass"));
            BadaoMissFortuneVariables.HarassQ1 = Harass.AddItem(new MenuItem("HarassQ1", "Q1")).SetValue(true);
            BadaoMissFortuneVariables.HarassQ2 = Harass.AddItem(new MenuItem("HarassQ2", "Q2")).SetValue(true);
            BadaoMissFortuneVariables.HarassE = Harass.AddItem(new MenuItem("HarassE", "E")).SetValue(false);
            BadaoMissFortuneVariables.HarassMana = Harass.AddItem(new MenuItem("HarassMana", "Min Mana To Harass")).SetValue(new Slider(30, 0, 100));

            // LaneClear
            Menu LaneClear = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            BadaoMissFortuneVariables.LaneClearQ = LaneClear.AddItem(new MenuItem("LaneClearQ", "Q")).SetValue(true);
            BadaoMissFortuneVariables.LaneClearW = LaneClear.AddItem(new MenuItem("LaneClearW", "W")).SetValue(true);
            BadaoMissFortuneVariables.LaneClearE = LaneClear.AddItem(new MenuItem("LaneClearE", "E")).SetValue(false);
            BadaoMissFortuneVariables.LaneClearMana = LaneClear.AddItem(new MenuItem("LaneClearMana", "Min Mana To LaneClear")).SetValue(new Slider(30, 0, 100));

            // JungleClear
            Menu JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            BadaoMissFortuneVariables.JungleClearQ = JungleClear.AddItem(new MenuItem("JungleClearQ", "Q")).SetValue(true);
            BadaoMissFortuneVariables.JungleClearW = JungleClear.AddItem(new MenuItem("JungleClearW", "W")).SetValue(true);
            BadaoMissFortuneVariables.JungleClearE = JungleClear.AddItem(new MenuItem("JungleClearE", "E")).SetValue(false);
            BadaoMissFortuneVariables.JungleClearMana = JungleClear.AddItem(new MenuItem("JungleClearMana", "Min Mana To JungleClear")).SetValue(new Slider(30, 0, 100));

            // Auto
            Menu Auto = config.AddSubMenu(new Menu("Auto", "Auto"));
            foreach (var hero in HeroManager.Enemies)
            {
                Auto.AddItem(new MenuItem("AutoQ2" + hero.NetworkId, "Q2 " + hero.ChampionName + " (" + hero.Name + ")")).SetValue(true);
            }
            BadaoMissFortuneVariables.AutoMana = Auto.AddItem(new MenuItem("AutoMana", "Min Mana To Auto")).SetValue(new Slider(30, 0, 100));

            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
