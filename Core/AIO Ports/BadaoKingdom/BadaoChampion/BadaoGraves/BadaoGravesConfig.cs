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
 namespace BadaoKingdom.BadaoChampion.BadaoGraves
{
    public static class BadaoGravesConfig
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Menu config;
        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 750);
            BadaoMainVariables.Q.SetSkillshot(0.25f, 100, 2500, false, SkillshotType.SkillshotLine);
            BadaoMainVariables.Q.MinHitChance = HitChance.Medium;
            BadaoMainVariables.W = new Spell(SpellSlot.W, 850);
            BadaoMainVariables.W.SetSkillshot(0.5f, 100, 1600, false, SkillshotType.SkillshotCircle);
            BadaoMainVariables.W.MinHitChance = HitChance.Medium;
            BadaoMainVariables.E = new Spell(SpellSlot.E, 425); // radius 260
            BadaoMainVariables.R = new Spell(SpellSlot.R, 1600);
            BadaoMainVariables.R.SetSkillshot(0.25f, 100, 2100, false, SkillshotType.SkillshotLine);
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
            BadaoGravesVariables.ComboQ = Combo.AddItem(new MenuItem("ComboQ", "Q")).SetValue(true);
            BadaoGravesVariables.ComboW = Combo.AddItem(new MenuItem("ComboW", "W")).SetValue(true);
            BadaoGravesVariables.ComboE = Combo.AddItem(new MenuItem("ComboE", "E")).SetValue(true);
            BadaoGravesVariables.ComboR = Combo.AddItem(new MenuItem("ComboR", "R")).SetValue(true);

            //Jungle
            Menu JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            BadaoGravesVariables.JungleQ = JungleClear.AddItem(new MenuItem("JungleQ", "Q")).SetValue(true);
            BadaoGravesVariables.JungleE = JungleClear.AddItem(new MenuItem("JungleE", "E")).SetValue(true);
            BadaoGravesVariables.ManaJungle = JungleClear.AddItem(new MenuItem("ManaJungle", "Min Mana").SetValue(new Slider(40, 0, 100)));

            // Auto
            Menu Auto = config.AddSubMenu(new Menu("Auto", "Auto"));
            BadaoGravesVariables.AutoSmite = Auto.AddItem(new MenuItem("AutoSmite", "Smite Baron, Dragon")).SetValue(true);
            BadaoGravesVariables.AutoRKS = Auto.AddItem(new MenuItem("AutoRKS", "R KS")).SetValue(true);
            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
