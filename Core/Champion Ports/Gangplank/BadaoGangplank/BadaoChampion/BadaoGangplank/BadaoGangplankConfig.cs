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
            BadaoGangplankVariables.ComboQSave = Combo.AddItem(new MenuItem("ComboQSave", "Save Q if can detonate barrels")).SetValue(false);


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

            //Flee nam o trong config luon nha
            Menu Flee = config.AddSubMenu(new Menu("Flee", "Flee"));
            BadaoGangplankVariables.FleeKey = Flee.AddItem(new MenuItem("FleeKey", "Flee Key")).SetValue(new KeyBind('G', KeyBindType.Press));

            //Draw nam o trong config luon ne
            Menu Draw = config.AddSubMenu(new Menu("Draw", "Draw"));
            BadaoGangplankVariables.DrawQ = Draw.AddItem(new MenuItem("DrawQ", "Q")).SetValue(true);
            BadaoGangplankVariables.DrawE = Draw.AddItem(new MenuItem("DrawE", "E")).SetValue(true);
            BadaoGangplankVariables.DrawEPlacement = Draw.AddItem(new MenuItem("DrawEPlacement", "E Chain range")).SetValue(true);

            // attach to mainmenu
            config.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!BadaoGangplankVariables.FleeKey.GetValue<KeyBind>().Active)
                return;
            Orbwalking.Orbwalk(null, Game.CursorPos);
            if (BadaoMainVariables.Q.IsReady())
            {
                foreach (var barrel in BadaoGangplankBarrels.QableBarrels())
                {
                    if (BadaoMainVariables.Q.Cast(barrel.Bottle) == Spell.CastStates.SuccessfullyCasted)
                        return;
                }
            }
            if (Orbwalking.CanAttack())
            {
                foreach (var barrel in BadaoGangplankBarrels.AttackableBarrels())
                {
                    Orbwalking.Attack = false;
                    Orbwalking.Move = false;
                    LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () =>
                    {
                        Orbwalking.Attack = true;
                        Orbwalking.Move = true;
                    }); 
                    if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle))
                        return;
                }
            }
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
            if (BadaoGangplankVariables.DrawEPlacement.GetValue<bool>())
            {
                foreach (var barrel in BadaoGangplankBarrels.Barrels)
                {
                    Render.Circle.DrawCircle(barrel.Bottle.Position, 660, Color.Red );
                }
            }
        }
    }
}
