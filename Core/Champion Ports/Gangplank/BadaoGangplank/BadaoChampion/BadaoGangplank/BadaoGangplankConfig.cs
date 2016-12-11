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
namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {

            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 625);
            BadaoMainVariables.W = new Spell(SpellSlot.W);
            BadaoMainVariables.E = new Spell(SpellSlot.E,1000);
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
            BadaoGangplankVariables.ComboE1 = Combo.AddItem(new MenuItem("ComboE1", "Place 1st Barrel")).SetValue(true);


            // Harass
            Menu Harass = config.AddSubMenu(new Menu("Harass", "Harass"));
            BadaoGangplankVariables.HarassQ = Harass.AddItem(new MenuItem("HarassQ", "Q")).SetValue(true);


            // LaneClear
            Menu LaneClear = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            BadaoGangplankVariables.LaneQ = LaneClear.AddItem(new MenuItem("LaneQ", "Use Q last hit")).SetValue(true);

            // JungleClear
            Menu JungleClear = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            BadaoGangplankVariables.JungleQ = JungleClear.AddItem(new MenuItem("jungleQ", "Use Q last hit")).SetValue(true);

            //Auto
            Menu Auto = config.AddSubMenu(new Menu("Auto", "Auto"));
            BadaoGangplankVariables.AutoWLowHealth = Auto.AddItem(new MenuItem("AutoWLowHealth", "W when low health")).SetValue(true);
            BadaoGangplankVariables.AutoWLowHealthValue = Auto.AddItem(new MenuItem("AutoWLowHealthValue", "% HP to W")).SetValue(new Slider(20,1,100));
            BadaoGangplankVariables.AutoWCC = Auto.AddItem(new MenuItem("AutoWCC", "W anti CC")).SetValue(true);

            //Draw
            Menu Draw = config.AddSubMenu(new Menu("Draw", "Draw"));
            BadaoGangplankVariables.DrawQ = Draw.AddItem(new MenuItem("DrawQ", "Q")).SetValue(true);
            BadaoGangplankVariables.DrawE = Draw.AddItem(new MenuItem("DrawE", "E")).SetValue(true);

            // attach to mainmenu
            config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;
            if (BadaoGangplankVariables.DrawQ.GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, BadaoMainVariables.Q.Range, Color.Yellow);
            }
            if (BadaoGangplankVariables.DrawE.GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, BadaoMainVariables.E.Range, Color.Pink);
            }
        }
    }
}
