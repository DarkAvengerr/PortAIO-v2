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
 namespace BadaoKingdom.BadaoChampion.BadaoJhin
{
    public static class BadaoJhinConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 600);
            BadaoMainVariables.W = new Spell(SpellSlot.W, 2500);
            BadaoMainVariables.W.SetSkillshot(1f, 100, 10000,true, SkillshotType.SkillshotLine);
            BadaoMainVariables.W.MinHitChance = HitChance.Medium;
            BadaoMainVariables.E = new Spell(SpellSlot.E, 750); // radius 260
            BadaoMainVariables.R = new Spell(SpellSlot.R, 3500);
            BadaoMainVariables.R.SetSkillshot(0.45f, 100, 5000, false, SkillshotType.SkillshotLine);
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
            BadaoJhinVariables.ComboQ = Combo.AddItem(new MenuItem("ComboQ", "Q")).SetValue(true);
            BadaoJhinVariables.ComboW = Combo.AddItem(new MenuItem("ComboW", "W for damage")).SetValue(true);
            BadaoJhinVariables.ComboWOnlySnare = Combo.AddItem(new MenuItem("ComboWOnlySnare", "W for Rooting")).SetValue(true);
            BadaoJhinVariables.ComboE = Combo.AddItem(new MenuItem("ComboE", "E")).SetValue(true);

            // Harass
            Menu Harass = config.AddSubMenu(new Menu("Harass", "Harass"));
            BadaoJhinVariables.HarassQ = Harass.AddItem(new MenuItem("HarassQ", "Q")).SetValue(true);
            BadaoJhinVariables.HarassW = Harass.AddItem(new MenuItem("HarassW", "W")).SetValue(true);
            BadaoJhinVariables.HarassE = Harass.AddItem(new MenuItem("HarassE", "E")).SetValue(true);
            BadaoJhinVariables.HarassMana = Harass.AddItem(new MenuItem("HarassMana", "Mana Limit")).SetValue(new Slider(30, 0, 100));

            // LaneClear
            Menu LaneClear = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            BadaoJhinVariables.LaneClearQ = LaneClear.AddItem(new MenuItem("LaneClearQ", "Q")).SetValue(true);
            BadaoJhinVariables.LaneClearMana = LaneClear.AddItem(new MenuItem("LaneClearMana", "Mana Limit")).SetValue(new Slider(30, 0, 100));

            // JungleClear
            Menu JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            BadaoJhinVariables.JungleClearQ = JungleClear.AddItem(new MenuItem("JungClearQ", "Q")).SetValue(true);
            BadaoJhinVariables.JungleClearW = JungleClear.AddItem(new MenuItem("JungClearW", "W")).SetValue(true);
            BadaoJhinVariables.JungleClearE = JungleClear.AddItem(new MenuItem("JungClearE", "E")).SetValue(true);
            BadaoJhinVariables.JungleClearMana = JungleClear.AddItem(new MenuItem("JungleClearMana", "Mana Limit")).SetValue(new Slider(30, 0, 100));

            // Auto
            Menu Auto = config.AddSubMenu(new Menu("Auto", "Auto"));
            BadaoJhinVariables.AutoPingKillable = Auto.AddItem(new MenuItem("AutoPingKillable", "Auto Ping Killable with R")).SetValue(true);
            BadaoJhinVariables.AutoR = Auto.AddItem(new MenuItem("AutoR", "Use R if channeling")).SetValue(true);
            BadaoJhinVariables.AutoRMode = Auto.AddItem(new MenuItem("AutoRMode", "R mode")).SetValue(new StringList(new string[] {"Auto","OnTap" }, 0));
            BadaoJhinVariables.AutoRTarget = Auto.AddItem(new MenuItem("AutoRTargets", "R targets")).SetValue(new StringList(new string[] { "Selected", "Near Mouse","Auto" }, 2));
            BadaoJhinVariables.AutoRTapKey = Auto.AddItem(new MenuItem("AutoRTapKey", "R tap key")).SetValue(new KeyBind('T', KeyBindType.Press));
            BadaoJhinVariables.AutoW = Auto.AddItem(new MenuItem("AutoW", "Use W on Slowed Target")).SetValue(true);
            BadaoJhinVariables.AutoWTrap  = Auto.AddItem(new MenuItem("AutoWTrap", "Use W Target on Trap")).SetValue(true);
            BadaoJhinVariables.AutoKS = Auto.AddItem(new MenuItem("AutoKS", "KS")).SetValue(true);
            BadaoJhinVariables.AutoMana = Auto.AddItem(new MenuItem("AutoMana", "Mana Limit")).SetValue(new Slider(30, 0, 100));

            // Draw
            Menu Draw = config.AddSubMenu(new Menu("Draw", "Draw"));
            BadaoJhinVariables.DrawWMiniMap = Draw.AddItem(new MenuItem("DrawWMiniMap", "Draw W on minimap")).SetValue(false);

            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
