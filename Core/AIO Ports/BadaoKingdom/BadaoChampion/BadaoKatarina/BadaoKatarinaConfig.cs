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
 namespace BadaoKingdom.BadaoChampion.BadaoKatarina
{
    using static BadaoKatarinaVariables;
    using static BadaoMainVariables;
    public static class BadaoKatarinaConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 625);
            BadaoMainVariables.Q2 = new Spell(SpellSlot.Q);
            BadaoMainVariables.W = new Spell(SpellSlot.W);
            BadaoMainVariables.E = new Spell(SpellSlot.E, 725);
            BadaoMainVariables.R = new Spell(SpellSlot.R, 550);

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
            ComboCancelRForKS = Combo.AddItem(new MenuItem("ComboCancelRForKS", "Cancel R for KS")).SetValue(true);
            ComboCancelRNoTarget = Combo.AddItem(new MenuItem("ComboCancelRNoTarget", "Cancel R if no target")).SetValue(true);

            // Harass
            Menu Harass = config.AddSubMenu(new Menu("Harass", "Harass"));
            HarassWE = Harass.AddItem(new MenuItem("HarassWE", "W-E spin")).SetValue(true);
            //// LaneClear
            //Menu LaneClear = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));

            //// JungleClear
            //Menu JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));

            // Auto
            Menu Auto = config.AddSubMenu(new Menu("Auto", "Auto"));
            AutoKs = Auto.AddItem(new MenuItem("AutoKs", "KS")).SetValue(true);

            //FleeAndWallJump
            Menu FleeJump = config.AddSubMenu(new Menu("Flee And Walljump", "Flee And Walljump"));
            FleeKey = FleeJump.AddItem(new MenuItem("FleeKey", "Flee Key").SetValue(new KeyBind('G', KeyBindType.Press)));
            JumpKey = FleeJump.AddItem(new MenuItem("JumpKey", "WallJump Key").SetValue(new KeyBind('H', KeyBindType.Press)));

            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
