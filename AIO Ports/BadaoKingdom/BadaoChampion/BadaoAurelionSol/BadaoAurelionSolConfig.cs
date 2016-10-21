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
 namespace BadaoKingdom.BadaoChampion.BadaoAurelionSol
{
    public static class BadaoAurelionSolConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {

            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 700);
            BadaoMainVariables.Q.SetSkillshot(0.25f, 70, 600, true, SkillshotType.SkillshotLine);
            BadaoMainVariables.Q.MinHitChance = HitChance.Medium;
            BadaoMainVariables.Q2 = new Spell(SpellSlot.Q);
            BadaoMainVariables.W = new Spell(SpellSlot.W);
            BadaoMainVariables.E = new Spell(SpellSlot.E);
            BadaoMainVariables.R = new Spell(SpellSlot.R);
            BadaoMainVariables.R.SetSkillshot(0.5f, 200, 10000, false, SkillshotType.SkillshotLine);
            BadaoMainVariables.R.MinHitChance = HitChance.Medium;

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
            //BadaoEliseVariables.ComboE = Combo.AddItem(new MenuItem("ComboE", "E Human")).SetValue(true);
            //BadaoEliseVariables.ComboE2 = Combo.AddItem(new MenuItem("ComboE2", "E Spider")).SetValue(true);
            //BadaoEliseVariables.ComboR = Combo.AddItem(new MenuItem("ComboR", "Switch Form")).SetValue(true);

            // LaneClear
            //Menu LaneClear = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            //BadaoEliseVariables.LaneClearR = Combo.AddItem(new MenuItem("LaneClearR", "Switch Form")).SetValue(true);
            //BadaoEliseVariables.LaneClearMana = LaneClear.AddItem(new MenuItem("LaneClearMana", "Mana Limit")).SetValue(new Slider(30, 0, 100));

            // JungleClear
            Menu JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            //BadaoEliseVariables.JungleQ = JungleClear.AddItem(new MenuItem("JungleQ", "Q Human")).SetValue(true);
            //BadaoEliseVariables.JungleW = JungleClear.AddItem(new MenuItem("JungleW", "W Human")).SetValue(true);
            //BadaoEliseVariables.JungleR = JungleClear.AddItem(new MenuItem("JungleR", "Switch Form")).SetValue(true);
            //BadaoEliseVariables.JungleMana = JungleClear.AddItem(new MenuItem("JungleClearMana", "Mana Limit")).SetValue(new Slider(30, 0, 100));


            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
