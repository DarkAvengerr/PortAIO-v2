using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoVeigar
{
    public static class BadaoVeigarConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q,950);
            BadaoMainVariables.Q.SetSkillshot(0.25f, 70f, 2200, true, SkillshotType.SkillshotLine);
            BadaoMainVariables.Q.MinHitChance = HitChance.Medium;
            BadaoMainVariables.Q2 = new Spell(SpellSlot.Q, 950);
            BadaoMainVariables.Q2.SetSkillshot(0.25f, 70f, 2200, false, SkillshotType.SkillshotLine);
            BadaoMainVariables.Q.MinHitChance = HitChance.Medium;
            BadaoMainVariables.W = new Spell(SpellSlot.W, 900);// 112.5 radius , 1.25 sec delay
            BadaoMainVariables.W.SetSkillshot(1.5f, 112.5f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            BadaoMainVariables.W.MinHitChance = HitChance.Medium;
            BadaoMainVariables.E = new Spell(SpellSlot.E, 700); // 375 radius, 500ms delay
            BadaoMainVariables.R = new Spell(SpellSlot.R,650);

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
            BadaoVeigarVariables.ComboQ = Combo.AddItem(new MenuItem("Q", "Q").SetValue(true));

            BadaoVeigarVariables.ComboWAlways = Combo.AddItem(new MenuItem("W always", "W always").SetValue(true));
            BadaoVeigarVariables.ComboWOnCC = Combo.AddItem(new MenuItem("W on CC-ed", "W on CC-ed").SetValue(true));
            foreach (var hero in HeroManager.Enemies)
            {
                Combo.AddItem(new MenuItem("WC" + hero.NetworkId, "W " + hero.ChampionName + "(" + hero.Name + ")").SetValue(true));
            }

            BadaoVeigarVariables.ComboE = Combo.AddItem(new MenuItem("E", "E").SetValue(true));
            foreach (var hero in HeroManager.Enemies)
            {
                Combo.AddItem(new MenuItem("EC" + hero.NetworkId, "E " + hero.ChampionName + "(" + hero.Name + ")").SetValue(true));
            }

            BadaoVeigarVariables.ComboRAlways = Combo.AddItem(new MenuItem("R always", "R always").SetValue(true));
            BadaoVeigarVariables.ComboRKillable = Combo.AddItem(new MenuItem("R killable", "R killable").SetValue(true));
            foreach (var hero in HeroManager.Enemies)
            {
                Combo.AddItem(new MenuItem("RC" + hero.NetworkId, "R " + hero.ChampionName + "(" + hero.Name + ")").SetValue(true));
            }

            // LaneClear
            Menu LaneClear = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            BadaoVeigarVariables.ClearCount = LaneClear.AddItem(new MenuItem("Q if kill x minions:", "Q if kill x minions:").SetValue(new Slider(2, 1, 2)));

            // JungleClear
            Menu JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));

            // Auto
            Menu Auto = config.AddSubMenu(new Menu("Auto", "Auto"));

            // E extra distance
            BadaoVeigarVariables.ExtraEDistance = config.AddItem(new MenuItem("Extra E Distance Reduce", "Extra E Distance Reduce").SetValue(new Slider(50, 0, 200)));

            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
