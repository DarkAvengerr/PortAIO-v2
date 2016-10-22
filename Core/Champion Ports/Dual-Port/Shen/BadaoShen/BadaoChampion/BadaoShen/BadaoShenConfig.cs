using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using BadaoShen;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoShen
{
    public static class BadaoShenConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q);
            BadaoMainVariables.Q.SetSkillshot(0f, 50f, 2500, false, SkillshotType.SkillshotLine, BadaoShenVariables.SwordPos.To3D());
            BadaoMainVariables.W = new Spell(SpellSlot.W, 300);
            BadaoMainVariables.E = new Spell(SpellSlot.E, 600);
            BadaoMainVariables.E.SetSkillshot(0f, 50f, 1600f, false, SkillshotType.SkillshotLine);
            BadaoMainVariables.R = new Spell(SpellSlot.R);

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
            BadaoShenVariables.ComboQ = Combo.AddItem(new MenuItem("ComboQ", "Q")).SetValue(true);
            BadaoShenVariables.ComboW = Combo.AddItem(new MenuItem("ComboW", "W")).SetValue(true);
            Menu EToTarget = Combo.AddSubMenu(new Menu("E to Target", "EToTarget"));
            foreach (var hero in HeroManager.Enemies)
            {
                EToTarget.AddItem(new MenuItem("EToTarget" + hero.NetworkId, hero.ChampionName + " (" + hero.Name + ")")).SetValue(true);
            }
            BadaoShenVariables.ComboEIfHit = Combo.AddItem(new MenuItem("ComboEIfHit", "E If Hit")).SetValue(true);
            BadaoShenVariables.ComboEIfWillHit = Combo.AddItem(new MenuItem("ComboEIfWillHit", "If Will Hit")).SetValue(new Slider(3, 1, 5));
            Menu Wtarget = Combo.AddSubMenu(new Menu("W to protect", "WProtect"));
            foreach (var hero in HeroManager.Allies)
            {
                Wtarget.AddItem(new MenuItem("WProtect" + hero.NetworkId, hero.ChampionName + " (" + hero.Name + ")")).SetValue(true);
            }

            // LaneClear
            Menu LaneClear = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            BadaoShenVariables.LaneClearQ = LaneClear.AddItem(new MenuItem("LaneClearQ", "Q")).SetValue(true);

            // JungleClear
            Menu JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            BadaoShenVariables.JungleClearQ = JungleClear.AddItem(new MenuItem("JungleClearQ", "Q")).SetValue(true);

            // Auto
            Menu Auto = config.AddSubMenu(new Menu("Auto", "Auto"));
            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe))
            {
                Auto.AddItem(new MenuItem("AutoR" + hero.NetworkId, "R " + hero.ChampionName + " (" + hero.Name + ")")).SetValue(true);
            }
            BadaoShenVariables.RHp = Auto.AddItem(new MenuItem("RHp", "% Hp to R")).SetValue(new Slider(20, 0, 100));

            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
